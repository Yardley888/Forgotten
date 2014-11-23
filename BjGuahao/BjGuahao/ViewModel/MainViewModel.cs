using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using BjGuahao.Service;
using System.Windows;
using System.Linq;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System;
using System.Windows.Navigation;
using BjGuahao.Model;
using System.Collections.ObjectModel;
using DataLib;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace BjGuahao.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        #region Properties

        public bool TurnG3GoAway { get; set; }

        public string ApplicationTitle
        {
            get
            {
                return "MVVM LIGHT";
            }

        }

        public string PageName
        {
            get
            {
                return "My page:";
            }
        }

        public string Welcome
        {
            get
            {
                return "Welcome to MVVM Light";
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<Hospital> hostList_;
        public System.Collections.ObjectModel.ObservableCollection<Hospital> HostList
        {
            get
            {
                if (hostList_ == null)
                    hostList_ = new System.Collections.ObjectModel.ObservableCollection<Hospital>();
                return hostList_;
            }
            set
            {
                hostList_ = value;
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<Department> departList_;
        public System.Collections.ObjectModel.ObservableCollection<Department> DepartList
        {
            get
            {
                if (departList_ == null)
                    departList_ = new System.Collections.ObjectModel.ObservableCollection<Department>();
                return departList_;
            }
            set
            {
                departList_ = value;
            }
        }

        private RelayCommand GoCommand_;

        public RelayCommand GoCommand
        {
            get { return GoCommand_; }
            set
            {
                GoCommand_ = value;
                this.RaisePropertyChanged("GoCommand");
            }
        }

        private RelayCommand LogonCommand_;

        public RelayCommand LogonCommand
        {
            get { return LogonCommand_; }
            set
            {
                LogonCommand_ = value;
                this.RaisePropertyChanged("LogonCommand");
            }
        }

        private RelayCommand MainLoadedCommand_;
        public RelayCommand MainLoadedCommand
        {
            get { return MainLoadedCommand_; }
            set
            {
                MainLoadedCommand_ = value;
                this.RaisePropertyChanged("MainLoadedCommand");
            }
        }

        private RelayCommand<string> ItemTapCommand_;
        public RelayCommand<string> ItemTapCommand
        {
            get { return ItemTapCommand_; }
            set
            {
                ItemTapCommand_ = value;
                this.RaisePropertyChanged("ItemTapCommand");
            }
        }

        private RelayCommand<string> sectionTapCommand_;
        public RelayCommand<string> SectionTapCommand
        {
            get { return sectionTapCommand_; }
            set
            {
                sectionTapCommand_ = value;
                this.RaisePropertyChanged("SectionTapCommand");
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
                this.GoCommand = new RelayCommand(Go);
                this.LogonCommand = new RelayCommand(Logon);
                this.MainLoadedCommand = new RelayCommand(MainLoaded);
                this.ItemTapCommand = new RelayCommand<string>(ItemTap);
                this.SectionTapCommand = new RelayCommand<string>(SectionTap);
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        #region Command

        private void Go()
        {
            //Util.GetDepartments();
            Util.ShowContent();
            //Util.TestIsoFileMethod();
            //Util.GetDepartments();
            
            //Util.ReBuildData();
            //Util.ParseTest();
           
        }

        private void Logon()
        {

        }

        private void MainLoaded()
        {
            GetLocalHospitalsToView(this.HostList);
            //ItemTap("yyks.php?hpid=142");
        }

        private void ItemTap(string link)
        {
            string url = Config.ServerCommm + link;
            HttpClient client = new HttpClient(url, true);
            client.AddHeader("Referer", Config.HomeUrl);
            client.LoadedSucceed += (s,e) =>
                {
                    if (e is string)
                    {
                        var departs = Util.GetDepartments(e as string);
                        if (departs != null)
                        {
                            foreach (var one in departs)
                            {
                                DepartList.Add(one);
                            }
                        }
                    }
                };
            client.run();
        }

        private void SectionTap(string link)
        {
            string url = Config.ServerUrl + link.TrimStart('/');
            HttpClient client = new HttpClient(url, true);
            client.AddHeader("Referer", Config.HomeUrl);
            client.LoadedSucceed += (s, e) =>
            {
                if (e is string)
                {
                    MessageBox.Show(e as string);
                }
            };
            client.run();
        }

        private bool GetLocalHospitalsToView(ObservableCollection<Hospital> hostList)
        {
            string content = IsoFile.ReadResourceContent("data.xml");
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    XElement doc = XElement.Parse(content);
                    List<Hospital> list = (from item in doc.Descendants("hospital")
                                           select new Hospital()
                                           {
                                               Name = item.Attribute("name").Value,
                                               Address = item.Attribute("address").Value,
                                               Link = item.Attribute("link").Value,
                                               Level = item.Attribute("level").Value,
                                               Hpid = item.Attribute("hpid").Value,
                                               District = item.Attribute("district").Value
                                           }).ToList();
                    foreach (var one in list)
                    {
                        hostList.Add(one);
                    }
                }
                catch(Exception e)
                {
                    Debug.WriteLine("GetLocalHospitalsToView error---" +e.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}