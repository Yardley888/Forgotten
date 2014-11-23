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
    /// <summary>
    /// 科室
    /// </summary>
    public class Department
    {
        private string name_;
        private List<SectionInfo> sections_ = null;

        public string Name
        {
            get
            {
                return name_;
            }
            set
            {
                name_ = value;
            }
        }

        public List<SectionInfo> Sections
        {
            get
            {
                if (sections_ == null)
                {
                    sections_ = new List<SectionInfo>();
                }
                return sections_;
            }
            
        }
    }
    
    /// <summary>
    /// 门诊
    /// </summary>
    public class SectionInfo
    {
        public string Name{get;set;}

        public string Link{get;set;}
    }
}
