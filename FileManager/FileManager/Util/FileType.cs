
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Util
{
    public static class FileType
    {
        public static string GetRealTypeName(string name)
        {
            string realName = name;

            var index = name.LastIndexOf('.');
            if (index != -1)
            {
                var firstName = name.Substring(0, index);
                var lastType = name.Substring(index + 1);
                var realType = string.Empty;
                switch (lastType)
                {
                    case "txty":
                    case "wavy":
                    case "mp3y":
                    case "mp4y":
                    case "3gpy":
                    case "wmvy":
                    case "gify":
                    case "aviy":
                    case "xlsy":
                    case "xlsxy":
                    case "docy":
                    case "docxy":
                    case "ppty":
                    case "pptxy":
                    case "jpgy":
                    case "pngy":
                    case "jpegy":
                    case "bmpy":
                    case "picy":
                    case "zipy":
                        realType = lastType.Remove(lastType.Length - 1, 1);
                        break;
                    default :
                        realType = lastType;
                        break;
                }
                return string.Format("{0}.{1}", firstName, realType);
            }

            return realName;
        }
    }


}
