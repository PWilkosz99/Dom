using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SerwerRoot.Podzespoły;
using System.Diagnostics;

namespace SerwerRoot
{
     class Client
    {
        NetworkStream Stream;
        Socket client;

        /// <summary>
        /// Id klienta do poźniejszego usunięcia z kolejki
        /// </summary>
        private int ID;

        /// <summary>
        /// Koniec pracy z klientem
        /// </summary>
        public static event EndConnectionEventHandler EndConnection;
        public delegate void EndConnectionEventHandler(int Id);

        /// <summary>
        /// Klasa obsługi klienta UI
        /// </summary>
        /// <param name="HClient">Klasa Tyou TcpClient</param>
        /// <param name="id">Numer Id Klienta</param>
        public Client(TcpClient HClient, int id)
        {
            ID = id;

            client = HClient.Client;

            Stream = HClient.GetStream();

            Brama.BramaWrite += BramaWrite;

            Biurko.BiurkoWrite += BiurkoWrite;

            ReciverAsync();
            Debug.WriteLine("New Client with id: " + ID);
            Log.Write("Nowy Klient id: " + ID);
        }

        /// <summary>
        /// Obsługa komunikacji przychodzącej
        /// </summary>
        /// <returns></returns>
        private async Task ReciverAsync()
        {
            while (true)
            {
                try
                {
                    byte[] lenghtbuf = new byte[3];

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
                    Debug.WriteLine(ID + ":" + e.Message);
                    EndConnection?.Invoke(ID);
                    return;

                }
            }
                
            
                
        }

        /// <summary>
        /// Rozkodowanie komunikacji przychodzącej
        /// </summary>
        /// <param name="Code">Surowe dane</param>
        private void Wykonaj(String Code)
        {
            String[] _Code = Code.Split(new char[] { ';' });

            Debug.WriteLine("ID:" +ID + ", pisze --> " + Code);

            Commands.Moduł Command = (Commands.Moduł)int.Parse(_Code[1]);            

            switch (Command)
            {
                case Commands.Moduł.Brama:
                    {
                        Brama.PrzetworzZadanie(_Code);
                        break;
                    }
                case Commands.Moduł.Biurko:
                    {
                        Biurko.PrzetworzZadanie(_Code, ID);
                        break;
                    }
                default:
                    {
                        Debug.WriteLine(ID + ": poza zasięgiem komendy");
                        EndConnection?.Invoke(ID);
                        return;
                    }
            }

        }

        /// <summary>
        /// Pisz do klienta
        /// </summary>
        /// <param name="Text">Surowe dane</param>
        /// <param name="okimmowa">Moduł</param>
        private void Write(String Text, Commands.Moduł okimmowa)
        {
            try
            {
                string _Text = ((int)okimmowa).ToString() + ";" + Text;

                String text = Crypt.EncryptAes(_Text);

                Stream.Write(Encoding.ASCII.GetBytes(text.Length.ToString("D3") + text), 0, text.Length + 3);
            }
            catch (Exception e)
            {
                Debug.WriteLine(ID + ":" + e.Message);
                EndConnection?.Invoke(ID);
                return;
            }
        }

        /// <summary>
        /// Moduł Brama chce napisać do UI
        /// </summary>
        /// <param name="Wartosc">Nowy stan bramy</param>
        private void BramaWrite(Brama.Stan Wartosc)
        {
            Write(Convert.ToString((int)Wartosc), Commands.Moduł.Brama);
        }

        /// <summary>
        /// Moduł biurko chce wysłać zaaktualizowany stan oświetlenia oprócz nr. ID który zlecił to zadanie (pętla nieskończona)
        /// </summary>
        /// <param name="Text">Surowe dane</param>
        /// <param name="ExceptID">ID do ominięcia</param>
        private void BiurkoWrite(string Text, int ExceptID)
        {
            if (ID == ExceptID)
            {
                return;
            }
            Write(Text, Commands.Moduł.Biurko);
        }
        
        /// <summary>
        /// Oczyszczanie
        /// </summary>
        public void Close()
        {
            Brama.BramaWrite -= BramaWrite;
            Biurko.BiurkoWrite -= BiurkoWrite;
            Stream.Dispose();
            client.Dispose();
        }
    }
}