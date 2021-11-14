using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

    internal class ProgressableStreamContent : HttpContent
    {
        /// <summary>
        /// Lets keep buffer of 20kb
        /// </summary>
        private const int defaultBufferSize = 5 * 4096;

        private HttpContent content;

        private int bufferSize;

        //private bool contentConsumed;
        private Action<long, long> progress;

        public ProgressableStreamContent(HttpContent content, Action<long, long> progress) : this(content,
            defaultBufferSize, progress)
        {
        }

        public ProgressableStreamContent(HttpContent content, int bufferSize, Action<long, long> progress)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            this.content = content;
            this.bufferSize = bufferSize;
            this.progress = progress;

            foreach (var h in content.Headers)
            {
                this.Headers.Add(h.Key, h.Value);
            }
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return Task.Run(async () =>
            {
                var buffer = new Byte[this.bufferSize];
                long size;
                TryComputeLength(out size);
                var uploaded = 0;


                using (var sinput = await content.ReadAsStreamAsync())
                {
                    while (true)
                    {
                        var length = sinput.Read(buffer, 0, buffer.Length);
                        if (length <= 0) break;

                        //downloader.Uploaded = uploaded += length;
                        uploaded += length;
                        progress?.Invoke(uploaded, size);

                        //System.Diagnostics.Debug.WriteLine($"Bytes sent {uploaded} of {size}");

                        stream.Write(buffer, 0, length);
                        stream.Flush();
                    }
                }

                stream.Flush();
            });
        }

        protected override bool TryComputeLength(out long length)
        {
            length = content.Headers.ContentLength.GetValueOrDefault();
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetFiles();
        }

        private async void GetFiles()
        {
            HttpClient client = new HttpClient();
            String filesURL = "http://localhost:5000/files";
            using (HttpResponseMessage response = client.GetAsync(filesURL).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    Trace.WriteLine(response.Content.ReadAsStringAsync().Result);
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
                multiPartStream.Add(new StringContent("file_name"), "file_name");
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
