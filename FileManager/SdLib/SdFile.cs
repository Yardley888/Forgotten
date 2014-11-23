using Microsoft.Phone.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SdLib
{
    public class SdFile : SdFModel
    {
        public ExternalStorageFile Storage { get; set; }
        public string Type { get; set; }
        public DateTime ModifyTime { get; set; }
    }
}
