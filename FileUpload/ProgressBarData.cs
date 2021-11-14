using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload
{
    public class ProgressBarData
    {
        private int _ProgressValue;
        public int ProgressValue
        {
            get { return this._ProgressValue; }
            set { this._ProgressValue = value; }
        }
    }
}
