using Newtonsoft.Json.Linq;
using Syroot.Windows.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FileUpload
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            GetFiles();
        }

        private void AddFileToView(String file_id, String file_name, String file_desc, String file_path)
        {
            var bmp = GetFileIcon(file_name).ToBitmap();
            this.FileViewer.Items.Add(new FileData { FileID = file_id, FileDesc = file_desc, FilePath = file_path, FileName = file_name, ImageData = ToBitmapImage(bmp) });
        }

        public static System.Drawing.Icon GetFileIcon(string name)
        {
            Shell32.Shfileinfo shfi = new Shell32.Shfileinfo();
            uint flags = Shell32.ShgfiIcon | Shell32.ShgfiUsefileattributes;
            flags += Shell32.ShgfiLinkoverlay;
            flags += Shell32.ShgfiLargeicon;
            Shell32.SHGetFileInfo(name, Shell32.FileAttributeNormal, ref shfi, (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi), flags);
            System.Drawing.Icon icon = (System.Drawing.Icon) System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
            User32.DestroyIcon(shfi.hIcon);
            return icon;
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        private async void GetFiles()
        {
            HttpClient client = new HttpClient();
            String filesURL = "http://localhost:5000/files";
            using (HttpResponseMessage response = client.GetAsync(filesURL).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    var objects = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    JArray array = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    foreach (JObject obj in array.Children<JObject>())
                    {
                        AddFileToView(obj.GetValue("file_id").ToString(), obj.GetValue("file_name").ToString(), obj.GetValue("file_desc").ToString(), obj.GetValue("file_path").ToString());
                    }
                }
            }
        }

        private async void DownloadFile(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri((sender as MenuItem).Tag.ToString());
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(uri);
            using (var fs = new FileStream(
                new KnownFolder(KnownFolderType.Downloads).Path + "/" + 
                (sender as MenuItem).Tag.ToString().Split('/').Last(),
                FileMode.CreateNew))
            {
                await response.Content.CopyToAsync(fs);
            }
        }

        private async void DeleteFile(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("http://localhost:5000/file/" + (sender as MenuItem).Tag.ToString());
            HttpClient client = new HttpClient();
            var response = client.DeleteAsync(uri).Result;
            if (response.IsSuccessStatusCode)
            {
                DeleteFileFromView((sender as MenuItem).Tag.ToString());
            }
        }

        private void DeleteFileFromView(String file_id)
        {
            foreach(FileData f in this.FileViewer.Items)
            {
                if(f.FileID == file_id)
                {
                    this.FileViewer.Items.Remove(f);
                    break;
                }
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {         
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                String filePath = openFileDlg.FileName;
                UploadFile(filePath);
            }
        }

        private async void UploadFile(String filePath)
        {
            this.BrowseButton.Visibility = Visibility.Hidden;
            this.pBar.Visibility = Visibility.Visible;
            string uploadURL = "http://localhost:5000/files";
            HttpClient client = new HttpClient();
            Byte[] fileContent = System.IO.File.ReadAllBytes(filePath);
            using (var multiPartStream = new MultipartFormDataContent())
            {
                var uploaded_file = new ProgressableStreamContent(new StreamContent(File.OpenRead(filePath)), (sent, total) => {
                    //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate {
                    //    pBar.Dispatcher.BeginInvoke((Action)(() => pBar.Value = (int)(sent * 100 / total)));
                    //}));
                });
                multiPartStream.Add(new StringContent(System.IO.Path.GetFileName(filePath)), "file_name");
                multiPartStream.Add(new StringContent("file_desc"), "file_desc");
                multiPartStream.Add(uploaded_file, "uploaded_file", System.IO.Path.GetFileName(filePath));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uploadURL);
                request.Content = multiPartStream;
                using (HttpResponseMessage response = client.SendAsync(request).Result)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        JObject obj = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        Trace.WriteLine(obj.GetValue("message"));
                        obj = JObject.Parse(obj.GetValue("data").ToString());
                        AddFileToView(obj.GetValue("file_id").ToString(), obj.GetValue("file_name").ToString(), obj.GetValue("file_desc").ToString(), obj.GetValue("file_path").ToString());
                        this.pBar.Visibility = Visibility.Hidden;
                        this.BrowseButton.Visibility = Visibility.Visible;
                    }
                }

            }
        }
    }
}
