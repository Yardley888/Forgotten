using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace FileManager.User
{
    public partial class SectionUserControl : UserControl
    {
        public SectionUserControl()
        {
            InitializeComponent();
            this.LayoutRoot.SetBinding(Grid.BackgroundProperty, new System.Windows.Data.Binding { Source = this, Path = new PropertyPath("Background") });
        }

        public string Title
        {
            get
            {
                return this.tTitle.Text;
            }
            set
            {
                this.tTitle.Text = value;
            }
        }
        public string Count
        {
            get
            {
                return this.tCount.Text;
            }
            set
            {
                this.tCount.Text = value;
            }
        }
        public string Capcity
        {
            get
            {
                return this.tCapity.Text;
            }
            set
            {
                this.tCapity.Text = value;
            }
        }
        public string Unit
        {
            get
            {
                return this.tUnit.Text;
            }
            set
            {
                this.tUnit.Text = value;
            }
        }
        public string Url
        {
            get
            {
                return this.Img.Source.ToString();
            }
            set
            {
                BitmapImage bi = new BitmapImage{ UriSource = new Uri(value,UriKind.RelativeOrAbsolute)};
                this.Img.Source = bi;
            }
        }
    }
}
