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
using Microsoft.Xna.Framework;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Media;

namespace FileManager
{
    public partial class MusicDetail : PhoneApplicationPage
    {
        DispatcherTimer _checkTiemr = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
        SongBinding _showSong = new SongBinding { Timer = "" };

        public static bool bHasRegistorMediaplayerEvent = false;
        public MusicDetail()
        {
            InitializeComponent();
            this.ShowSongGrid.DataContext = _showSong;
            this.Loaded += MediaDetail_Loaded;
            if (!bHasRegistorMediaplayerEvent)
            {
                //MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
                MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
                bHasRegistorMediaplayerEvent = true;
            }
            _checkTiemr.Tick += SongStateChanged;
            //CheckMediaState();

            this.ShowSongGrid.Background = Config.bgBrush;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("OnNavigatedTo");
            base.OnNavigatedTo(e);
            CheckMediaState(true);
        }

        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            CheckMediaState();
        }

        void SongStateChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("SongStateChanged");
            if (MediaPlayer.State == MediaState.Playing)
            {
                try
                {
                    FrameworkDispatcher.Update();
                    if (_showSong.Song != null)
                    {
                        var total = _showSong.Song.Duration;
                        var reduce = MediaPlayer.PlayPosition;
                        var remain = total - reduce;
                        _showSong.Timer = string.Format("{0}:{1:00}", remain.Minutes, remain.Seconds);
                    }
                }
                catch
                { }
            }
            else
            {
                if (_checkTiemr.IsEnabled)
                    _checkTiemr.Stop();
                this.ShowTitle.Visibility = Visibility.Visible;
                this.ShowSongGrid.Visibility = Visibility.Collapsed;
                //this.DetailListBox.Tag = true;
                this.DetailListBox.SelectedValue = null;

            }
        }

        void CheckMediaState(bool bNavigateTo = false)
        {
            if (bNavigateTo)
            {
                FrameworkDispatcher.Update();
            }
            if (MediaPlayer.Queue != null)
            {
                var song = MediaPlayer.Queue.ActiveSong;
                if (song != null)
                {
                    _showSong.Name = song.Name;
                    _showSong.Song = song;
                    if (!_checkTiemr.IsEnabled)
                        _checkTiemr.Start();
                    this.ShowTitle.Visibility = Visibility.Collapsed;
                    this.ShowSongGrid.Visibility = Visibility.Visible;

                    return;
                }
            }

            if (_checkTiemr.IsEnabled)
                _checkTiemr.Stop();
            this.ShowTitle.Visibility = Visibility.Visible;
            this.ShowSongGrid.Visibility = Visibility.Collapsed;
        }

        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("MediaPlayer_MediaStateChanged");
            CheckMediaState();
        }

        void MediaDetail_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MediaDetail_Loaded");
            var allMusic = MediaLib.MediaManager.GetMusicInfos();
            this.DetailListBox.ItemsSource = allMusic;
            MarkTheShowSong();
        }

        private void MarkTheShowSong()
        {
            Debug.WriteLine("MarkTheShowSong");
            var allMusic = MediaLib.MediaManager.GetMusicInfos();
            if (_showSong.Song != null && MediaPlayer.State == MediaState.Playing)
            {
                var query = allMusic.FirstOrDefault(c => c.Song == _showSong.Song);
                if (query != null)
                {
                    //this.DetailListBox.SelectedItem = query;
                    this.DetailListBox.Tag = true;
                    this.DetailListBox.SelectedValue = query;

                }
            }
        }

        private void DetailListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("DetailListBox_SelectionChanged");
            var selectedMusic = (sender as ListBox).SelectedValue as MSong;
            if (selectedMusic == null)
                return;
            var tag = (sender as ListBox).Tag;
            if (tag != null && tag is bool)
            {
                (sender as ListBox).Tag = null;
                return;
            }

            //避免卡UI
            this.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        MediaPlayer.Stop();
                        FrameworkDispatcher.Update();
                        MediaPlayer.Play(selectedMusic.Song);
                    }
                    catch
                    { }
                    CheckMediaState();
                });
        }

        private void SongHolding(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var holdingSong = (sender as FrameworkElement).DataContext as MSong;
            if (holdingSong != null)
            {
                MediaManager.DeleteSong(holdingSong);
            }
        }

        private void ShowSongGrid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //FrameworkDispatcher.Update();
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
                this.DetailListBox.SelectedItem = null;
            }
        }

        public static OrderType nowType = OrderType.ABC;
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            var list = this.DetailListBox.ItemsSource as ObservableCollection<MSong>;

            if (list != null && list.Count > 1)
            {
                if (nowType == OrderType.ABC)
                {
                    var newList = list.OrderByDescending(c => c.PlayCount);
                    foreach (var s in newList)
                    {
                        list.Add(s);
                        list.RemoveAt(0);
                    }
                    MarkTheShowSong();
                    nowType = OrderType.Offen;
                }
                else if (nowType == OrderType.Offen)
                {
                    var newList = list.OrderBy(c => c.Singer);
                    foreach (var s in newList)
                    {
                        list.Add(s);
                        list.RemoveAt(0);
                    }
                    MarkTheShowSong();
                    nowType = OrderType.Singer;
                }
                else
                {
                    var newList = list.OrderBy(c => c.Name);
                    foreach (var s in newList)
                    {
                        list.Add(s);
                        list.RemoveAt(0);
                    }
                    MarkTheShowSong();
                    nowType = OrderType.ABC;
                }
                ShowOrderTypeBlock(nowType);
            }

        }

        DispatcherTimer lastTimer = null;
        private void ShowOrderTypeBlock(OrderType type)
        {
            string text = "";
            if (type == OrderType.ABC)
            {
                text = "曲目名称 ↑";
            }
            else if (type == OrderType.Offen)
            {
                text = "播放频率 ↓";
            }
            else
            {
                text = "艺术家 ↑";
            }
            this.ShowOrderType.Text = text;
            this.ShowOrderType.Visibility = System.Windows.Visibility.Visible;
            if (lastTimer != null && lastTimer.IsEnabled)
            {
                lastTimer.Stop();
                lastTimer = null;
            }
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += delegate
            {
                timer.Stop();
                this.ShowOrderType.Visibility = System.Windows.Visibility.Collapsed;
            };
            timer.Start();
            lastTimer = timer;
        }

        public enum OrderType
        {
            ABC = 0,
            Offen = 1,
            Singer = 2,
        }
    }

    public class SongBinding : INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Name"));
                }
            }
        }

        private string _timer;
        public string Timer
        {
            get
            {
                return _timer;
            }
            set
            {
                _timer = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Timer"));
                }
            }
        }

        public Song Song { get; set; }
    }
}