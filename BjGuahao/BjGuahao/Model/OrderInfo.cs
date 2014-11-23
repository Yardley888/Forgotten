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
    public class OrderInfo
    {
        public string DateTime_ { get; set; }
        public string OrderUrl_ { get; set; }

        public List<OrderDetail> Details_ {get;set;}

        public OrderInfo()
        {
            Details_ = new List<OrderDetail>();
        }
    }

    public class OrderDetail
    {
        public string DateTime_ { get; set; }
        public string Period { get; set; }
        public string Yard { get; set; }
        public string Section { get; set; }
        public string Doctor { get; set; }
        public string Post { get; set; }
        public string Cost { get; set; }
        public string Strong { get; set; }
        public string Total { get; set; }
        public string Surplus { get; set; }
        public string State { get; set; }

        public string OrderLink { get; set; }
        public bool CanOrder
        {
            get
            {
                return !string.IsNullOrEmpty(OrderLink);
            }
        }
    }
}
