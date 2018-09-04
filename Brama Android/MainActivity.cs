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
using System.Threading;

enum Ster : byte { otwórz = 145, zamknij = 167, stop = 155 };


namespace Aplikacja_do_bramy_na_androida
{
    [Activity(/*Label = "Aplikacja_do_esp_na_androida",*/ MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {

        TcpClient client;
        // String haha;
        TextView info;

        byte[] recvbuf = new byte[100];

        bool connected = false;

        NetworkStream stream;
        // byte[] datalength = new byte[4];


        EditText tekst = null;



        void Revived(IAsyncResult a)
        {
            RunOnUiThread(() =>
            {
                String tmp = Encoding.ASCII.GetString(recvbuf);
                if (String.Compare(tmp, "opning") == 0)
                {

                    info.Text = "Otwieranie bramy...";

                }
                else if (String.Compare(tmp, "cloing") == 0)
                {
                    info.Text = "Zamykanie bramy...";

                }
                else if (String.Compare(tmp, "opened") == 0)
                {
                    info.Text = "Brama jest otwarta";

                }
                else if (String.Compare(tmp, "closed") == 0)
                {
                    info.Text = "Brama jest zamknięta";

                }
                else if (String.Compare(tmp, "brokee") == 0)
                {
                    info.Text = "Atywny czujnik przerwana wiązki IR";

                }
                else if (String.Compare(tmp, "none..") == 0)
                {
                    info.Text = "Brama nie jest ani zamknięta, ani otwarta";
                }
            });
        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);



            Thread th = new Thread(() =>
            {
                try
                {
                    client = new TcpClient("192.168.1.4", 80);
                    stream = client.GetStream();

                    connected = client.Connected;
                    Resp();

                }
                catch (Exception e)
                {
                    RunOnUiThread(() => tekst.Text += "Nie można nawiązać połązenia, \n" + "\n" + e.Message);
                }
            });

            th.Start();

           new Thread(() =>
           {
               for (;;)
               {
                   if (client != null)
                   {
                       client.GetStream().Write(Encoding.Default.GetBytes(" "), 0, 1);
                   }
                   Thread.Sleep(30000);
               }
              
           }).Start();




            info = FindViewById<TextView>(Resource.Id.info);

            Button otworz = FindViewById<Button>(Resource.Id.otworz);
            Button zamknij = FindViewById<Button>(Resource.Id.zamknij);

            Button stop = FindViewById<Button>(Resource.Id.stop);

            Button polacz = FindViewById<Button>(Resource.Id.polacz);

            otworz.Click += delegate { ClientSend(Ster.otwórz); };
            zamknij.Click += delegate { ClientSend(Ster.zamknij); };
            stop.Click += delegate { ClientSend(Ster.stop); };
            polacz.Click += Polacz_Click;


            tekst = FindViewById<EditText>(Resource.Id.tekst);

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


        }

        private void Resp()
        {
            if (connected)
            {
                //stream = client.GetStream();

                new Thread(() =>
                {
                    if (connected)
                    {
                        for (;;)
                        {
                            if (stream.CanRead)
                            {
                                try
                                {

                                    /*RunOnUiThread(() => {*/
                                    stream.BeginRead(recvbuf, 0, recvbuf.Length, new AsyncCallback(Revived), stream); // });

                                }
                                catch { }
                            }
                            Thread.Sleep(10);

                        }
                    }
                }).Start();
            }

        }

        

        private void ClientSend(Ster msg)
        {               
                            
            try
            {
                if (!connected)
                {

                    Thread th = new Thread(() =>
                    {
                        try
                        {
                            client = new TcpClient("192.168.1.4", 80);
                            stream = client.GetStream();
                            
                            connected = client.Connected;
                            Resp();

                        }
                        catch (Exception e)
                        {
                            RunOnUiThread(() => tekst.Text += "Nie można nawiązać połązenia, \n"+ "\n" + e.Message);
                        }
                    });

                    th.Start();
                    th.Join();
                    
                }

                //stream.BeginRead(recvbuf, 0, recvbuf.Length, new AsyncCallback(revived), stream);
                //  Toast.MakeText(this, "ss", ToastLength.Short).Show();

                byte[] tmp = new byte[1];
                tmp[0]  = (byte) msg;

                client.GetStream().Write(tmp, 0, 1);

            }
            catch (Exception ex)
            {
                //connected = false;
                tekst.Text += "\n" + ex.Message;
            }

            }
     
           
        }

    }      