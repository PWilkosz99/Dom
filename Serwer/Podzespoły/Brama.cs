using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;

namespace Serwer.Podzespoły
{
    class Brama
    {
        /// <summary>
        /// Możliwe stany bramy
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
        /// Zmienna przechowująca stany bramy
        /// </summary>
        static private Stan State = Stan.BrakPolaczenia;
        static public Stan GetState => State;
             
        /// <summary>
        /// Kody komunikacyjne do bramy
        /// </summary>
        private enum Ster : byte { otwórz = 145, zamknij = 167, stop = 155 };

        /// <summary>
        /// Kto coś chciał
        /// </summary>
        static public Commands.Podmiot PodmiotValue = Commands.Podmiot.Niezidentyfikowany;

        /// <summary>
        /// Adres Ip
        /// </summary>
        static private string Adress { get { return "192.168.1.4"; } }

        /// <summary>
        /// Port
        /// </summary>
        static private int Port { get { return 80; } }

        static private TcpClient Client = new TcpClient();
        static  NetworkStream Stream;

        /// <summary>
        /// Zgłoszenie do wysłania danych
        /// </summary>
        /// <param name="NowyStan"></param>
        public delegate void BramaWriteEventHandler(Stan AktualnyStanBramy);
        public static event BramaWriteEventHandler BramaWrite;

        /// <summary>
        /// Blokada przed czynnościami na stream ie podczas pracy w funkcji BeginAsync
        /// </summary>
        static ManualResetEvent BeginWorker = new ManualResetEvent(false);

        /// <summary>
        /// Połącz z bramą WiFi
        /// </summary>
        static public async void BeginAsync()
        {            
            try
            {
                Client = new TcpClient();

                if (Client.Connected)
                {
                    return;
                }

                SetState(Stan.BrakPolaczenia);
                Log.Write("Brama Begin");
                Debug.WriteLine("Łączenie z bramą");

                await Client.ConnectAsync(Adress, Port);

                Log.Write("Połączono z bramą");
                Debug.WriteLine("Połączono z bramą");
                Stream = Client.GetStream();
                ReciveAsync();
                LikeKeepAliveAsync();
            }
            catch (Exception e)
            {
                Log.Write(e);
                BeginAsync();
            }            
        }

        /// <summary>
        /// Obsługa komunikatów przychodzących z bramy
        /// </summary>
        static private async void ReciveAsync()
        {
            while (true)
            {
                try
                {
                    byte[] MyReadBuffer = new byte[1];

                    await Stream.ReadAsync(MyReadBuffer, 0, MyReadBuffer.Length);

                    string Text = Encoding.ASCII.GetString(MyReadBuffer);

                    if (Text == "p")
                    {
                        Brama.PodmiotValue = Commands.Podmiot.Przycisk;
                    }
                    else if (Text == "r")
                    {
                        Brama.PodmiotValue = Commands.Podmiot.Pilot;
                    }

                    byte[] myReadBuffer = new byte[1];

                    await Stream.ReadAsync(myReadBuffer, 0, myReadBuffer.Length);

                    string text = Encoding.ASCII.GetString(myReadBuffer);

                    if (text == "a")
                    {
                        SetState(Stan.Otwieranie);
                    }
                    else if (text == "b")
                    {
                        SetState(Stan.Zamykanie);
                    }
                    else if (text == "c")
                    {
                        SetState(Stan.Otwarta);
                    }
                    else if (text == "d")
                    {
                        SetState(Stan.Zamknięta);
                    }
                    else if (text == "e")
                    {
                        SetState(Stan.CzujnikPrzerwaniaWiązki);
                    }
                    else if (text == "f")
                    {
                        SetState(Stan.NieOtwartaNieZamknięta);
                    }
                    else if (text == "g")
                    {
                        SetState(Stan.Awaria);
                    }
                }
                catch (Exception e)
                {
                    Log.Write(e);
                    BeginAsync();
                    return;
                }
            }
        }

        /// <summary>
        /// Zmiana stanu bramy
        /// </summary>
        /// <param name="value"></param>
        static private void SetState(Stan value)
        {
            if (State != value)
            {
                State = value;
                BramaWrite?.Invoke(State);
                ZmianaStanu(State);
            }
        }

        /// <summary>
        /// Zmień stan na bramie
        /// </summary>
        /// <param name="code"></param>
        static private void SendToDevice(Ster code)
        {
            try
            {
                if (Client.Connected)
                {
                    Stream.WriteByte((byte)code);
                }
            }
            catch (Exception e)
            {
                Log.Write(e);
            }
        }

        /// <summary>
        /// Przetwórz surowe dane
        /// </summary>
        /// <param name="akcja"></param>
        /// <param name="podmiot"></param>
        static public void PrzetworzZadanie(String[] Code)
        {
            PodmiotValue = (Commands.Podmiot)int.Parse(Code[0]);
            Commands.AkcjaBramy akcja = (Commands.AkcjaBramy)int.Parse(Code[2]);
            Ster tmp = Ster.stop;

            switch (akcja)
            {
                case Commands.AkcjaBramy.Otwórz:
                    {
                        tmp = Ster.otwórz;
                        break;
                    }
                case Commands.AkcjaBramy.Stop:
                    {
                        tmp = Ster.stop;
                        break;
                    }
                case Commands.AkcjaBramy.Zamknij:
                    {
                        tmp = Ster.zamknij;
                        break;
                    }
                case Commands.AkcjaBramy.Stan:
                    {
                        BramaWrite?.Invoke(State);
                        return;
                    }
            }

            SendToDevice(tmp);
        }
        
        /// <summary>
        /// Sprawdza połączenie
        /// </summary>
        private static async void LikeKeepAliveAsync()
        {
            try
            {
                while (true)
                {
                    await Task.Delay(10000);
                    await Client.GetStream().WriteAsync(new byte[] { 1 }, 0, 1 );                    
                }
            }
            catch
            {
                Debug.WriteLine("Utracono połączenie z bramą");
                Log.Write("Utracono połączenie z bramą");
                BeginAsync();
                return;
            }
        }

        /// <summary>
        /// Zapisz zminę stanu do bazdy danych SQL
        /// </summary>
        /// <param name="NowyStan"></param>
        private static void ZmianaStanu(Brama.Stan NowyStan)
        {
            string Podmiot = Brama.PodmiotValue.ToString();

            switch (NowyStan)
            {
                case Brama.Stan.Otwarta:
                    {
                        Event.Write("Brama Wjazdowa", Brama.PodmiotValue.ToString(), "Otwrto Bramę", "Brama została otwarta");
                        break;
                    }
                case Brama.Stan.Zamknięta:
                    {
                        Event.Write("Brama Wjazdowa", Brama.PodmiotValue.ToString(), "Zamknięto Bramę", "Brama została zamknięta");
                        break;
                    }
                case Brama.Stan.Otwieranie:
                    {
                        break;
                    }
                case Brama.Stan.Zamykanie:
                    {
                        break;
                    }
                case Brama.Stan.NieOtwartaNieZamknięta:
                    {
                        break;
                    }
                case Brama.Stan.CzujnikPrzerwaniaWiązki:
                    {
                        break;
                    }
                case Brama.Stan.Awaria:
                    {
                        Event.Write("Brama Wjazdowa", "Awaria", "Brama zgłasza awarię");
                        break;
                    }
            }
        }
    }
}