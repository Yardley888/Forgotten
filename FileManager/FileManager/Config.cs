using MediaLib;
using SdLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FileManager
{
    public class Config
    {
        public static MPicture CurrentChoosePicture { get; set; }
        public static MpictureAlbum CurrentChooseAlbum { get; set; }

        public static SolidColorBrush bgBrush { get; set; }

        public static bool IsLocked { get; set; }
    }
}
