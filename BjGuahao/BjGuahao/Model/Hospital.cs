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
using System.Collections.Generic;

namespace BjGuahao.Model
{
    public class Hospital
    {
        public string Hpid { get; set; }
        
        public string Name { get; set; }

        public string Level { get; set; }

        public string Link { get; set; }

        public string Address { get; set; }

        public string District { get; set; }

        public List<Department> Departs { get; set; }
    }
}
