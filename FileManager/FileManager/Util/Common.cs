using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace FileManager.Util
{
    public static class Common
    {
        public static T FindVisualChild<T>(DependencyObject element) where T : class
        {
            while (element != null)
            {
                if (element is T)
                    return element as T;

                element = VisualTreeHelper.GetChild(element, 0);
            }
            return null;
        }

        public static VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            IList groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
            {
                if (group.Name == name)
                    return group;
            }
            return null;
        }

        public static VisualStateGroup FindScrollStastes(DependencyObject parent)
        {
            var scroller = Common.FindVisualChild<ScrollViewer>(parent);
            if (scroller != null)
            {
                FrameworkElement element = VisualTreeHelper.GetChild(scroller, 0) as FrameworkElement;
                if (element != null)
                {
                    VisualStateGroup group = Common.FindVisualState(element, "ScrollStates");
                    return group;
                }
            }
            return null;
        }

        /// <summary>
        /// Delete all files and sub folders in the dirName folder.
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="myIsolatedStorage"></param>
        public static void DeleteDirectory(string dirName, IsolatedStorageFile myIsolatedStorage = null)
        {
            bool bIS_Init = false;
            if (myIsolatedStorage == null)
            {
                bIS_Init = true;
                myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            }

            Action<string> loopDeleteFolders = null;
            loopDeleteFolders = (folder) =>
            {
                foreach (var file in myIsolatedStorage.GetFileNames(folder + "/*"))
                {
                    myIsolatedStorage.DeleteFile(folder + "/" + file);
                }
                foreach (var subFolder in myIsolatedStorage.GetDirectoryNames(folder + "/*"))
                {
                    loopDeleteFolders(folder + "/" + subFolder);
                }
                myIsolatedStorage.DeleteDirectory(folder);
            };

            if (myIsolatedStorage.DirectoryExists(dirName))
            {
                loopDeleteFolders(dirName);
            }
            if (bIS_Init)
            {
                myIsolatedStorage.Dispose();
            }
        }

    }

    public class AssociationUriMapper : UriMapperBase
    {

        public override Uri MapUri(Uri uri)
        {
            string tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());

            //验证Uri，是否为文件关联引发
            if (tempUri.StartsWith("/FileTypeAssociation?fileToken="))
            {
                string fileToke = tempUri.Substring(31);
                //整理成规范URL，回发到MainPage.xaml
                return new Uri("/Host.xaml?fileToke=" + fileToke, UriKind.Relative);
            }
            return uri;
        }
    }
}
