
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload
{
    public class CategoryData
    {
        private string _Category;
        public string Category
        {
            get { return this._Category; }
            set { this._Category = value; }
        }

        private List<FileData> _FilesData;
        public List<FileData> FilesData
        {
            get { return this._FilesData; }
            set { this._FilesData = value; }
        }

    }
}
