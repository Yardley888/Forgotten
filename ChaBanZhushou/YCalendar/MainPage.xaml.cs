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
using System.Windows.Threading;
using Microsoft.Phone.Scheduler;
using System.IO.IsolatedStorage;
using System.Diagnostics;

namespace YCalendar
{
    public partial class MainPage : PhoneApplicationPage
    {

        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            UpdateAlarmState();
        }

        private void UpdateAlarmState()
        {
            IsolatedStorageSettings s = IsolatedStorageSettings.ApplicationSettings;
            if (s.Contains("state"))
            {
                string state = s["state"] as string;
                if (state == "on")
                {
                    if (s.Contains("time") && s.Contains("song"))
                    {
                        var time = s["time"] as DateTime?;
                        var song = s["song"] as Song;
                        if (time != null && song != null)
                        {
                            double plus = DateTime.Now.ToOADate() - time.Value.ToOADate();
                            if (Math.Abs((int)plus) > 30)
                            {
                                var r = Service.MakeAlarm(time.Value.TimeOfDay, song.Uri);
                                if (r)
                                {
                                    var n = DateTime.Now;
                                    var p = time.Value.TimeOfDay;
                                    s["time"] = new DateTime(n.Year, n.Month, n.Day, p.Hours, p.Minutes, p.Milliseconds);
                                    s.Save();
                                }
                            }
                        }
                    }

                }
                else if (state == "off")
                {
                    return;
                }
            }
            else
            {
                return;
            }


        }

        private void RegularPolygon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LayoutRoot.Background = new SolidColorBrush(GetRandomColor());
        }

        private Color GetRandomColor()
        {
            int c1 = new Random().Next(256);
            int c2 = new Random().Next(256);
            int c3 = new Random().Next(256);

            Color color = Color.FromArgb(255, (byte)c1, (byte)c2, (byte)c3);

            return color;
        }

        private void SettingButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Setting.xaml", UriKind.RelativeOrAbsolute));

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //if(NavigationContext.QueryString.Count > 0)
            //{
            //    if (NavigationContext.QueryString.ContainsKey("r"))
            //    {
            //        if (NavigationContext.QueryString["r"] == "ok")
            //        {
            //            //ShowToast();
            //        }
            //    }
            //}
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("class"))
            {
                Service.ClassName = settings["class"] as string;
                this.ClassNameTitle.Text = Service.ClassName;

                this.calendarContent.Children.Clear();
                this.calendarContent.Children.Add(new Calendar() { Width = 480, Height = 600 });
            }
            else
            {
                CustomMessageBox custom = new CustomMessageBox()
                {
                    Title = "欢迎使用",
                    FontSize = 30,
                    RightButtonContent = "开始"
                };

                StackPanel sp = new StackPanel { Orientation = System.Windows.Controls.Orientation.Vertical };
                sp.Children.Add(new TextBlock { Margin = new Thickness(0,100,0,0), Height = 100, FontSize = 25, TextWrapping = System.Windows.TextWrapping.Wrap, Text = "   本应用仅适用于北京地铁员工，献给奋斗在地铁一线的人们！" });

                ListPicker lp = new ListPicker { Height = 200, Margin = new Thickness(0,100,0,0) ,Header = "请选择您所在地铁班次", FontSize = 25 };
                lp.Items.Add(new ListPickerItem { Content = Service.Jia });
                lp.Items.Add(new ListPickerItem { Content = Service.Yi });
                lp.Items.Add(new ListPickerItem { Content = Service.Bing });
                sp.Children.Add(lp);
                custom.Content = sp;
                custom.Dismissed += (s, ee) =>
                {
                    Service.ClassName = (lp.SelectedItem as ListPickerItem).Content.ToString();
                    settings["class"] = Service.ClassName;
                    settings.Save();
                    this.calendarContent.Children.Clear();
                    this.calendarContent.Children.Add(new Calendar() { Width = 480, Height = 600 });
                    this.ClassNameTitle.Text = Service.ClassName;
                };
                custom.Show();
            }

            //System.IO.IsolatedStorage.IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("state"))
            {
                var state = settings["state"] as string;
                if (state == "on")
                {
                    this.Tb_alarm_result.Text = "已开启";
                }
                else if (state == "off")
                {
                    this.Tb_alarm_result.Text = "已关闭";
                }
                else
                {
                    Debug.WriteLine("state-----wrong");
                }
            }
        }
    }
}