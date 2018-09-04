
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using System.Net.Sockets;
using System.Threading;

namespace Dom_android
{    
	public class Brama : Android.Support.V4.App.Fragment
	{
        //TcpClient client;
        //// String haha;

        //byte[] recvbuf = new byte[100];

        //bool connected = false;

        //NetworkStream stream;
        // byte[] datalength = new byte[4];


        Button otworz = null;
        Button zamknij = null;
        Button stop = null;

        ProgressBar ConnectProgressBar = null;
        TextView info = null;
        EditText tekst = null;

        /// <summary>
        /// Stan Bramy
        /// </summary>
        public enum Stan
        {
            Otwarta = 2,
            Zamknięta = 3,
            Otwieranie = 4,
            Zamykanie = 5,
            NieOtwartaNieZamknięta = 6,
            CzujnikPrzerwaniaWiązki = 7,
            Awaria = 8,
            BrakPolaczenia = 9,
        };


        /// <summary>
        /// Zmienna przechowująca stan bramy
        /// </summary>
        public static Stan State;

        public override void OnResume()
        {
            BramaWrite(Commands.AkcjaBramy.Stan);
            base.OnResume();
        }

        public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);           
        }

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
           View view = inflater.Inflate(Resource.Layout.Brama, container, false);

            ConnectProgressBar = view.FindViewById<ProgressBar>(Resource.Id.ConnectProgressBar);

            info = view.FindViewById<TextView>(Resource.Id.info);

            otworz = view.FindViewById<Button>(Resource.Id.otworz);
            zamknij = view.FindViewById<Button>(Resource.Id.zamknij);

            stop = view.FindViewById<Button>(Resource.Id.stop);

             otworz.Click += delegate { BramaWrite(Commands.AkcjaBramy.Otwórz); };
             zamknij.Click += delegate { BramaWrite(Commands.AkcjaBramy.Zamknij); };
             stop.Click += delegate { BramaWrite(Commands.AkcjaBramy.Stop); };


            Client_ConnectedEvent(Client.Connected);
            Client.ConnectedEvent += Client_ConnectedEvent;

            Client.BramaEvent += Client_BramaEvent;


            //Thread th = new Thread(() =>
            //{
            //    try
            //    {
            //        client = new TcpClient("192.168.1.4", 80);                    
            //        stream = client.GetStream();
            //        connected = client.Connected;
            //        Resp();
            //        Activity.RunOnUiThread(() => 
            //        {
            //            pbar.Visibility = ViewStates.Invisible;
            //        });
            //    }
            //    catch (Exception e)
            //    {
            //        Activity.RunOnUiThread(() => tekst.Text += "Nie można nawiązać połązenia, \n" + "\n" + e.Message);
            //    }
            //});
            //th.Start();


            // Create your fragment here  

            return view;
        }

        public void Client_BramaEvent(Stan NowyStan)
        {
            switch (NowyStan)
            {
                case Stan.Otwarta:
                    {
                        Activity.RunOnUiThread(() => info.Text = "Brama jest otwarta");
                        Client.ShowNotify(5, "Brama Wjazdowa", "Brama jest otwarta", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                        break;
                    }
                case Stan.Zamknięta:
                    {
                        Activity.RunOnUiThread(() => info.Text = "Brama jest zamknięta");
                        //Client.ShowNotify(5, "Brama Wjazdowa", "Brama jest zamknięta", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                        break;
                    }
                case Stan.Otwieranie:
                    {
                        Activity.RunOnUiThread(() => info.Text = "Otwieranie bramy...");
                       // Client.ShowNotify(5, "Brama Wjazdowa", "Otwieranie bramy" , "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                        break;
                    }

                case Stan.Zamykanie:
                    {
                        Activity.RunOnUiThread(() => info.Text = "Zamykanie bramy...");
                       // Client.ShowNotify(5, "Brama Wjazdowa", "Zamykanie bramy" , "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                        break;
                    }
                case Stan.NieOtwartaNieZamknięta:
                    {
                        Activity.RunOnUiThread(() => info.Text = "Brama nie jest ani zamknięta, ani otwarta");
                       // Client.ShowNotify(5, "Brama Wjazdowa", "Brama nie jest ani zamknięta, ani otwarta", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                        break;
                    }
                case Stan.CzujnikPrzerwaniaWiązki:
                    {
                        Activity.RunOnUiThread(() => info.Text = "Aktywny czujnik przerwana wiązki IR");
                      //  Client.ShowNotify(5, "Brama Wjazdowa", "Aktywny czujnik przerwana wiązki IR", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                        break;
                    }
                case Stan.Awaria:
                    {
                        Activity.RunOnUiThread(() => info.Text = "Awaria czujników krańcowych");
                      //  Client.ShowNotify(5, "Brama Wjazdowa", "Awaria czujników krańcowych" , "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                        break;
                    }
                case Stan.BrakPolaczenia:
                    {                        
                        Activity.RunOnUiThread(() => info.Text = "Oczekiwanie na połączenie Bramy z serwerem");
                      //  Client.ShowNotify(5, "Brama Wjazdowa", "Oczekiwanie na połączenie Bramy z serwerem", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                        break;
                    }
            }
        }

        /// <summary>
        /// Moduł Brama chce napisać do Serwera
        /// </summary>
        /// <param name="Wartosc">akcja bramy</param>
        static public void BramaWrite(Commands.AkcjaBramy Wartosc)
        {
            Client.Write(Convert.ToString((int)Wartosc), Commands.Moduł.Brama);
        }




        private void Client_ConnectedEvent(bool Connect)
        {
            Activity.RunOnUiThread(() =>
            {
                if (ConnectProgressBar == null)
                {
                    return;
                }
                if (Connect)
                {
                    ConnectProgressBar.Visibility = ViewStates.Invisible;
                }
                else
                {
                    ConnectProgressBar.Visibility = ViewStates.Visible;
                }
            });
        }





        //private void ClientSend(Ster msg)
        //{

        //    try
        //    {
        //        if (!connected)
        //        {

        //            Thread th = new Thread(() =>
        //            {
        //                try
        //                {
        //                    client = new TcpClient("192.168.1.4", 80);
        //                    stream = client.GetStream();

        //                    connected = client.Connected;
        //                    Resp();

        //                    Activity.RunOnUiThread(() =>
        //                    {
        //                        pbar.Visibility = ViewStates.Invisible;
        //                    });

        //                }
        //                catch (Exception e)
        //                {
        //                    Activity.RunOnUiThread(() => tekst.Text += "Nie można nawiązać połązenia, \n" + "\n" + e.Message);
        //                }
        //            });

        //            th.Start();
        //            th.Join();

        //        }

        //        //stream.BeginRead(recvbuf, 0, recvbuf.Length, new AsyncCallback(revived), stream);
        //        //  Toast.MakeText(this, "ss", ToastLength.Short).Show();

        //        byte[] tmp = new byte[1];
        //        tmp[0] = (byte)msg;

        //        client.GetStream().Write(tmp, 0, 1);

        //    }
        //    catch (Exception ex)
        //    {
        //        //connected = false;
        //        tekst.Text += "\n" + ex.Message;
        //    }

        //}

        //void Revived(IAsyncResult a)
        //{            

        //    this.Activity.RunOnUiThread(() =>
        //    {
        //        String tmp = Encoding.ASCII.GetString(recvbuf);
        //        if (String.Compare(tmp, "opning") == 0)
        //        {

        //            info.Text = "Otwieranie bramy...";
        //            pbar.Visibility = ViewStates.Visible;

        //        }
        //        else if (String.Compare(tmp, "cloing") == 0)
        //        {
        //            info.Text = "Zamykanie bramy...";
        //            pbar.Visibility = ViewStates.Visible;

        //        }
        //        else if (String.Compare(tmp, "opened") == 0)
        //        {
        //            info.Text = "Brama jest otwarta";
        //            pbar.Visibility = ViewStates.Invisible;

        //        }
        //        else if (String.Compare(tmp, "closed") == 0)
        //        {
        //            info.Text = "Brama jest zamknięta";
        //            pbar.Visibility = ViewStates.Invisible;

        //        }
        //        else if (String.Compare(tmp, "brokee") == 0)
        //        {
        //            info.Text = "Atywny czujnik przerwana wiązki IR";
        //            pbar.Visibility = ViewStates.Invisible;

        //        }
        //        else if (String.Compare(tmp, "none..") == 0)
        //        {
        //            info.Text = "Brama nie jest ani zamknięta, ani otwarta";
        //            pbar.Visibility = ViewStates.Invisible;
        //        }
        //    });
        //}

        //private void Resp()
        //{
        //    if (connected)
        //    {
        //        //stream = client.GetStream();

        //        new Thread(() =>
        //        {
        //            if (connected)
        //            {
        //                for (;;)
        //                {
        //                    if (stream.CanRead)
        //                    {
        //                        try
        //                        {

        //                            /*RunOnUiThread(() => {*/
        //                            stream.BeginRead(recvbuf, 0, recvbuf.Length, new AsyncCallback(Revived), stream); // });

        //                        }
        //                        catch { }
        //                    }
        //                    Thread.Sleep(10);

        //                }
        //            }
        //        }).Start();
        //    }

        //}

    }
}

