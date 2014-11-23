using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using MediaLib;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using FileManager.Util;
using FileManager.View;
using System.Windows.Controls.Primitives;
using FileManager.User;

namespace FileManager
{
    public partial class PictureDetail : PhoneApplicationPage
    {
        public PictureDetail()
        {
            InitializeComponent();
            this.Dispatcher.BeginInvoke(() =>
                {
                    if (Config.CurrentChooseAlbum != null)
                    {
                        this.ShowTitle.Text = "图片>相册>" + Config.CurrentChooseAlbum.Name;
                        var list = MediaLib.MediaManager.GetPicsFromAlbum(Config.CurrentChooseAlbum);

                        this.DetailListBox.ItemsSource = list;
                    }
                });
            this.showTitleGrid.Background = Config.bgBrush;
        }



        private void Lb_ItemUnrealized(object sender, ItemRealizationEventArgs e)
        {
            var img = Common.FindVisualChild<Image>(e.Container);
            if (img == null)
                return;

            var bi = img.Source as BitmapImage;
            if (bi != null)
            {
                bi.UriSource = null;
                bi = null;
            }
            img.Source = null;
            img = null;
        }

        private void Lb_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            Debug.WriteLine(Microsoft.Phone.Info.DeviceStatus.ApplicationCurrentMemoryUsage.ToString());
            var mPic = e.Container.DataContext as MPicture;
            var img = Common.FindVisualChild<Image>(e.Container);
            if (mPic == null || img == null)
                return;

            var p = mPic.Picture;
            try
            {
                BitmapImage bi = new BitmapImage();
                bi.CreateOptions = BitmapCreateOptions.DelayCreation;
                using (var stream = p.GetThumbnail())
                {
                    bi.SetSource(stream);
                    stream.Dispose();
                    stream.Close();
                    img.Source = bi;
                }
                if (!mPic.bSetLength())
                {
                    this.Dispatcher.BeginInvoke(() =>
                     {
                         using (var stream = p.GetImage())
                         {
                             var len = stream.Length;
                             if (mPic != null)
                             {
                                 mPic.CheckAndSetLenthString(len);
                             }
                         }

                     });
                }
            }
            catch
            {
                Debug.WriteLine("Exception");
            }
        }

        
        private void DetailListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as LongListSelector;
            var item = listBox.SelectedItem as MPicture;
            if (item == null) return;
            listBox.SelectedItem = null;
            Config.CurrentChoosePicture = item;
            
            ShowPicture(item);
        }

        bool isShowing = false;
        private void ShowPicture(MPicture p)
        {
            this.Sb_list_slide_left.Begin();
            isShowing = true;

            this.Dispatcher.BeginInvoke(() =>
                {
                    this.ShowTitle.Text = "图片>相册>" + Config.CurrentChooseAlbum.Name + " >" + p.Name;
                    this.ImgView.SetPic(p.Picture);
                    this.showTitleGrid.Visibility = Visibility.Visible;
                });
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (isShowing)
            {
                isShowing = false;
                this.ShowTitle.Text = "图片>相册>" + Config.CurrentChooseAlbum.Name;
                this.ImgView.SetPic(null);
                this.Sb_list_slide_right.Begin();
                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

        private void ShowImgGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.showTitleGrid.Visibility == Visibility.Visible)
            {
                this.showTitleGrid.Visibility = Visibility.Collapsed;
            }
        }

    }
}
