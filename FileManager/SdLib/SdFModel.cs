
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SdLib
{
    public class SdFModel
    {
        private string _name;
        public string Name 
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

            }
        }

        public Brush BgBrush { get; set; }

        public string Path { get; set; }

        public SdFModel()
        {
            if (this is SdFile)
            {
                BgBrush = new SolidColorBrush(Colors.White);
            }
            else
            {
                BgBrush = new SolidColorBrush(Colors.Yellow);
            }
        }
    }
}
