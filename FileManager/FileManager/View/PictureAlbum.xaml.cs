using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;

namespace FileManager.View
{
    public partial class PictureAlbum : PhoneApplicationPage
    {
        public PictureAlbum()
        {
            InitializeComponent();
            this.Loaded += PictureAlbum_Loaded;
            this.showTittleGrid.Background = Config.bgBrush;
        }

        void PictureAlbum_Loaded(object sender, RoutedEventArgs e)
        {
            var albumsList = MediaLib.MediaManager.GetPicAlbums();
            this.AlbumListBox.ItemsSource = albumsList;
        }

        private void BottomGrid_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as Grid).Background = Config.bgBrush;
        }

        private void AlbumListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedItem == null) 
                return;
            var album = lb.SelectedItem as MediaLib.MpictureAlbum;
            if (album.Count > 0)
            {
                Config.CurrentChooseAlbum = album;
                NavigationService.Navigate(new Uri("/View/PictureDetail.xaml",UriKind.RelativeOrAbsolute));
            }

            lb.SelectedItem = null;

        }

        private void Img_Loaded(object sender, RoutedEventArgs e)
        {
            
            var p = (sender as Image).DataContext as Picture;
            if (p != null && (sender as Image).Source == null)
            {
                using (var s = p.GetThumbnail())
                {
                    try
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.SetSource(s);
                        (sender as Image).Source = bi;
                    }
                    catch { }
                }
            }
        }
    }
}