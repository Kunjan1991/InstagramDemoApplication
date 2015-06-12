using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;using InstagramDemo.Data;
using System.IO.IsolatedStorage;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Threading;
using Microsoft.Phone.Net.NetworkInformation;
using CaledosLab.Portable.Logging;
using System.IO;


namespace InstagramDemo.View
{
    public partial class InstagramPage : PhoneApplicationPage
    {        
        private HttpClient _httpClient;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var tempEvent = PropertyChanged;
            if (tempEvent != null)
            {
                tempEvent(this, e);
            }
        }
        bool busy;
        public bool Busy
        {
            get
            {
                return busy;
            }
            set
            {
                if (busy == value)
                {
                    return;
                }
                busy = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Busy"));
            }
        }
       
        #region constructor
        public InstagramPage()
        {
            InitializeComponent();
            GC.Collect();
            GC.WaitForPendingFinalizers();          
            progressOverlay.Visibility = Visibility.Visible;           
            _httpClient = new HttpClient();
            if ((!(IsolatedStorageSettings.ApplicationSettings.Contains("InstagramLogIn")) || (IsolatedStorageSettings.ApplicationSettings.Contains("InstagramLogIn") && (bool)IsolatedStorageSettings.ApplicationSettings["InstagramLogIn"] == false)))
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    progressOverlay.Hide();
                    progressOverlay.Visibility = Visibility.Collapsed;
                    loginBrowserControl.Navigate(new Uri("https://instagram.com/oauth/authorize/?client_id=" + InstagramSettings.consumerKey + "&redirect_uri=" + InstagramSettings.redirect_Uri + "&response_type=token&scope=basic+likes+comments+relationships"));
                    Logger.WriteLine("Instagram login");                    
                }
                else
                {                   
                    progressOverlay.Visibility = Visibility.Collapsed;
                    progressOverlay1.Visibility = Visibility.Collapsed;                  
                    Response.NetworkErrorMsg();
                }
            }
            else
            {              
                progressOverlay.Visibility = Visibility.Collapsed;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                Busy = false;
        }
        #endregion        

        #region browserevents
        private void loginBrowserControl_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                this.loginBrowserControl.Visibility = Visibility.Visible;
                if (e.Uri.AbsoluteUri.Contains('#'))
                {
                    //parse our access token
                    if (e.Uri.Fragment.StartsWith("#access_token="))
                    {
                        string token = e.Uri.Fragment.Replace("#access_token=", string.Empty);
                        MainUtil.SetKeyValue<string>("InstagramAccessToken", token);


                        this.loginBrowserControl.Visibility = Visibility.Collapsed;                       
                        if (!IsolatedStorageSettings.ApplicationSettings.Contains("InstagramLogIn"))
                            IsolatedStorageSettings.ApplicationSettings.Add("InstagramLogIn", true);
                        else
                            IsolatedStorageSettings.ApplicationSettings["InstagramLogIn"] = true;
                        IsolatedStorageSettings.ApplicationSettings.Save();

                        loginBrowserControl.Navigated -= loginBrowserControl_Navigated;
                        loginBrowserControl.Navigating -= loginBrowserControl_Navigating;
                        loginBrowserControl = null;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                else
                {
                    progressOverlay.Visibility = Visibility.Collapsed;
                    progressOverlay1.Visibility = Visibility.Collapsed;                   
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "InstagramPage.xaml/loginBrowserControl_Navigated");
                App.SaveLogToFile();
            }
        }
        private void loginBrowserControl_Navigating(object sender, NavigatingEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        private void Restaurantslist_webClinet_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e != null && e.Error != null)
            {
            }
        }
        #endregion        
        
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {           
            if (progressOverlay.Visibility == Visibility.Visible)
            {
                progressOverlay.Hide();
                progressOverlay.Visibility = Visibility.Collapsed;
                base.OnBackKeyPress(e);
                e.Cancel = true;
            }
            else
            {
                ClearWebBrowserVisualTree();
                ResetPageCache();
                CallGCForFreeMemory();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                System.Windows.Application.Current.Terminate();
            }
        }

        #region GarbageCollection
        private const long maxGarbage = 150;
        private readonly ObservableCollection<string> _History =
            new ObservableCollection<string>();
        private readonly Stack<Uri> _NavigatingUrls = new Stack<Uri>();

        private void ClearWebBrowserVisualTree()
        {
            try
            {
                foreach (var border in Response.FindVisualChildren<Grid>(ContentPanel))
                {
                    foreach (var image in Response.FindVisualChildren<Image>(border))
                    {
                        if (image.Name != "imgOpenMenu" && image.Source != null)
                        {
                            BitmapImage bitmapImage = image.Source as BitmapImage;
                            bitmapImage.UriSource = null;
                            image.Source = null;
                            ContentPanel.Children.Remove(image);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "InstagramPage.xaml/ClearWebBrowserVisualTree");
                App.SaveLogToFile();
            }
        }
        public ObservableCollection<string> History
        {
            get { return _History; }
        }

        #region Garbage Collecter
        void MakeSomeGarbage()
        {
            Version vt;
            for (int i = 0; i < maxGarbage; i++)
                vt = new Version("1.0.0.0");
        }
        void CallGCForFreeMemory()
        {
            try
            {
                InstagramPage myGCCol = new InstagramPage();
                myGCCol.MakeSomeGarbage();
                GC.Collect();
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "InstagramPage.xaml/CallGCForFreeMemory");
                App.SaveLogToFile();
            }
        }

        private void ResetPageCache()
        {
            var cacheSize = ((Frame)Parent).CacheSize;
            ((Frame)Parent).CacheSize = 0;
            ((Frame)Parent).CacheSize = cacheSize;
        }
        #endregion
        #endregion
    }
}