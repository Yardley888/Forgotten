using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SdLib;
using Microsoft.Phone.Storage;
using System.Windows.Data;
using Windows.Storage;
using Windows.System;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FileManager.Util;

namespace FileManager
{
    public partial class SdFileDetail : PhoneApplicationPage
    {
        private List<Tuple<List<SdFModel>, string>> SdFolderStack = new List<Tuple<List<SdFModel>, string>>();
        
        public SdFileDetail()
        {
            InitializeComponent();
            this.Dispatcher.BeginInvoke(new Action(FirstViewAction));
            this.ShowBg.Background = Config.bgBrush;

            this.BackKeyPress += SdFileDetail_BackKeyPress;
        }

        void SdFileDetail_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private async void FirstViewAction()
        {
            var sd = await SdManager.GetSDCard();
            if (sd != null)
            {
                if (NavigationContext.QueryString.ContainsKey("search"))
                {
                    this.ApplicationBar.IsVisible = false;
                    var query = NavigationContext.QueryString["search"] as string;
                    var sdfList = await SdManager.GetAllFilesOnSd();
                    var resultList = sdfList.Where(f => f.Name.Contains(query)).ToList();
                    this.SdFolderViewList.ItemsSource = resultList;
                    this.Tb_Path.Text = "SdCard";
                }
                else
                {
                    var fs = await SdManager.SerachFoldersAndFiles(sd.RootFolder);
                    var sdfList = SdManager.GetSdFModelsFromSdFolder(fs);

                    this.SdFolderViewList.ItemsSource = sdfList;
                    this.Tb_Path.Text = "SdCard";
                    this.SdFolderStack.Add(new Tuple<List<SdFModel>, string>(sdfList, this.Tb_Path.Text));
                }
            }
        }
        
        private async void SdFolderViewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = this.SdFolderViewList.SelectedItem;
            if (item == null)
            {
                return;
            }
            if (item is SdFolder)
            {
                this.Tb_Path.Text = "SdCard\\" + (item as SdFolder).Path;
                this.Sb_Content_SliderLeft1.Begin();
                var fs = await SdManager.SerachFoldersAndFiles(item as SdFolder);
                var sdfList = SdManager.GetSdFModelsFromSdFolder(fs);

                this.SdFolderViewList.ItemsSource = sdfList;
                this.Sb_Content_SliderLeft2.Begin();

                this.SdFolderStack.Add(new Tuple<List<SdFModel>, string>(sdfList, this.Tb_Path.Text));
            }
            else if (item is SdFile)
            {
                ProgressTopBar.IsIndeterminate = true;
                var file = (item as SdFile).Storage;
                var sdStream = await file.OpenForReadAsync();
                try
                {
                    using (var local = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!local.DirectoryExists("temp"))
                            local.CreateDirectory("temp");
                        var saveName = FileType.GetRealTypeName(file.Name);
                        var localFileStream = local.CreateFile("temp/" + saveName);
                        var copyTask = sdStream.CopyToAsync(localFileStream, 4096);
                        await copyTask.ContinueWith(new Action<System.Threading.Tasks.Task>(async (task) => 
                        {
                            try
                            {
                                localFileStream.Close();
                                localFileStream.Dispose();
                                sdStream.Close();
                                sdStream.Dispose();

                                if (task.IsCompleted && !task.IsCanceled)
                                {
                                    var tempFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("temp");
                                    StorageFile tempFile = await tempFolder.GetFileAsync(saveName);
                                    await Launcher.LaunchFileAsync(tempFile);
                                }
                            }
                            catch { }

                            this.Dispatcher.BeginInvoke(() => { ProgressTopBar.IsIndeterminate = false; });

                        }));
                    }
                }
                catch
                {
                    ProgressTopBar.IsIndeterminate = false;
                }
            }

            this.SdFolderViewList.SelectedItem = null;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (this.SdFolderStack.Count > 1)
            {
                this.SdFolderStack.RemoveAt(SdFolderStack.Count - 1);
                var last = SdFolderStack.LastOrDefault();
                if (last != null)
                {
                    this.Sb_Content_SliderRight1.Begin();
                    this.SdFolderViewList.ItemsSource = last.Item1;
                    this.Sb_Content_SliderRight2.Begin();
                    this.Tb_Path.Text = last.Item2;
                }
            }
        }
 
    }



}