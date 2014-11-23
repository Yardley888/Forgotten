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
using System.Net.NetworkInformation;
using System.Text;
using System.IO.IsolatedStorage;

namespace BjGuahao.Service
{
    public static class Common
    {
        public static bool CheckNetAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
            
        }

        public static string ConvertExtendedAscii(string html)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in html.ToString())
            {
                int charInt = Convert.ToInt32(c);
                if (charInt > 127)
                    sb.AppendFormat("&#{0};", charInt);
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        #region Cookie
                
        public static void AddCookie(string key ,string value)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("Cookie"))
            {
                var cContainer = settings["Cookie"] as CookieContainer;
                cContainer.Add(new Uri(Config.ServerUrl), new Cookie(key, value));
                settings.Save();
            }
            else
            {
                CookieContainer cContainer = new CookieContainer();
                cContainer.Add(new Uri(Config.ServerUrl), new Cookie(key, value));
                settings.Add("Cookie", cContainer);
                settings.Save();
            }
        }

        public static CookieContainer GetCookie()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("Cookie"))
            {
                var cContainer = settings["Cookie"] as CookieContainer;
                return cContainer;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Find Control

        public static Page FindHostPage()
        {
            var RootVisualFrame = Application.Current.RootVisual as Frame;
            if (RootVisualFrame != null)
            {
                var hostPage = FindVisualChild<Page>(RootVisualFrame);
                return hostPage;
            }
            return null;
        }

        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T t = default(T);

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    t = child as T;
                    break;
                }
                else
                {
                    var t1 = FindVisualChild<T>(child);
                    if (t1 != null)
                    {
                        t = t1;
                        break;
                    }
                }
            }

            return t;
        }

        private static Grid hostGird;
        public static Grid HostGird
        {
            get
            {
                if (hostGird == null)
                {
                    hostGird = FindVisualChild<Grid>(HostPage);
                }

                return hostGird;
            }
        }

        private static Page hostPage;
        public static Page HostPage
        {
            get
            {
                if (hostPage == null)
                {
                    hostPage = FindHostPage();
                }

                return hostPage;
            }
        }

        #endregion


    }
}
