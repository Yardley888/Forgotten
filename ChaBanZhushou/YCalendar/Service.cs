using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace YCalendar
{
    class Service
    {
        public static string Jia = "甲班";
        public static string Yi = "乙班";
        public static string Bing = "丙班";

        private static string _className = Jia;
        public static string ClassName
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }

        private static DateTime JiaWhite = new DateTime(2013, 3, 16);
        private static DateTime YiWhite = new DateTime(2013, 3, 18);
        private static DateTime BingWhite = new DateTime(2013, 3, 17);

        public static Action QuitAction = null;

        public static DateTime WhiteFlag
        {
            get
            {
                if (ClassName == Jia)
                    return JiaWhite;
                else if (ClassName == Yi)
                    return YiWhite;
                else
                    return BingWhite;
            }
        }

        private static DateTime? GetNearWhiteDay()
        {
            DateTime date = DateTime.Now.Date;
            double plus = date.ToOADate() - WhiteFlag.ToOADate();

            int mod = (int)plus % 3;
            if (mod == 0)
            {
                return date;
            }
            else if (mod == 1)
            {
                return date.AddDays(2);
            }
            else if (mod == 2)
            {
                return date.AddDays(1);
            }
            else
            {
                return null;
            }
        }

        public static bool MakeAlarm(TimeSpan pickTime, Uri _uri)
        {

            var nearWhite = GetNearWhiteDay();

            if (pickTime != null && nearWhite != null)
            {
                DateTime firstWhite = nearWhite.Value;

                var nowTime = DateTime.Now.TimeOfDay;
                if (firstWhite.Date == DateTime.Now.Date && pickTime.CompareTo(nowTime) < 0)
                {
                    firstWhite = firstWhite.AddDays(3);
                }
                firstWhite = new DateTime(firstWhite.Year, firstWhite.Month, firstWhite.Day, pickTime.Hours, pickTime.Minutes, pickTime.Seconds);

                try
                {
                    TryToSetAlarm(firstWhite, _uri);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool ClearAlarms()
        {
            try
            {
                var alarmList = ScheduledActionService.GetActions<Alarm>();
                if (alarmList != null && alarmList.Count() > 0)
                {
                    foreach (var action in alarmList)
                    {
                        ScheduledActionService.Remove(action.Name);
                    }
                }
                var remindList = ScheduledActionService.GetActions<Reminder>();
                if (remindList != null && remindList.Count() > 0)
                {
                    foreach (var action in remindList)
                    {
                        ScheduledActionService.Remove(action.Name);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static void TryToSetAlarm(DateTime first, Uri _uri)
        {
            ClearAlarms();

            var temp = first;
            for (int i = 0; i < 48; i++)
            {
                string id = Guid.NewGuid().ToString();
                Alarm alarm = new Alarm(id);
                alarm.Content = "该起床了！";
                alarm.BeginTime = temp;
                alarm.ExpirationTime = alarm.BeginTime.AddHours(1);
                alarm.RecurrenceType = RecurrenceInterval.None;
                alarm.Sound = _uri;
                ScheduledActionService.Add(alarm);
                if (i >= 46)
                {
                    Reminder remind = new Reminder(Guid.NewGuid().ToString());
                    remind.BeginTime = temp.AddHours(3);
                    remind.ExpirationTime = remind.BeginTime.AddHours(1);
                    remind.RecurrenceType = RecurrenceInterval.None;
                    remind.Title = "提示";
                    remind.Content = "亲爱的：\n  你好久没进来了，请你现在务必进来一次，不然下次上班可能会迟到哦。\n【点我即可】";
                    remind.NavigationUri = new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute);
                    ScheduledActionService.Add(remind);
                }
                temp = temp.AddDays(3);

            }
        }
    }
}
