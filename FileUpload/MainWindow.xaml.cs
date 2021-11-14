using Newtonsoft.Json.Linq;
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
            Trace.WriteLine(file_name);
            //FileViewer.Children.Add(new TextBlock() { Text = file_name });
            //var icon = System.Drawing.Icon.ExtractAssociatedIcon(file_name);
            //var bmp = icon.ToBitmap();
            var bmp = GetFileIcon(file_name).ToBitmap();
            this.FileViewer.Items.Add(new FileData { FileName = file_name, ImageData = ToBitmapImage(bmp) });
            //icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
            //        sysicon.Handle,
            //        System.Windows.Int32Rect.Empty,
            //        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions())

        }

        static class Shell32
        {
            private const int MaxPath = 256;
            [StructLayout(LayoutKind.Sequential)]
            public struct Shfileinfo
            {
                private const int Namesize = 80;
                public readonly IntPtr hIcon;
                private readonly int iIcon;
                private readonly uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
                private readonly string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Namesize)]
                private readonly string szTypeName;
            };
            public const uint ShgfiIcon = 0x000000100;     // get icon
            public const uint ShgfiLinkoverlay = 0x000008000;     // put a link overlay on icon
            public const uint ShgfiLargeicon = 0x000000000;     // get large icon
            public const uint ShgfiSmallicon = 0x000000001;     // get small icon
            public const uint ShgfiUsefileattributes = 0x000000010;     // use passed dwFileAttribute
            public const uint FileAttributeNormal = 0x00000080;
            [DllImport("Shell32.dll")]
            public static extern IntPtr SHGetFileInfo(
                string pszPath,
                uint dwFileAttributes,
                ref Shfileinfo psfi,
                uint cbFileInfo,
                uint uFlags
                );
        }
        static class User32
        {
            /// <summary>
            /// Provides access to function required to delete handle. This method is used internally
            /// and is not required to be called separately.
            /// </summary>
            /// <param name="hIcon">Pointer to icon handle.</param>
            /// <returns>N/A</returns>
            [DllImport("User32.dll")]
            public static extern int DestroyIcon(IntPtr hIcon);
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
                        Trace.WriteLine(obj.GetValue("file_name"));
                        AddFileToView(obj.GetValue("file_id").ToString(), obj.GetValue("file_name").ToString(), obj.GetValue("file_desc").ToString(), obj.GetValue("file_path").ToString());
                    }
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
            string uploadURL = "http://localhost:5000/files";
            HttpClient client = new HttpClient();
            Byte[] fileContent = System.IO.File.ReadAllBytes(filePath);
            using (var multiPartStream = new MultipartFormDataContent())
            {
                var uploaded_file = new ProgressableStreamContent(new StreamContent(File.OpenRead(filePath)), (sent, total) => {
                    Trace.WriteLine("Uploading " + ((float)sent / total) * 100f);
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
                        Trace.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }

            }
        }
    }
}
