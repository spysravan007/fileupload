
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

        private string _FileID;
        public string FileID
        {
            get { return this._FileID; }
            set { this._FileID = value; }
        }

        private string _FilePath;
        public string FilePath
        {
            get { return this._FilePath; }
            set { this._FilePath = value; }
        }

    }
}
