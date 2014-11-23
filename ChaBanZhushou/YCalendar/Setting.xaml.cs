using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace YCalendar
{
    public partial class Setting : PhoneApplicationPage
    {
        private Song _thePickedSong = null;


        public Setting()
        {
            InitializeComponent();
            this.songListPicker.ItemsSource = GetSongList();
            UpdateState();
        }

        void UpdateState()
        {
            IsolatedStorageSettings s = IsolatedStorageSettings.ApplicationSettings;
            if (s.Contains("state"))
            {
                string state = s["state"] as string;
                if (state == "on")
                {
                    this.alarmSwitch.IsChecked = true;

                }
                else if (state == "off")
                {
                    this.alarmSwitch.IsChecked = false;
                }
            }
            else
            {
                this.alarmSwitch.IsChecked = false;
            }

            if (s.Contains("time"))
            {
                var time = s["time"] as DateTime?;
                if (time != null)
                {
                    this.timePicker.Value = time;
                }
            }
            if (s.Contains("song"))
            {
                var song = s["song"] as Song;
                if (song != null)
                {
                    var list = this.songListPicker.ItemsSource as List<Song>;
                    var p = list.FirstOrDefault(x => x.Uri == song.Uri);
                    if (p != null)
                    {
                        this.songListPicker.SelectedItem = p;
                    }
                }
            }

            this.ClassPicker.SelectedItem = this.ClassPicker.Items.SingleOrDefault(x => (x as ListPickerItem).Content.ToString() == Service.ClassName);
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            (sender as ToggleSwitch).Content = "开";
            this.timePicker.IsEnabled = true;
            this.songListPicker.IsEnabled = true;
            this.btnCon.IsEnabled = true;

        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            (sender as ToggleSwitch).Content = "关";
            this.timePicker.IsEnabled = false;
            this.songListPicker.IsEnabled = false;
            this.btnCon.IsEnabled = false;
            //this.p.Visibility = this.s.Visibility = Visibility.Collapsed;
        }

        private bool ReStartAlarms(TimeSpan timeValue)
        {
            if (timeValue != null && _thePickedSong != null)
            {
                //var pickTime = this.timePicker.Value.Value.TimeOfDay;

                return Service.MakeAlarm(timeValue, _thePickedSong.Uri);
            }
            else
            {
                return false;
            }
        }

        private List<Song> GetSongList()
        {
            List<Song> songs = new List<Song>();
            for (int i = 1; i <= 22; i++)
            {
                string r = string.Format("{0:D2}", i);
                string url = "Alarm-" + r + ".wma";
                if (i == 5 || i == 6)
                    url = "Alarm-" + r + ".mp3";
                Uri uri = new Uri("Songs/" + url, UriKind.RelativeOrAbsolute);
                var info = Application.GetResourceStream(uri);
                if (info != null)
                {
                    Song _song = new Song { SongName = "铃音" + i, Uri = uri };
                    songs.Add(_song);
                }
            }
            return songs;
        }

        private void Song_Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (this.SongPlayer.CurrentState != MediaElementState.Playing)
            {
                this.SongPlayer.Play();
            }
            else
            {
                this.SongPlayer.Stop();
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.SongPlayer.Source == null && _thePickedSong != null)
            {
                this.SongPlayer.Source = _thePickedSong.Uri;
            }
        }

        private void SongPick_Changed(object sender, SelectionChangedEventArgs e)
        {

            var picker = sender as ListPicker;
            var song = picker.SelectedItem as Song;
            if (song != null)
            {
                this.SongPlayer.Source = song.Uri;
                _thePickedSong = song;
            }
        }

        private void PalyerStateChanged(object sender, RoutedEventArgs e)
        {
            if (this.SongPlayer.CurrentState == MediaElementState.Playing)
            {
                //this.SongPlayer.Play();
                this.p.Visibility = Visibility.Collapsed;
                this.s.Visibility = Visibility.Visible;
                this.btnCon.Content = "停止";
            }
            else
            {
                //this.SongPlayer.Stop();
                this.s.Visibility = Visibility.Collapsed;
                this.p.Visibility = Visibility.Visible;
                this.btnCon.Content = "试听";
            }
        }

        //private void SaveButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //this.progressBar.IsIndeterminate = true;
        //    //this.TopGrid.Visibility = Visibility;

        //    if (this.alarmSwitch.IsChecked.Value)
        //    {
        //        ReStartAlarms();
        //    }
        //    else
        //    {
        //        Service.ClearAlarms();
        //    }

        //    NavigationService.GoBack();
        //    //this.TopGrid.Visibility = System.Windows.Visibility.Collapsed;
        //    //this.progressBar.IsIndeterminate = false;

        //    //ShowToast();
        //}

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            //this.waitTb.Visibility = System.Windows.Visibility.Visible;
            //this.TopGrid.Visibility = Visibility;

            SaveSetting();
        }

        private void SaveSetting()
        {
            bool checkValue = this.alarmSwitch.IsChecked != null ? this.alarmSwitch.IsChecked.Value : false;
            var p = this.timePicker.Value.Value.TimeOfDay;
            var _className = (this.ClassPicker.SelectedItem as ListPickerItem).Content.ToString();
            Service.ClassName = _className;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings["class"] = Service.ClassName;
            settings["song"] = _thePickedSong;
            var n = DateTime.Now;
            settings["time"] = new DateTime(n.Year, n.Month, n.Day, p.Hours, p.Minutes, p.Milliseconds);


            if (checkValue)
            {
                settings["state"] = "on";
            }
            else
            {
                settings["state"] = "off";
            }

            settings.Save();

            Action QuitACtion = delegate
            {

                if (checkValue)
                {
                    bool r = Service.MakeAlarm(p, _thePickedSong.Uri);
                    if (r)
                    {
                        settings["state"] = "on";
                    }
                    else
                    {
                        settings["state"] = "off";
                    }
                    settings.Save();
                }
                else
                {
                    bool r = Service.ClearAlarms();
                    if (r)
                    {
                        settings["state"] = "off";
                        settings.Save();
                    }
                }
            };

            Service.QuitAction = QuitACtion;
        }

        //private void SaveButton_Click(object sender, EventArgs e)
        //{
        //    return;

        //    var savaBar = sender as ApplicationBarIconButton;
        //    savaBar.IsEnabled = false;
        //    this.progressBar.IsIndeterminate = true;
        //    this.waitTb.Visibility = System.Windows.Visibility.Visible;
        //    this.TopGrid.Visibility = Visibility;


        //    bool checkValue = this.alarmSwitch.IsChecked != null ? this.alarmSwitch.IsChecked.Value : false;
        //    var p = this.timePicker.Value.Value.TimeOfDay;
        //    var _className = (this.ClassPicker.SelectedItem as ListPickerItem).Content.ToString();
        //    Service.ClassName = _className;

        //    new Thread(
        //        delegate()
        //        {
        //            bool result = false;
        //            if (checkValue)
        //            {
        //                bool r = ReStartAlarms(p);
        //                if (r)
        //                {
        //                    IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        //                    settings["class"] = Service.ClassName;
        //                    settings["state"] = "on";
        //                    var n = DateTime.Now;
        //                    settings["time"] = new DateTime(n.Year, n.Month, n.Day, p.Hours, p.Minutes, p.Milliseconds);
        //                    settings["song"] = _thePickedSong;
        //                    settings.Save();
        //                    result = true;
        //                }
        //            }
        //            else
        //            {
        //                bool r = Service.ClearAlarms();
        //                if (r)
        //                {
        //                    IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        //                    settings["state"] = "off";
        //                    settings.Save();
        //                    result = true;
        //                }
        //            }

        //            this.Dispatcher.BeginInvoke(() =>
        //                {
        //                    this.TopGrid.Visibility = System.Windows.Visibility.Collapsed;
        //                    this.progressBar.IsIndeterminate = false;
        //                    this.waitTb.Visibility = System.Windows.Visibility.Collapsed;
        //                    savaBar.IsEnabled = true;
        //                    if (result)
        //                        NavigationService.GoBack();
        //                    else
        //                        MessageBox.Show("Sorry,Save error.");
        //                });
        //        }).Start();
        //}

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Tasks.EmailComposeTask mail = new Microsoft.Phone.Tasks.EmailComposeTask();
            mail.To = "huspace999@gmail.com";
            var phoneName = Microsoft.Phone.Info.DeviceStatus.DeviceName;
            mail.Subject = "来自北京地铁员工[" + phoneName + "]";

            mail.Show();
        }


    }
}