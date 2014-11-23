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
using System.IO;
using System.Diagnostics;

namespace DataLib
{
    public static class IsoFile
    {
        public static bool IsFileExist(string fileName, string folder)
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return file.FileExists(folder + "/" + fileName);
            }
        }

        public static bool IsFolderExist(string folder)
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return file.DirectoryExists(folder);
            }
        }

        public static bool CreateFile(string fileNmae, string folder , string content)
        {
            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!file.DirectoryExists(folder))
                    {
                        file.CreateDirectory(folder);
                    }

                    using (IsolatedStorageFileStream stream = file.CreateFile(folder + "/" + fileNmae))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(content);
                        }
                    }
                }

                return true;
            }
            catch(Exception e)
            {
                Debug.WriteLine("Create file error---" + e.Message);
                return false;
            }
        }

        public static string ReadFile(string fileName, string folder)
        {
            string result = string.Empty;

            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = file.OpenFile(folder + "/" + fileName, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine("Read file error---" + e.Message);
            }

            return result ;
        }

        public static bool DeleteFile(string fileName, string folder)
        {
            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    file.DeleteFile(folder + "/" + fileName);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Delete file error---" + e.Message);

                return false;
            }
            return true;
        }

        public static bool DeleteFolder(string folder)
        {
            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    file.DeleteDirectory(folder);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Delete folder error---" + e.Message);

                return false;
            }

            return true;
        }

        #region Read Resource File

        public static string ReadResourceContent(string fileName)
        {
            string result = string.Empty;
            string path = "Resource/" + fileName;

            try
            {
                var info = Application.GetResourceStream(new Uri(path, UriKind.RelativeOrAbsolute));
                if (info != null)
                {
                    using (Stream stream = info.Stream)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ReadResourceContent error---" + e.Message);   
            }

            return result;
        }

        #endregion

    }
}
