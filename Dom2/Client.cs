using Dom.Podzespoły;
using Dom.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Dom_Background;

namespace Dom
{
    static class Client
    {
        static TcpClient client = new TcpClient();

        public delegate void BramaEventHandler (Brama.Stan NowyStan);
        public static event BramaEventHandler BramaEvent;

        public delegate void BiurkoEventHandler(String [] Code);
        public static event BiurkoEventHandler BiurkoEvent;

        public delegate void ConnectedEventHandler(bool Connect);
        public static event ConnectedEventHandler ConnectedEvent;

        public static bool Connected { get { return client.Connected; } }

        static NetworkStream Stream;

        static bool connecting = false;

        /// <summary>
        /// Nawiązanie połączenia z serwerem
        /// </summary>
        public static async Task ConnectAsnc()
        {
            if (!connecting)
            {
                connecting = true;
                while (!client.Connected)
                {
                    ConnectedEvent?.Invoke(false);
                    try
                    {
                        await client.ConnectAsync(Data.SerwerIp, Data.SerwerPort);
                        Stream = client.GetStream();
                        ReviveAsync();
                        Debug.WriteLine("Nawiązano połączenie z Serwerem");
                        ConnectedEvent?.Invoke(true);
                        connecting = false;

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        await ConnectAsnc();
                    }
                }
            }
        }

        /// <summary>
        /// Obsługa komunikatów z serwera
        /// </summary>
        private static async void ReviveAsync()
        {    
            try
            {
                while (true)
                {
                    // Pobierz rozmiar zaszyfrowanego tetsku
                    byte[] lenghtbuf = new byte[3];

                    await Stream.ReadAsync(lenghtbuf, 0, 3);

                    int Lenght;

                    // Spróbuj przekonwertować to na Liczb e całkowitą
                    if (int.TryParse(Encoding.ASCII.GetString(lenghtbuf), out Lenght))
                    {
                        byte[] myReadBuffer = new byte[Lenght];

                        // Odbierz dane o rozmiarze Lenght
                        await Stream.ReadAsync(myReadBuffer, 0, myReadBuffer.Length);

                        // Odszyfruj
                        String data = Crypt.DecryptAes(Encoding.ASCII.GetString(myReadBuffer));

                        Debug.WriteLine(data);

                        // Przkonwertuj to na tablicę komunikatów o stałych indeksach
                        String[] Code = data.Split(new char[] { ';' });

                        Commands.Moduł Command = (Commands.Moduł)int.Parse(Code[0]);

                        // Przetwórz dane w każdym module

                        switch (Command)
                        {
                            case Commands.Moduł.Brama:
                                {
                                    BramaEvent?.Invoke((Brama.Stan)int.Parse(Code[1]));
                                   // Brama.State = (Brama.Stan)int.Parse(Code[1]);
                                    break;
                                }
                            case Commands.Moduł.Biurko:
                                {
                                    BiurkoEvent?.Invoke(Code);
                                    //Dom.Podzespoły.Biurko.SyncResp(Code);
                                    break;
                                }
                            default:
                                {
                                    Debug.WriteLine("Nieprawidłowa Komenda");
                                    break;
                                }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }


        /// <summary>
        /// Wysyłanie żadań bramy
        /// </summary>
        /// <param name="Czynnosc">Co chcesz zrobić </param>
        public static void BramaSend(Commands.AkcjaBramy Czynnosc)
        {
            Send(((int)Commands.Moduł.Brama).ToString() + ";" + ((int)Czynnosc).ToString());           
        }

        public static void BiurkoSend(string Text)
        {
            Send(Text);
        }           

        /// <summary>
        /// Procedura komunikacyjna z serwerem
        /// </summary>
        /// <param name="TextToSend">Przygotowany text </param>
        private static void Send(string TextToSend)
        {
            if (client.Connected)
            {
                //Przygotowanie Odpowiednich danych
                String coded = Crypt.EncryptAes( ((int)Commands.Podmiot.PC).ToString() + ";" + TextToSend );
                //Wysłanie danych wraz z trójcyfrową informacją o rozmiarze zakodowanej informacji
                Stream.Write(Encoding.ASCII.GetBytes(coded.Length.ToString("D3") + coded), 0, coded.Length + 3);
            }
            else
            {
                ConnectAsnc();
            }

        }

    }
}
