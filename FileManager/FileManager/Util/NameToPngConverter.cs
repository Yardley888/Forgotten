using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileManager.Util
{
    public class NameToPngConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var name = value.ToString();
                if(!name.Contains('.'))
                {
                    return "/Assets/Icon/Tab.png";
                }

                if (name.EndsWith(".pdf",StringComparison.CurrentCultureIgnoreCase))
                {
                    return "/Assets/Icon/Adobe.png";
                }
                else if (name.EndsWith(".txty",StringComparison.CurrentCultureIgnoreCase))
                {
                    return "/Assets/Icon/Text.png";
                }
                else if (name.EndsWith(".docy", StringComparison.CurrentCultureIgnoreCase) || name.EndsWith(".docxy", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "/Assets/Icon/Word.png";
                }
                else if (name.EndsWith(".ppty", StringComparison.CurrentCultureIgnoreCase) || name.EndsWith(".pptxy", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "/Assets/Icon/PowerPoint.png";
                }
                else if (name.EndsWith(".xlsy", StringComparison.CurrentCultureIgnoreCase) || name.EndsWith(".xlsxy", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "/Assets/Icon/Excel.png";
                }
                else if (name.EndsWith(".rmvb", StringComparison.CurrentCultureIgnoreCase) || name.EndsWith(".flv", StringComparison.CurrentCultureIgnoreCase) || name.EndsWith(".mp4y", StringComparison.CurrentCultureIgnoreCase) || name.EndsWith(".wmvy", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "/Assets/Icon/Video.png";
                }
                else if (name.EndsWith(".rar", StringComparison.CurrentCultureIgnoreCase) || name.EndsWith(".zipy", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "/Assets/Icon/Rar.png";
                }
                else if (name.EndsWith(".DIR", StringComparison.CurrentCultureIgnoreCase) || name.EndsWith(".thumbnails", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "/Assets/Icon/Tab.png";
                }
                else if (name.EndsWith(".mp3y", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "/Assets/Icon/Music1.png";
                }
                else if (name.EndsWith(".jpgy", StringComparison.CurrentCultureIgnoreCase) || name.EndsWith(".pngy", StringComparison.CurrentCultureIgnoreCase) || name.EndsWith(".bmpy", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "/Assets/Icon/Picture.png";
                }
                else
                {
                    return "/Assets/Icon/Blank.png";
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
