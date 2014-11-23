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
using System.IO.IsolatedStorage;

namespace DataLib
{
    public static  class IsoSetting
    {
        public static IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;

        public static void Add(string key , object value)
        {
            if (setting.Contains(key))
            {
                setting.Remove(key);
            }
            setting.Add(key, value);
        }

        public static void Remove(string key)
        {
            setting.Remove(key);
        }

        public static void Clear()
        {
            setting.Clear();
        }

        public static bool Contains(string key)
        {
            return setting.Contains(key);
        }
    }
}
