using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Serwer.Podzespoły;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace Serwer
{
    class Main
    {
        TcpListener Serwer = new TcpListener(IPAddress.Parse("192.168.1.8"), 80);
           
        public Main()
        {
            Serwer.Start();
            Log.Write("Uruchomiono Serwer");
            Debug.WriteLine("Uruchomiono Serwer");

            Biurko.ConnectAsync().Wait();

            Event.Connect();

            Brama.ZmianaStanu += Brama_ZmianaStanu;

            Brama.ConnectAsync();


            SerwerWorkAsync().Wait();

        }

        private void Brama_ZmianaStanu(Brama.Stan NowyStan)
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
                        Event.Write("Brama Wjazdowa","Awaria", "Brama zgłasza awarię");
                        break;
                    }                
            }
        }

        private async Task SerwerWorkAsync()
        {
            while (true)
            {
                TcpClient _Client = await Serwer.AcceptTcpClientAsync();

                Client CCLient = new Client(_Client);
                
            }
        }                      
    }
}
