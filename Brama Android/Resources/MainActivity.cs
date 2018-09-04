using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Android.Net.Wifi;
using Android.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.Provider;
using Android.Content.PM;

namespace Aplikacja_do_bramy_na_androida
{
    [Activity(/*Label = "Aplikacja_do_esp_na_androida",*/ MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        TcpClient client; 




        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button otworz = FindViewById<Button>(Resource.Id.otworz);
            Button zamknij = FindViewById<Button>(Resource.Id.zamknij);

            Button stop = FindViewById<Button>(Resource.Id.stop);

            Button polacz = FindViewById<Button>(Resource.Id.polacz);

            otworz.Click += delegate { clientSend("o"); };
            zamknij.Click += delegate { clientSend("z"); };
            stop.Click += delegate { clientSend("s"); };
            polacz.Click += Polacz_Click;
        }

        private void Polacz_Click(object sender, EventArgs e)
        {
          WifiManager wifi = (WifiManager)GetSystemService(WifiService);
            

            if (!wifi.IsWifiEnabled)
            {
                wifi.SetWifiEnabled(true);
                Toast.MakeText(this, "Włączam Wifi", ToastLength.Short).Show();
            }


            if (String.Compare(wifi.ConnectionInfo.SSID, "\"Home_WiFi\"") != 0) // jest falszem
            {
                StartActivity(new Intent(Settings.ActionWifiSettings));
                wifi.EnableNetwork(0, true);
            }


            /*
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo wifiInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi);
            if (!wifiInfo.IsConnected)
            {
                Toast.MakeText(this, "niepolaczony", ToastLength.Short ).Show();

                Task.Factory.StartNew(() =>
                {
                    // string networkSSID = "bbox-xxx";
                    string networkPass = "rb26dett";

                    WifiConfiguration wifiConfig = new WifiConfiguration();
                    wifiConfig.Ssid = "\"Home_WiFi\"";
                    wifiConfig.PreSharedKey = string.Format("\"{0}\"", networkPass);

                    WifiManager wifiManager = (WifiManager)Application.Context.GetSystemService(Context.WifiService);
                    int netId = wifiManager.AddNetwork(wifiConfig);
                    wifiManager.Disconnect();
                    wifiManager.EnableNetwork(netId, true);
                    wifiManager.Reconnect();
                });
            }*/
        }

        public void clientSend(string msg)
        {
            //try {
                
                            
            try
            {
                    for (int i = 0; i < 3; i++)
                    {
                        client = new TcpClient("192.168.1.4", 80);
                        client.GetStream().Write(Encoding.Default.GetBytes(msg), 0, 1);
                        client.Close();
                    }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                 
            }

            }
           // catch (Exception e)
           // {
           //     Toast.MakeText(this, e.Message, ToastLength.Short).Show();
           // }
           
        }








    }      