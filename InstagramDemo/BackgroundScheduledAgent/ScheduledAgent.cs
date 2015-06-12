#define DEBUG_AGENT

using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using System.Net;
using System;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Net.NetworkInformation;
using Hammock.Authentication.OAuth;
using Hammock;
using Hammock.Web;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using CaledosLab.Portable.Logging;
using System.IO;
using OnesocialBackgroundAgent;

namespace BackgroundScheduledAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        private static readonly object _sync = new object();
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }
        IsoStorageData temp = new IsoStorageData();
        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            Logger.WriteLine(e.ExceptionObject != null ? e.ExceptionObject.Message != null ? e.ExceptionObject.Message : "Error From background debugger break" : "Error From debugger debugger break");
            SaveLogToFile();
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                e.Handled = true;
                //  Debugger.Break();
            }
        }
        public static void SaveLogToFile()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream fs = storage.CreateFile("logfilebackgroundMysocial.txt"))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        if (!string.IsNullOrEmpty(writer.ToString()))
                            Logger.Save(writer);
                    }
                }
            }
        }
        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            try
            {
#if DEBUG_AGENT
                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromMinutes(10));
#endif
                UpdateFacebook();
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/OnInvoke");
                SaveLogToFile();
                NotifyComplete();
                Console.WriteLine(ex.Message);
            }
        }

        //facebook-------------------
        private void UpdateFacebook()
        {
            try
            {
                if (((IsolatedStorageSettings.ApplicationSettings.Contains("facebookLogIn")) || (IsolatedStorageSettings.ApplicationSettings.Contains("facebookLogIn") && (bool)IsolatedStorageSettings.ApplicationSettings["facebookLogIn"] == false)))
                {
                    var session = SessionStorage.Load();
                    if (null != session && IsolatedStorageSettings.ApplicationSettings.Contains("FacebookBaseValue") && (!string.IsNullOrEmpty(IsolatedStorageSettings.ApplicationSettings["FacebookBaseValue"].ToString())))
                    {
                        CallFacebook(IsolatedStorageSettings.ApplicationSettings["FacebookBaseValue"].ToString());
                    }
                    else
                        UpdateTwitter();
                }
                else
                    UpdateTwitter();
            }
            catch
            {
                UpdateTwitter();
            }

        }
        private void CallFacebook(string url)
        {
            if (NetworkInterface.GetIsNetworkAvailable() && url != string.Empty)
            {
                WebClient wb = new WebClient();
                wb.DownloadStringAsync(new Uri(url, UriKind.RelativeOrAbsolute));
                wb.DownloadStringCompleted += wbFB_DownloadStringCompleted;
            }
            else
                UpdateTwitter();
        }
        private void wbFB_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e != null && e.Result != null && !string.IsNullOrEmpty(e.Result.ToString()))
                {
                    Home root = Newtonsoft.Json.JsonConvert.DeserializeObject<Home>(e.Result.ToString());
                    if (root != null && root.data != null)
                    {
                        temp.FacebookBaseValueCount = root.data.Count.ToString();
                        if (IsolatedStorageSettings.ApplicationSettings.Contains("FacebookBaseValueCount"))
                            IsolatedStorageSettings.ApplicationSettings["FacebookBaseValueCount"] = root.data.Count.ToString();
                        else
                            IsolatedStorageSettings.ApplicationSettings.Add("FacebookBaseValueCount", root.data.Count.ToString());
                        //   SaveSettings();
                        IsolatedStorageSettings.ApplicationSettings.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/wbFB_DownloadStringCompleted");
                SaveLogToFile();
            }
            UpdateTwitter();
        }


        //twitter-------------------
        private void UpdateTwitter()
        {
            try
            {
                if (((IsolatedStorageSettings.ApplicationSettings.Contains("TwitterLogIn")) || (IsolatedStorageSettings.ApplicationSettings.Contains("TwitterLogIn") && (bool)IsolatedStorageSettings.ApplicationSettings["TwitterLogIn"] == false)))
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("TwitterBaseValue") && (!string.IsNullOrEmpty(IsolatedStorageSettings.ApplicationSettings["TwitterBaseValue"].ToString())))
                    {
                        CallTwitter(IsolatedStorageSettings.ApplicationSettings["TwitterBaseValue"].ToString());
                    }
                    else
                        UpdateLinkedIn();
                }
                else
                    UpdateLinkedIn();
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/UpdateTwitter");
                SaveLogToFile();
                UpdateLinkedIn();
            }
        }
        private void CallTwitter(string p)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("TwitterAccessToken") && IsolatedStorageSettings.ApplicationSettings.Contains("TwitterAccessTokenSecret") &&
                !string.IsNullOrEmpty(IsolatedStorageSettings.ApplicationSettings["TwitterAccessToken"].ToString()) && !string.IsNullOrEmpty(IsolatedStorageSettings.ApplicationSettings["TwitterAccessTokenSecret"].ToString()))
            {
                var credentials = new OAuthCredentials
                {
                    Type = OAuthType.ProtectedResource,
                    SignatureMethod = OAuthSignatureMethod.HmacSha1,
                    ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                    ConsumerKey = TwitterSettings.consumerKey,
                    ConsumerSecret = TwitterSettings.consumerKeySecret,
                    Token = IsolatedStorageSettings.ApplicationSettings["TwitterAccessToken"].ToString(),
                    TokenSecret = IsolatedStorageSettings.ApplicationSettings["TwitterAccessTokenSecret"].ToString(),
                    Version = "1.0"
                };

                var restClient = new RestClient
                {
                    Authority = "https://api.twitter.com",
                    HasElevatedPermissions = true
                };

                var restRequest = new RestRequest
                {
                    Credentials = credentials,
                    Path = "/1.1/statuses/home_timeline.json?since_id=" + p,
                    Method = WebMethod.Get,
                };
                restClient.BeginRequest(restRequest, new RestCallback(PostTweetRequestCallback));

            }
            else
                UpdateLinkedIn();

        }
        private void PostTweetRequestCallback(RestRequest request, RestResponse response, object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(response.Content.ToString()))
                {
                    List<RootObjectTwt> root = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RootObjectTwt>>(response.Content.ToString());
                    if (root != null)
                    {
                        temp.TwitterBaseValueCount = root.Count.ToString();
                        if (IsolatedStorageSettings.ApplicationSettings.Contains("TwitterBaseValueCount"))
                            IsolatedStorageSettings.ApplicationSettings["TwitterBaseValueCount"] = root.Count.ToString();
                        else
                            IsolatedStorageSettings.ApplicationSettings.Add("TwitterBaseValueCount", root.Count.ToString());
                        IsolatedStorageSettings.ApplicationSettings.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/PostTweetRequestCallback");
                SaveLogToFile();
            }
            UpdateLinkedIn();
        }

        //LinkedIn-------------------
        private void UpdateLinkedIn()
        {
            try
            {
                if (((IsolatedStorageSettings.ApplicationSettings.Contains("LinkedInLogIn")) || (IsolatedStorageSettings.ApplicationSettings.Contains("LinkedInLogIn") && (bool)IsolatedStorageSettings.ApplicationSettings["LinkedInLogIn"] == false)))
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("LinkedInBaseValue") && (((object)IsolatedStorageSettings.ApplicationSettings["LinkedInBaseValue"]) != null))
                    {
                        CallLinkedIn((object)IsolatedStorageSettings.ApplicationSettings["LinkedInBaseValue"]);
                    }
                    else
                        UpdateInstagram();
                }
                else
                    UpdateInstagram();
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/UpdateLinkedIn");
                SaveLogToFile();
                UpdateInstagram();
            }
        }
        private void CallLinkedIn(object p)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LinkedInAccessToken") && IsolatedStorageSettings.ApplicationSettings.Contains("LinkedInAccessTokenSecret") &&
                 !string.IsNullOrEmpty(IsolatedStorageSettings.ApplicationSettings["LinkedInAccessToken"].ToString()) && !string.IsNullOrEmpty(IsolatedStorageSettings.ApplicationSettings["LinkedInAccessTokenSecret"].ToString()))
            {

                var credentials = new OAuthCredentials
                {
                    Type = OAuthType.ProtectedResource,
                    SignatureMethod = OAuthSignatureMethod.HmacSha1,
                    ParameterHandling = OAuthParameterHandling.UrlOrPostParameters,
                    ConsumerKey = LinkedInAppsettings.consumerKey,
                    ConsumerSecret = LinkedInAppsettings.consumerKeySecret,
                    Token = IsolatedStorageSettings.ApplicationSettings["LinkedInAccessToken"].ToString(),
                    TokenSecret = IsolatedStorageSettings.ApplicationSettings["LinkedInAccessTokenSecret"].ToString(),
                    Version = "1.0"
                };

                var client = new RestClient
                {
                    Authority = "https://api.linkedin.com",
                    HasElevatedPermissions = true
                };

                var request = new RestRequest
                {
                    Credentials = credentials,
                    Path = "v1/people/~/network/updates?after=" + p + "&format=json",
                    Method = WebMethod.Get,
                };
                client.BeginRequest(request, new RestCallback(CurrentUserProfileRequestCallback));
            }
            else
                UpdateInstagram();
        }
        private void CurrentUserProfileRequestCallback(RestRequest request, RestResponse response, object obj)
        {
            try
            {
                if (response != null && !string.IsNullOrEmpty(response.Content))
                {
                    RootObjectLinkedIn root = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObjectLinkedIn>(response.Content);
                    //  this.DataContext = root;
                    if (root != null && root._count > 0)
                    {
                        temp.LinkedInBaseValueCount = (root._count - 1).ToString();
                        if (IsolatedStorageSettings.ApplicationSettings.Contains("LinkedInBaseValueCount"))
                            IsolatedStorageSettings.ApplicationSettings["LinkedInBaseValueCount"] = (root._count - 1).ToString();
                        else
                            IsolatedStorageSettings.ApplicationSettings.Add("LinkedInBaseValueCount", (root._count - 1).ToString());
                        IsolatedStorageSettings.ApplicationSettings.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/CurrentUserProfileRequestCallback");
                SaveLogToFile();
            }
            UpdateInstagram();
        }

        //Instagram-------------------
        private void UpdateInstagram()
        {
            try
            {
                if (((IsolatedStorageSettings.ApplicationSettings.Contains("InstagramLogIn")) || (IsolatedStorageSettings.ApplicationSettings.Contains("InstagramLogIn") && (bool)IsolatedStorageSettings.ApplicationSettings["InstagramLogIn"] == false)))
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("InstagramBaseValue") && (!string.IsNullOrEmpty(IsolatedStorageSettings.ApplicationSettings["InstagramBaseValue"].ToString())))
                    {
                        CallInstagram(IsolatedStorageSettings.ApplicationSettings["InstagramBaseValue"].ToString());
                    }
                    else
                        UpdateTumblr();
                }
                else
                    UpdateTumblr();
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/UpdateInstagram");
                SaveLogToFile();
                UpdateTumblr();
            }
        }
        private async void CallInstagram(string p)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, p);
                HttpResponseMessage response;
                HttpClient _httpClient = new HttpClient();
                response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    RootObjectInstagram root = JsonConvert.DeserializeObject<RootObjectInstagram>(responseBody);
                    if (root != null && root.data != null && root.data.Count > 0)
                    {
                        temp.InstagramBaseValueCount = root.data.Count.ToString();
                        if (IsolatedStorageSettings.ApplicationSettings.Contains("InstagramBaseValueCount"))
                            IsolatedStorageSettings.ApplicationSettings["InstagramBaseValueCount"] = root.data.Count.ToString();
                        else
                            IsolatedStorageSettings.ApplicationSettings.Add("InstagramBaseValueCount", root.data.Count.ToString());
                        IsolatedStorageSettings.ApplicationSettings.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/CallInstagram");
                SaveLogToFile();
            }
            UpdateTumblr();
        }

        //Tumblr-------------------
        private void UpdateTumblr()
        {
            try
            {
                if (((IsolatedStorageSettings.ApplicationSettings.Contains("TumblrLogIn")) || (IsolatedStorageSettings.ApplicationSettings.Contains("TumblrLogIn") && (bool)IsolatedStorageSettings.ApplicationSettings["TumblrLogIn"] == false)))
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("TumblrBaseValue") && (!string.IsNullOrEmpty(IsolatedStorageSettings.ApplicationSettings["TumblrBaseValue"].ToString())))
                    {
                        CallTumblr(IsolatedStorageSettings.ApplicationSettings["TumblrBaseValue"].ToString());
                    }
                    else
                        UpdateVkontakte();
                }
                else
                    UpdateVkontakte();
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/UpdateTumblr");
                SaveLogToFile();
                UpdateVkontakte();
            }
        }
        private void CallTumblr(string p)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("TumblrAccessToken") && IsolatedStorageSettings.ApplicationSettings.Contains("TumblrAccessTokenSecret") &&
                !string.IsNullOrEmpty(IsolatedStorageSettings.ApplicationSettings["TumblrAccessToken"].ToString()) && !string.IsNullOrEmpty(IsolatedStorageSettings.ApplicationSettings["TumblrAccessTokenSecret"].ToString()))
            {
                var credentials = new OAuthCredentials
                {
                    Type = OAuthType.ProtectedResource,
                    SignatureMethod = OAuthSignatureMethod.HmacSha1,
                    ParameterHandling = OAuthParameterHandling.UrlOrPostParameters,
                    ConsumerKey = TumblrAppSettings.consumerKey,
                    ConsumerSecret = TumblrAppSettings.consumerKeySecret,
                    Token = IsolatedStorageSettings.ApplicationSettings["TumblrAccessToken"].ToString(),
                    TokenSecret = IsolatedStorageSettings.ApplicationSettings["TumblrAccessTokenSecret"].ToString(),
                    Version = "1.0"
                };

                RestClient client = new RestClient
                {
                    Authority = "https://api.tumblr.com",
                    HasElevatedPermissions = true
                };

                var request = new RestRequest
                {
                    Credentials = credentials,
                    Path = "/v2/user/dashboard?since_id=" + p,
                    Method = WebMethod.Get
                };
                client.BeginRequest(request, new RestCallback(CurrentUserRequestCallback));
            }
            else
                UpdateVkontakte();
        }
        private void CurrentUserRequestCallback(RestRequest request, RestResponse response, object obj)
        {
            try
            {
                if (response != null && !string.IsNullOrEmpty(response.Content))
                {
                    RootObject root = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(response.Content);
                    //  this.DataContext = root;
                    if (root.response != null && root.response.posts != null)
                    {
                        temp.TumblrBaseValueCount = root.response.posts.Count.ToString();
                        if (IsolatedStorageSettings.ApplicationSettings.Contains("TumblrBaseValueCount"))
                            IsolatedStorageSettings.ApplicationSettings["TumblrBaseValueCount"] = root.response.posts.Count.ToString();
                        else
                            IsolatedStorageSettings.ApplicationSettings.Add("TumblrBaseValueCount", root.response.posts.Count.ToString());
                        IsolatedStorageSettings.ApplicationSettings.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/CurrentUserRequestCallback");
                SaveLogToFile();
            }
            UpdateVkontakte();
        }

        //Vkontakte-------------------
        private void UpdateVkontakte()
        {
            try
            {
                if (((IsolatedStorageSettings.ApplicationSettings.Contains("VkontakteLogIn")) || (IsolatedStorageSettings.ApplicationSettings.Contains("VkontakteLogIn") && (bool)IsolatedStorageSettings.ApplicationSettings["VkontakteLogIn"] == false)))
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("VkontakteBaseValue") && (!string.IsNullOrEmpty(IsolatedStorageSettings.ApplicationSettings["VkontakteBaseValue"].ToString())))
                    {
                        CallVkontakte(IsolatedStorageSettings.ApplicationSettings["VkontakteBaseValue"].ToString());
                    }
                    else
                    {
                        savecount();
                        NotifyComplete();
                    }
                }
                else
                {
                    savecount();
                    NotifyComplete();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/UpdateVkontakte");
                SaveLogToFile();
                savecount();
                NotifyComplete();
            }
        }
        private void CallVkontakte(string url)
        {
            if (NetworkInterface.GetIsNetworkAvailable() && url != string.Empty)
            {
                WebClient wb = new WebClient();
                wb.DownloadStringAsync(new Uri(url, UriKind.RelativeOrAbsolute));
                wb.DownloadStringCompleted += wbVk_DownloadStringCompleted;
            }
            else
            {
                savecount();
                NotifyComplete();
            }
        }
        private void wbVk_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e != null && e.Result != null && !string.IsNullOrEmpty(e.Result.ToString()))
                {
                    RootObjectNews root = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObjectNews>(e.Result.ToString());
                    if (root != null && root.response != null && root.response.items != null && root.response.items.Count > 0)
                    {
                        temp.VkontakteBaseValueCount = (root.response.items.Count - 1).ToString();
                        if (IsolatedStorageSettings.ApplicationSettings.Contains("VkontakteBaseValueCount"))
                            IsolatedStorageSettings.ApplicationSettings["VkontakteBaseValueCount"] = (root.response.items.Count - 1).ToString();
                        else
                            IsolatedStorageSettings.ApplicationSettings.Add("VkontakteBaseValueCount", (root.response.items.Count - 1).ToString());
                        //   SaveSettings();
                        IsolatedStorageSettings.ApplicationSettings.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine((ex.Message != null ? ex.Message : "") + "BackgroundOneSocial/wbVk_DownloadStringCompleted");
                SaveLogToFile();
            }
            savecount();
            NotifyComplete();
        }


        private void savecount()
        {
            MutexedIsoStorageFile.Write(temp);
        }

        private static void SaveSettings()
        {
            lock (_sync)
            {
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }
    }
}