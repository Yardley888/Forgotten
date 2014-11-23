using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MediaLib
{
    public class MSong
    {
        public string Name { get; set; }
        public string Time { get; set; }
        public int PlayCount { get; set; }
        public string Singer { get; set; }

        public Song Song { get; set; }
    }

    public class MPicture : System.ComponentModel.INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int FontSize { get; private set; }
        private string _name;
        public string Name 
        {
            get { return _name; }
            set
            {
                _name = value;
                if (FontSize == 0)
                {
                    TextBlock tb = new TextBlock { Text = _name, FontSize = 25 };
                    if (tb.ActualWidth > 355)
                        FontSize = 20;
                    else
                        FontSize = 25;
                }
            }
        }
        public string Path { get; set; }
        public DateTime Date { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public long Length { get; private set; }

        public string SizeString
        {
            get
            {
                return Width.ToString() + " X " + Height.ToString();
            }
        }

        public string DateString
        {
            get
            {
                return Date.ToString("yyyy/MM/dd HH:ss");
            }
        }

        private string _lengthString = string.Empty;
        public string LengthString
        {
            set
            {
                _lengthString = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LengthString"));
                }
            }
            get
            {
                return _lengthString;
            }
        }

        public Picture Picture { get; set; }

        public void CheckAndSetLenthString(long length)
        {
            if (this.Length == 0 || string.IsNullOrEmpty(_lengthString))
            {
                this.Length = length;

                var kb = Length / 1024;
                var mb = kb / 1024.0;
                if (mb > 1)
                {
                    LengthString = Math.Round(mb, 1).ToString() + "MB";
                }
                else
                {
                    LengthString = kb.ToString() + "KB";
                }
            }
        }

        public bool bSetLength()
        {
            if (this.Length == 0 || string.IsNullOrEmpty(_lengthString))
            {
                return false;
            }
            return true;
        }
    }

    public class MVideo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime Date { get; set; }

    }

    public class MpictureAlbum
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public Picture LastPic 
        { get; set; }
        public PictureAlbum PictureAlbum { get; set; }
    }

}
