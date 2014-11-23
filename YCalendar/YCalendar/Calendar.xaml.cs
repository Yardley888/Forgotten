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
using System.ComponentModel;
using System.Windows.Data;
using System.Diagnostics;

namespace YCalendar
{
    public partial class Calendar : UserControl
    {
        private static double width = 480;
        private static double cell = width / 7;
        private double fontSize = 22;
        private SolidColorBrush forBrush = new SolidColorBrush(Colors.White);
        private SolidColorBrush secBrush = new SolidColorBrush(Colors.Gray);
        string[] months = new string[] { "", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月" ,""};

        private Grid FrontGrid = null;
        public Grid BackGrid
        {
            get
            {
                return FrontGrid == this.Content1 ? Content2 : Content1;
            }
        }

        private int pickYear_;
        public int PickYear
        {
            get { return pickYear_; }
            set
            {
                pickYear_ = value;
            }
        }
        private int pickMonth_;
        public int PickMonth
        {
            get { return pickMonth_; }
            set
            {
                pickMonth_ = value;
            }
        }

        public Calendar()
        {
            
            InitializeComponent();

            GestureListener listener = GestureService.GetGestureListener(this.LayoutRoot);
            listener.Flick += new EventHandler<FlickGestureEventArgs>(listener_Flick);

            var today = DateTime.Today;
            PickYear = today.Year;
            PickMonth = today.Month;

            string cnNowMonth = months[PickMonth];

            int daysCount = DateTime.DaysInMonth(PickYear,PickMonth);

            SetViewToContent(FrontGrid = Content1, PickYear, PickMonth);
            UpdateYearMonthTb(null);
            this.FlyUp.Completed += new EventHandler(story_Completed);
            this.FlyDown.Completed += new EventHandler(story_Completed);
            this.SingleYFly.Completed += new EventHandler(SingleFly_Completed);
            this.SingleMFly.Completed += new EventHandler(SingleFly_Completed);
        }

        void SingleFly_Completed(object sender, EventArgs e)
        {
            this.YeahTb.Text = PickYear.ToString();
            this.MonthTb.Text = months[PickMonth];
            (sender as Storyboard).Stop();
        }

        void story_Completed(object sender, EventArgs e)
        {
            this.BackGrid.Visibility = Visibility.Collapsed;
        }

        private void UpdateYearMonthTb(bool? isUp)
        {
            if (isUp == null)
            {
                this.YeahTb.Text = PickYear.ToString();
                this.MonthTb.Text = months[PickMonth];

                return;
            }

            if (this.YeahTb.Text != PickYear.ToString())
            {
                if (SingleYFly.GetCurrentState() == ClockState.Active)
                {
                    SingleFly_Completed(SingleYFly, null);
                }
                (SingleYFly.Children[1] as DoubleAnimationUsingKeyFrames).KeyFrames[1].Value = isUp.Value ? -YeahTb.ActualHeight : YeahTb.ActualHeight;
                SingleYFly.Begin();
            }

            if(this.MonthTb.Text != PickMonth.ToString())
            {
                if (SingleMFly.GetCurrentState() == ClockState.Active)
                {
                    SingleFly_Completed(SingleMFly, null);
                }
                (SingleMFly.Children[1] as DoubleAnimationUsingKeyFrames).KeyFrames[1].Value = isUp.Value ? -MonthTb.ActualHeight/1.5 : MonthTb.ActualHeight/1.5;
                SingleMFly.Begin();
            }

        }

        void listener_Flick(object sender, FlickGestureEventArgs e)
        {
            var direction = e.Direction;
            if (direction == Orientation.Horizontal)
            {
                return;
            }
            else
            {
                if (e.VerticalVelocity < 0)
                {
                    this.FlickUp();
                }
                else if(e.VerticalVelocity > 0)
                {
                    this.FlickDown();
                }
            }
        }

        private void FlickUp()
        {
            DateTime temp = new DateTime(PickYear, PickMonth, 1).AddMonths(1);
            this.SetViewToContent(BackGrid, PickYear = temp.Year, PickMonth=temp.Month);

            var upStory = this.FlyUp as Storyboard;
            MakeStory(upStory);
            UpdateYearMonthTb(true);
            this.FrontGrid = BackGrid;
            SetClipView();
        }

        private void MakeStory(Storyboard story)
        {
             story.Stop();
            (story.Children[0] as DoubleAnimationUsingKeyFrames).SetValue(Storyboard.TargetNameProperty, this.FrontGrid.Name);
            (story.Children[1] as DoubleAnimationUsingKeyFrames).SetValue(Storyboard.TargetNameProperty, this.FrontGrid.Name);
            (story.Children[2] as DoubleAnimationUsingKeyFrames).SetValue(Storyboard.TargetNameProperty, this.BackGrid.Name);
            (story.Children[3] as DoubleAnimationUsingKeyFrames).SetValue(Storyboard.TargetNameProperty, this.BackGrid.Name);

            (story.Children[0] as DoubleAnimationUsingKeyFrames).KeyFrames[1].Value = (story == this.FlyUp) ? -GerFrontHeight() : GerBackHeight();
            (story.Children[2] as DoubleAnimationUsingKeyFrames).KeyFrames[0].Value = (story == this.FlyUp) ? GerFrontHeight() : -GerBackHeight();

            this.BackGrid.Visibility = Visibility.Visible;
            story.Begin();
        }

        private double GerFrontHeight()
        {
            return FrontGrid.RowDefinitions.Sum(r => r.Height.Value);
        }
        private double GerBackHeight()
        {
            return  BackGrid.RowDefinitions.Sum(r => r.Height.Value);
        }

        private void SetClipView()
        {
            double height = FrontGrid.RowDefinitions.Sum(r => r.Height.Value);
            this.ClipGrid.Clip = new RectangleGeometry { Rect = new Rect { Width = 480, Height = height } };
        }

        private void FlickDown()
        {
            DateTime temp = new DateTime(PickYear, PickMonth, 1).AddMonths(-1);

            this.SetViewToContent(BackGrid, PickYear = temp.Year, PickMonth = temp.Month);

            var downStory = this.FlyDown as Storyboard;
            MakeStory(downStory);
            UpdateYearMonthTb(false);
            this.FrontGrid = BackGrid;
            SetClipView();
        }

        private void SetViewToContent(Grid content, int year,int month)
        {
            var temp = new DateTime(year, month, 1);
            int daysCount = DateTime.DaysInMonth(year, month);

            content.Children.Clear();

            int row = 1;
            for (int i = 1; i <= daysCount; i++)
            {
                int hash = temp.DayOfWeek.GetHashCode();
                int x = hash == 0 ? 7 : hash;

                if (i == 1 && x > 1)
                {
                    var lastDay = temp.AddDays(-1);

                    for (int t = x-1; t >= 1; t--)
                    {
                        TextBlock tb1 = new TextBlock { Margin = new Thickness(2,0,0,0), FontSize = fontSize, Foreground = secBrush, Text = lastDay.Day.ToString(), VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Left };
                        tb1.Tag = temp;
                        tb1.SetValue(Grid.RowProperty, row);
                        tb1.SetValue(Grid.ColumnProperty, t - 1);
                        content.Children.Add(tb1);
                        lastDay = lastDay.AddDays(-1);
                    }
                }
                else if (i == daysCount && x < 7)
                {
                    var nextDay = temp.AddDays(1);

                    for (int t = x + 1; t <= 7; t++)
                    {
                        TextBlock tb2 = new TextBlock { Margin = new Thickness(2, 0, 0, 0), FontSize = fontSize, Foreground = secBrush, Text = nextDay.Day.ToString(), VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Left };
                        tb2.Tag = temp;
                        tb2.SetValue(Grid.RowProperty, row);
                        tb2.SetValue(Grid.ColumnProperty, t - 1);
                        content.Children.Add(tb2);
                        nextDay = nextDay.AddDays(1);
                    }
                    
                }

                TextBlock tb = new TextBlock { Margin = new Thickness(2, 0, 0, 0), FontSize = fontSize, Foreground = forBrush, Text = i.ToString(), VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Left };
                tb.Tag = temp;
                tb.SetValue(Grid.RowProperty, row);
                tb.SetValue(Grid.ColumnProperty, x - 1);
                content.Children.Add(tb);

                if (temp == DateTime.Today)
                {
                    tb.Foreground = new SolidColorBrush(Colors.Red);
                }

                if (x == 7 && i < daysCount) row++;
                temp = temp.AddDays(1);
            }

            if (row == 4)
            {
                this.BgGrid.RowDefinitions[5].Height = this.BgGrid.RowDefinitions[6].Height = new GridLength(0);
                content.RowDefinitions[5].Height = content.RowDefinitions[6].Height = new GridLength(0);
            }
            else if (row == 5)
            {
                this.BgGrid.RowDefinitions[5].Height = new GridLength(cell);
                this.BgGrid.RowDefinitions[6].Height = new GridLength(0);
                content.RowDefinitions[5].Height = new GridLength(cell);
                content.RowDefinitions[6].Height = new GridLength(0);
            
            }
            else if (row == 6)
            {
                this.BgGrid.RowDefinitions[5].Height = this.BgGrid.RowDefinitions[6].Height = new GridLength(cell);
                content.RowDefinitions[5].Height = content.RowDefinitions[6].Height = new GridLength(cell);
            }
        }

    }
}
