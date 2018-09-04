using Android.App;
using Android.Net;
using Android.Net.Wifi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;

namespace Dom_android
{
    static class Client
    {
        static NetworkStream Stream;
        static TcpClient client =  new TcpClient();


        public delegate void BramaEventHandler(Brama.Stan NowyStan);
        public static event BramaEventHandler BramaEvent;

        public delegate void BiurkoEventHandler(String[] Code);
        public static event BiurkoEventHandler BiurkoEvent;

        public delegate void ConnectedEventHandler(bool Connect);
        public static event ConnectedEventHandler ConnectedEvent;

        public static bool Connected { get { return client.Connected; } }

        private static void ClientBeginFunc()
        {
            while (true)
            {
                if (client != null)
                {
                    if (client.Connected)
                    {
                        return;
                    }
                }


                //ConnectivityManager connectivityManager = (ConnectivityManager) Application.Context.GetSystemService(ConnectivityService);
                try
                {
                    client = new TcpClient();
                    IAsyncResult result = client.BeginConnect("192.168.1.8", 80, null, null);
                    bool succss = result.AsyncWaitHandle.WaitOne(5000, true);

                    if (client.Connected)
                    {
                        client.EndConnect(result);
                        Debug.WriteLine("Połączono z serwerem");
                        ConnectedEvent?.Invoke(true);
                        Reciver();
                        Ask();
                        return;
                    }
                    else
                    {
                        client.Close();
                        client = null;
                        throw new SocketException();  // connection timed out
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Client.Begin() - error");
                    ConnectedEvent?.Invoke(false);
                    string SSID = (((WifiManager)Application.Context.GetSystemService(Android.Content.Context.WifiService)).ConnectionInfo.SSID);
                    if (!SSID.Contains("Home_WiFi"))
                    {
                        Console.WriteLine("Połącz się z właściwą siecią - ClientBegin");
                        Thread.Sleep(1000);
                        return;
                    }
                }
            }
        }

        private static void Ask()
        {
            Brama.BramaWrite(Commands.AkcjaBramy.Stan);
        }

        static Thread ClientBeginTh = new Thread(new ThreadStart(ClientBeginFunc));

        public static void Begin()
        {

            if (ClientBeginTh.IsAlive)
            {
                return;
            }

            ClientBeginTh = null;
            ClientBeginTh = new Thread(new ThreadStart(ClientBeginFunc)); ;

          
            try
            {
              
                ClientBeginTh.Start();
            }
            catch { }
        }






        /// <summary>
        /// Obsługa komunikacji przychodzącej
        /// </summary>
        /// <returns></returns>
        static private void Reciver()
        {
            new Thread(new ThreadStart(async () =>
           {
               while (true)
               {
                   try
                   {
                       byte[] lenghtbuf = new byte[3];

                       Stream = client.GetStream();

                       await Stream.ReadAsync(lenghtbuf, 0, 3);

                       int Lenght;

                       if (int.TryParse(Encoding.ASCII.GetString(lenghtbuf), out Lenght))
                       {
                           byte[] myReadBuffer = new byte[Lenght];

                           await Stream.ReadAsync(myReadBuffer, 0, myReadBuffer.Length);

                           String text = Encoding.ASCII.GetString(myReadBuffer);

                           Wykonaj(Crypt.DecryptAes(text));
                       }
                   }

                   catch (Exception e)
                   {
                       Debug.WriteLine(e.Message);
                       Begin();
                       return;

                   }
               }
           })).Start();
        }


        //static void Wykonaj(string ok)
        //{
        //    Debug.WriteLine(ok);
        //}

        /// <summary>
        /// Rozkodowanie komunikacji przychodzącej
        /// </summary>
        /// <param name="Code">Surowe dane</param>
        static private void Wykonaj(String Code)
        {
            new Thread(new ThreadStart(async () =>
            {
                String[] _Code = Code.Split(new char[] { ';' });

                Debug.WriteLine("Pisze --> " + Code);

              

                Commands.Moduł Command = (Commands.Moduł)int.Parse(_Code[0]);

                switch (Command)
                {
                    case Commands.Moduł.Brama:
                        {
                            Brama.Stan stan = (Brama.Stan)(int.Parse(_Code[1]));
                            BramaEvent?.Invoke(stan);
                            switch (stan)
                            {
                                case Brama.Stan.Otwarta:
                                    {
                                        ShowNotify(5, "Brama Wjazdowa", "Brama jest otwarta", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                                        break;
                                    }
                                case Brama.Stan.Zamknięta:
                                    {
                                        ShowNotify(5, "Brama Wjazdowa", "Brama jest zamknięta", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                                        break;
                                    }
                                case Brama.Stan.Otwieranie:
                                    {
                                        ShowNotify(5, "Brama Wjazdowa", "Otwieranie bramy", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                                        break;
                                    }

                                case Brama.Stan.Zamykanie:
                                    {
                                        ShowNotify(5, "Brama Wjazdowa", "Zamykanie bramy", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                                        break;
                                    }
                                case Brama.Stan.NieOtwartaNieZamknięta:
                                    {
                                        ShowNotify(5, "Brama Wjazdowa", "Brama nie jest ani zamknięta, ani otwarta", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                                        break;
                                    }
                                case Brama.Stan.CzujnikPrzerwaniaWiązki:
                                    {
                                        ShowNotify(5, "Brama Wjazdowa", "Aktywny czujnik przerwana wiązki IR", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                                        break;
                                    }
                                case Brama.Stan.Awaria:
                                    {
                                        ShowNotify(5, "Brama Wjazdowa", "Awaria czujników krańcowych", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                                        break;
                                    }
                                case Brama.Stan.BrakPolaczenia:
                                    {
                                        ShowNotify(5, "Brama Wjazdowa", "Oczekiwanie na połączenie Bramy z serwerem", "Brama Wjazdowa", NotificationImportance.Default, NotificationVisibility.Public);
                                        break;
                                    }
                            }
                            break;
                        }
                    case Commands.Moduł.Biurko:
                        {
                            BiurkoEvent?.Invoke(_Code);
                            ShowNotify(4, "Aplikacja Dom", Code, "RawData", NotificationImportance.Low, NotificationVisibility.Public);
                            // Biurko.PrzetworzZadanie(_Code, ID);
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine("Ppoza zasięgiem komendy");
                            return;
                        }
                }
            })).Start();
        }

        /// <summary>
        /// Pisz do klienta
        /// </summary>
        /// <param name="Text">Surowe dane</param>
        /// <param name="okimmowa">Moduł</param>
        static public void Write(String Text, Commands.Moduł okimmowa)
        {
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    string _Text = ((int)Commands.Podmiot.Telefon) + ";" + ((int)okimmowa).ToString() + ";" + Text;

                    String text = Crypt.EncryptAes(_Text);

                    Stream.Write(Encoding.ASCII.GetBytes(text.Length.ToString("D3") + text), 0, text.Length + 3);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return;
                }
            })).Start();
        }



 


       



        /// <summary>
        /// Funkcja pokazująca powiadomienia
        /// </summary>
        /// <param name="NotifyID">ID powiadomienia</param>
        /// <param name="NotifyTitle">Tytuł powiadomienia</param>
        /// <param name="Notifytext">Zawartość<param>
        /// <param name="ChannelName">Nazwa kanału powiadomień</param>
        /// <param name="notificationImportance">Ważność powiadomienia</param>
        /// <param name="notificationVisibility">Widoczność dla innych powiadomienia</param>
        public static void ShowNotify(int NotifyID, string NotifyTitle, string NotifyText, string ChannelName, NotificationImportance notificationImportance, NotificationVisibility notificationVisibility)
        {
            new Thread(new ThreadStart(() =>
            {
                string URGENT_CHANNEL = ChannelName;
                NotificationManager notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);


                // Work has finished, now dispatch anotification to let the user know.
                Notification.Builder notificationBuilder = new Notification.Builder(Application.Context)
                    .SetSmallIcon(Resource.Drawable.Icon)
                    .SetContentTitle(NotifyTitle)
                    .SetContentText(NotifyText);


                try
                {
                    // Avaliable in android 8.0
                    NotificationChannel chan = new NotificationChannel(URGENT_CHANNEL, ChannelName, notificationImportance);
                    chan.EnableVibration(true);
                    chan.LockscreenVisibility = notificationVisibility;
                    notificationManager.CreateNotificationChannel(chan);
                    notificationBuilder.SetChannelId(URGENT_CHANNEL);
                }
                catch { }

                //var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.Notify(NotifyID, notificationBuilder.Build());
            })).Start();
        }
        
    }

}
