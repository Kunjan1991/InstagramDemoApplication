using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using InstagramDemo.Resources;

namespace InstagramDemo.Data
{
   public class Response
    {
       public static void NetworkErrorMsg()
       {
           Deployment.Current.Dispatcher.BeginInvoke(() =>
           {               
               var message = new CustomMessageBox
               {
                   Caption = AppResources.Alert,
                   Message = AppResources.NetworkErrorMsg,                   
                   LeftButtonContent = AppResources.LeftBtnContent,
               };
               //message.Dismissed += (sender, args) =>
               //{
               //    ((CustomMessageBox)sender).Dismissing += (o, eventArgs) => eventArgs.Cancel = true;
               //    if (args.Result == CustomMessageBoxResult.LeftButton)
               //    {                       
               //        CallSetting();
               //    }
               //    else if (args.Result == CustomMessageBoxResult.RightButton)
               //    {
               //        App.OnOff = false;

               //        Application.Current.Terminate();
               //    }
               //};
               message.Show();
           });
       }
       public static void UnknownErrorMsg()
       {
           Deployment.Current.Dispatcher.BeginInvoke(() =>
              {
                  var message = new CustomMessageBox
                  {
                      Caption = AppResources.Alert,
                      Message = AppResources.UnknownErrorMsg,
                      LeftButtonContent = AppResources.LeftBtnContent,
                  };
                  //message.Dismissed += (sender, args) =>
                  //{
                  //    ((CustomMessageBox)sender).Dismissing += (o, eventArgs) => eventArgs.Cancel = true;
                  //    if (args.Result == CustomMessageBoxResult.LeftButton)
                  //    {                       
                  //        CallSetting();
                  //    }
                  //    else if (args.Result == CustomMessageBoxResult.RightButton)
                  //    {
                  //        App.OnOff = false;

                  //        Application.Current.Terminate();
                  //    }
                  //};
                  message.Show();
              });
       }
       public static string Gettime(string p)
       {
           DateTime time = DateTime.Parse(p);
           DateTime ty = DateTime.Now;
           string posttime;
           int d = (int)ty.Subtract(time).TotalDays;
           {
               if (d < 1)
               {
                   int h = (int)ty.Subtract(time).TotalHours;
                   if (h < 1)
                   {
                       int M = (int)ty.Subtract(time).TotalMinutes;
                       if (M < 1)
                       {
                           if (ty.CompareTo(time) > 0)
                           {
                               var seconds = (int)(ty - time).TotalSeconds;
                               posttime = seconds.ToString() + " seconds ago";
                           }
                           else
                               posttime = "";
                           //posttime = ((int)ty.Subtract(time).TotalSeconds).ToString() + " seconds ago";                           
                       }
                       else
                       {
                           if (M == 1)
                               posttime = M.ToString() + " minute ago";
                           else
                               posttime = M.ToString() + " minutes ago";
                       }
                   }
                   else
                   {
                       if (h == 1)
                           posttime = h.ToString() + " hour ago";
                       else
                           posttime = h.ToString() + " hours ago";
                   }
               }
               else
               {
                   if (d == 1)
                       posttime = d.ToString() + " day ago";
                   else
                       posttime = d.ToString() + " days ago";
               }
           }
           return posttime;
       }
       //public static void SaveLogToFile()
       //{
       //    using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
       //    {
       //        using (IsolatedStorageFileStream fs = storage.CreateFile("logfileMysocial.txt"))
       //        {
       //            using (StreamWriter writer = new StreamWriter(fs))
       //            {
       //                if (!string.IsNullOrEmpty(writer.ToString()))
       //                    Logger.Save(writer);
       //            }
       //        }
       //    }
       //}
       public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
       {
           if (depObj != null)
           {
               for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
               {
                   DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                   if (child != null && child is T)
                   {
                       yield return (T)child;
                   }

                   foreach (T childOfChild in FindVisualChildren<T>(child))
                   {
                       yield return childOfChild;
                   }
               }
           }
       }
    }
}
