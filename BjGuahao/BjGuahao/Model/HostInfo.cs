using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BjGuahao.Model
{
    public class HostInfo
    {
        public string Id { get; set; }

        public string HospitalName { get; set; }

        public string FileName { get; private set;}

        public string FolderName{get;set;}

        public string Path { get { return FolderName + "/" + FileName; } }

        public string Address { get; set; }

        public string Trafic { get; set; }

        public string WebUrl { get; set; }

        public string ImagePath { get { return FolderName + "/img/" + Id + ".jpg"; } }

        public HostInfo()
        {
            FileName = new Guid().ToString();
        }
    }
}
