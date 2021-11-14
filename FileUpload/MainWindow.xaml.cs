    using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
                multiPartStream.Add(new StringContent("{}"), "metadata");
                multiPartStream.Add(new StringContent("file_name"), "file_name");
                multiPartStream.Add(new StringContent("file_desc"), "file_desc");
                multiPartStream.Add(new ByteArrayContent(fileContent, 0, fileContent.Length), "uploaded_file", System.IO.Path.GetFileName(filePath));
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
