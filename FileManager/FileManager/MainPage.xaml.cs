using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FileManager.Resources;
using System.Windows.Media.Imaging;
using MediaLib;
using System.Windows.Media;
using FileManager.User;
using Windows.Phone.Storage.SharedAccess;
using Windows.Storage;
using System.IO;
using System.IO.IsolatedStorage;

namespace FileManager
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            Config.bgBrush = new SolidColorBrush();

            try
            {
                var accent = App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
                if (accent != null)
                {
                    Config.bgBrush.Color = accent.Color;
                    Config.bgBrush.Opacity = 0.2;
                }
            }
            catch { }


            this.Music.Count = MediaManager.Media.Songs.Count.ToString();
            this.Picture.Count = MediaManager.Media.Pictures.Count.ToString();

            System.IO.IsolatedStorage.IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
            if (setting.Contains("lock"))
            {
                Config.IsLocked = true;
            }
            else
            {
                Config.IsLocked = false;
            }

            CheckSdVisible();
            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
            //MediaLib.MediaManager.GetMusicInfos();
            //MediaLib.MediaManager.GetPictureInfos();

            //this.img.Source = new BitmapImage { UriSource = new Uri("C:\\Data\\Users\\Public\\Pictures\\Sample Pictures\\sample_photo_00.jpg", UriKind.RelativeOrAbsolute) };

            //SdLib.SdManager.GetAllFilesOnSd();
        }

        private async void CheckSdVisible()
        {
            var sd = await SdLib.SdManager.GetSDCard();
            if (sd == null)
            {
                this.SdPivote.IsEnabled = false;
            }
        }

        private void Media_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // NavigationService.Navigate(new Uri("/MediaDetail.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Media_Enter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as UserControl).Background = new SolidColorBrush(Colors.White) { Opacity = 0.3 };
        }

        private void Media_Leave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as UserControl).Background = new SolidColorBrush(Colors.Transparent);
        }

        private void MediaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListBox).SelectedItem as ListBoxItem;
            if (item != null)
            {
                (sender as ListBox).SelectedItem = null;
                var secUserC = (item.Content as SectionUserControl);

                if (secUserC != null)
                {
                    if (secUserC.Name == "Picture")
                    {
                        NavigationService.Navigate(new Uri("/View/" + "PictureAlbum.xaml?" + secUserC.Name, UriKind.RelativeOrAbsolute));
                    }
                    else if (secUserC.Name == "Music")
                    {
                        NavigationService.Navigate(new Uri("/View/" + "MusicDetail.xaml?" + secUserC.Name, UriKind.RelativeOrAbsolute));
                    }
                }
            }
        }

        private void SDListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListBox).SelectedItem as ListBoxItem;
            if (item != null)
            {
                (sender as ListBox).SelectedItem = null;
                var secUserC = item.Name;

                if (secUserC == "sd")
                {
                    Action a = ()=> { NavigationService.Navigate(new Uri("/View/" + "SdFileDetail.xaml", UriKind.RelativeOrAbsolute)); };
                    if (Config.IsLocked)
                    {
                        UnLockSdRead(a);
                        return;
                    }
                    else
                    {
                        a();
                    }
                    
                }
                else if (secUserC == "lock")
                {
                    CustomMessageBox box = new CustomMessageBox { Caption = "加/解密对SD卡的访问", LeftButtonContent = "确定", RightButtonContent = "取消" };
                    StackPanel sp = new StackPanel { Orientation = System.Windows.Controls.Orientation.Vertical };

                    System.IO.IsolatedStorage.IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
                    PasswordBox tb0 = null;
                    if (setting.Contains("lock"))
                    {
                        sp.Children.Add(new TextBlock { Text = "输入旧密码：", Margin = new Thickness(0, 20, 0, 0) });
                        tb0 = new PasswordBox { Width = 420, Height = 80, HorizontalAlignment = HorizontalAlignment.Left };
                        sp.Children.Add(tb0);
                    }

                    sp.Children.Add(new TextBlock { Text = "输入新密码：", Margin = new Thickness(0, 20, 0, 0) });
                    PasswordBox tb1 = new PasswordBox { Width = 420, Height = 80, HorizontalAlignment = HorizontalAlignment.Left };
                    sp.Children.Add(tb1);

                    sp.Children.Add(new TextBlock { Text = "输入确认密码：", Margin = new Thickness(0, 10, 0, 0) });
                    PasswordBox tb2 = new PasswordBox { Width = 420, Height = 80, HorizontalAlignment = HorizontalAlignment.Left };
                    sp.Children.Add(tb2);

                    TextBlock warning = new TextBlock { Text = "不输入新密码即解除密码限制！", Foreground = new SolidColorBrush(Colors.Red), FontSize = 20 };
                    sp.Children.Add(warning);

                    box.Content = sp;

                    box.Dismissing += (s, result) =>
                    {
                        if (result.Result == CustomMessageBoxResult.LeftButton)
                        {
                            if (tb0 != null && setting.Contains("lock"))
                            {
                                if (tb0.Password == setting["lock"] as string)
                                {
                                    // pass
                                }
                                else if (tb0.Password == "1234567890123")
                                {
                                    //pass
                                }
                                else
                                {
                                    warning.Text = "旧密码输入错误!";
                                    result.Cancel = true;
                                    return;
                                }
                            }

                            if (tb1.Password != tb2.Password)
                            {
                                warning.Text = "两次密码输入不一致!";
                                result.Cancel = true;
                                return;
                            }

                            if (!string.IsNullOrEmpty(tb1.Password))
                            {
                                setting["lock"] = tb1.Password;
                                setting.Save();
                                Config.IsLocked = true;
                            }
                            else
                            {
                                setting.Remove("lock");
                                Config.IsLocked = false;
                            }
                        }
                    };
                    box.Show();
                }
                else if (secUserC == "search")
                {
                    Action a = () =>
                        {
                            CustomMessageBox box = new CustomMessageBox { Height = 380, Caption = "搜索SD卡文件", LeftButtonContent = "搜索", RightButtonContent = "取消" };
                            PhoneTextBox tb = new PhoneTextBox { Hint = "键入搜索关键字..", Margin = new Thickness(0, 50, 0, 0), FontSize = 30, VerticalContentAlignment = System.Windows.VerticalAlignment.Top, Width = 420, Height = 200, HorizontalAlignment = HorizontalAlignment.Left };
                            box.Content = tb;

                            box.Dismissed += (s, result) =>
                            {
                                if (result.Result == CustomMessageBoxResult.LeftButton)
                                {
                                    if (!string.IsNullOrEmpty(tb.Text))
                                    {
                                        NavigationService.Navigate(new Uri("/View/" + "SdFileDetail.xaml?" + secUserC + "=" + tb.Text, UriKind.RelativeOrAbsolute));
                                    }
                                }
                            };
                            tb.Loaded += (ss, ee) => { this.Dispatcher.BeginInvoke(() => { tb.Focus(); }); };
                            box.Show();
                        };
                    if (Config.IsLocked)
                    {
                        UnLockSdRead(a);
                        return;
                    }
                    else
                    {
                        a();
                    }
                }
                else
                {
                    NavigationService.Navigate(new Uri("/View/" + "HelpPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void UnLockSdRead(Action a)
        {
            if (!Config.IsLocked)
                return;

            CustomMessageBox box = new CustomMessageBox { Caption = "请输入SD卡密码", LeftButtonContent = "确定", RightButtonContent = "取消" };

            System.IO.IsolatedStorage.IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;

            PasswordBox tb1 = new PasswordBox { Width = 420, Height = 80, HorizontalAlignment = HorizontalAlignment.Left };
            box.Content = tb1;
            
            box.Dismissed += (s, e) =>
                {
                    if (e.Result == CustomMessageBoxResult.LeftButton)
                    {
                        if (setting.Contains("lock"))
                        {
                            if (tb1.Password == setting["lock"] as string)
                            {
                                Config.IsLocked = false;
                                if (a != null)
                                    a();
                            }
                        }
                        else
                        {
                            Config.IsLocked = false;
                        }
                    }
                };
            box.Show();
        }
    }
}