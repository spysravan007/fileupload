using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload
{
    public class FileData
    {
        private string _FileName;
        public string FileName
        {
            get { return this._FileName; }
            set { this._FileName = value; }
        }

        private System.Windows.Media.Imaging.BitmapImage _ImageData;
        public System.Windows.Media.Imaging.BitmapImage ImageData
        {
            get { return this._ImageData; }
            set { this._ImageData = value; }
        }

    }
}
