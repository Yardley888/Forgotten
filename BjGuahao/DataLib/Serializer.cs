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
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;

namespace DataLib
{
    public static class Serializer
    {
        public static bool ToXml(object sender , string fileName , string folder = null)
        {
            Type type = sender.GetType();

            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (Stream stream = file.CreateFile(folder + "/" + fileName))
                    {
                        XmlSerializer xml = new XmlSerializer(type);
                        xml.Serialize(stream, sender);
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine("serializer to xml error---"+ e.Message);
                return false;
            }

            return true;
        }

        public static object ToObject(Type type ,string fileName, string folder = null)
        {
            object o = null;

            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (Stream stream = file.OpenFile(folder + "/" + fileName, FileMode.Open))
                    {
                        XmlSerializer xml = new XmlSerializer(type);
                        o = xml.Deserialize(stream);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("serializer to object error---" + e.Message);
                return null;
            }

            return o;
            
        }
    }
}
