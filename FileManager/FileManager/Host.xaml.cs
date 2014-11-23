using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Storage.SharedAccess;
using Windows.Storage;

namespace FileManager
{
    public partial class Host : PhoneApplicationPage
    {
        public Host()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //获取URI参数
            IDictionary<string, string> queryStrings = NavigationContext.QueryString;

            if (queryStrings.ContainsKey("fileToke"))
            {
                var fileToke = queryStrings["fileToke"];
                queryStrings.Remove("fileToke");

                ////获取文件名
                var fileName = SharedStorageAccessManager.GetSharedFileName(fileToke);
                var index = fileName.LastIndexOf('.');
                if (index != -1)
                {
                    var type = fileName.Remove(0, index + 1);

                    ////本地文件夹
                    //var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    ////将文件拷贝到本地文件夹同名文件中
                    //var file = await SharedStorageAccessManager.CopySharedFileAsync(localFolder, fileName.TrimEnd('y'), NameCollisionOption.ReplaceExisting, fileToke);
                    //var stream = await file.OpenAsync(FileAccessMode.Read);
                    //await Windows.System.Launcher.LaunchFileAsync(file);
                    ////StreamReader s = new StreamReader(stream.AsStream(), System.Text.Encoding.Unicode);
                    ////var fileText = s.ReadToEnd();
                    ////MessageBox.Show("文件名：" + fileName + "\n内容：" + fileText);
                    var result = MessageBox.Show("无法打开此类型文件，立即下载相关软件?", "文件打开", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("zune:search?keyword=" + type + "&contenttype=app"));
                    }
                }
            }
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                App.Current.Terminate();
            }
        }
    }
}