
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Dom_android
{
    [BroadcastReceiver(Name = "com.Dom_android.dombroadcast", Exported = true, DirectBootAware = true, Enabled = true)]
    [IntentFilter(new[] {/* Intent.ActionBootCompleted,*/ Intent.ActionLockedBootCompleted })]
    public class DomBroadcastReceiver : BroadcastReceiver
    {
        Thread thread = new Thread(() => { });

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                thread = new Thread(() =>
                {                 
                    if (intent.Action.Equals(Intent.ActionBootCompleted))
                    {
                       // Toast.MakeText(context, Intent.ActionBootCompleted, ToastLength.Short).Show();
                        Intent handler = new Intent(context, typeof(MainActivity));
                        handler.AddFlags(ActivityFlags.NewTask);
                        context.StartActivity(handler);
                        Android.Util.Log.WriteLine(Android.Util.LogPriority.Info, "Dom", "send on boot");


                    }

                    if (intent.Action.Equals(Intent.ActionLockedBootCompleted))
                    {
                        //Toast.MakeText(context, Intent.ActionLockedBootCompleted, ToastLength.Short).Show();
                        Intent handler = new Intent(context, typeof(MainActivity));
                        handler.AddFlags(ActivityFlags.NewTask);
                        context.StartActivity(handler);
                        Android.Util.Log.WriteLine(Android.Util.LogPriority.Info, "Dom", "send on boot");
                        //    //Service    
                        //    Intent serviceIntent = new Intent(context, StartOnBootService.class);       
                        //context.startService(serviceIntent);   
                    }


                    if (WifiManager.NetworkStateChangedAction == intent.Action)
                    {
                        var netInfo = (NetworkInfo)intent.GetParcelableExtra(WifiManager.ExtraNetworkInfo);
                        var netInfoDetailed = netInfo.GetDetailedState();
                        if (netInfo.IsConnected || netInfoDetailed == NetworkInfo.DetailedState.Connected)
                        {
                            //string SSID = (((WifiManager)Application.Context.GetSystemService(Context.WifiService)).ConnectionInfo.SSID);                        
                            //if(SSID.Contains("Home_WiFi"))
                            //{
                            Console.WriteLine("Połączono z siecią - BroadcastReciver");
                            Client.Begin();
                            //}     
                        }
                    }
                });


                if (thread.ThreadState == ThreadState.Running)
                {
                    return;
                }

                thread.Start();
            }
            catch (Exception e)
            {
                Toast.MakeText(context, e.Message, ToastLength.Long).Show();
            }


            /* if(WifiManager.WifiStateChangedAction == intent.Action)
             {



                 var wifiManager = (WifiManager)Application.Context.GetSystemService(Context.WifiService);
                 if (wifiManager.ConnectionInfo.SupplicantState == SupplicantState.Completed)
                 {
                     Toast.MakeText(context, wifiManager.ConnectionInfo.SSID, ToastLength.Short).Show();
                 }
                 else
                 {
                     Toast.MakeText(context, "WiFi hasn't got SSID", ToastLength.Short).Show();
                 }
             }
             */
        }
    }
}