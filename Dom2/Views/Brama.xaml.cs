using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
using Dom.Podzespoły;
using System.Net.Sockets;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Dom.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Brama : Page
    {
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
        


        public Brama()
        {

            this.InitializeComponent();

            //ProgressBar informujący o Połączeniu
            if(Client.Connected)
            {
                ConnectProgressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                Client.ConnectedEvent += Client_ConnectedEvent;
            }       

            Client_BramaEvent(State);

            Client.BramaEvent += Client_BramaEvent;

            BadgeNotify.Clear();

        }

        /// <summary>
        /// Zdarzenie połączenia z serwerem
        /// </summary>
        /// <param name="Connect"> Czy połączono ? </param>
        private void Client_ConnectedEvent(bool Connect)
        {
            if(Connect)
            {
                ConnectProgressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                ConnectProgressBar.Visibility = Visibility.Visible;
            }
        }        


        /// <summary>
        /// Obsługa odpowiedzi z serwera
        /// </summary>
        /// <param name="NowyStan"> Nowy Stan </param>
        private void Client_BramaEvent(Stan NowyStan)
        {
            debugarea.Text += NowyStan.ToString();

            State = NowyStan;

            switch (NowyStan)
            {
                case Stan.Otwarta:
                    {
                        kolko.IsActive = false;
                        //BadgeNotify.Set(BadgeValue.none);
                        //info.Text = "Brama jest otwarta";
                        //if (!first)
                        //{
                        //    ToastNotify.Send("Brama jest otwarta");
                        //    TileNotify.Send("Otwarta brama");
                        //}
                        //first = false;
                        break;
                    }
                case Stan.Zamknięta:
                    {
                        kolko.IsActive = false;
                        //BadgeNotify.Set(BadgeValue.none);
                        //info.Text = "Brama jest zamknięta";
                        //if (!first)
                        //{
                        //    ToastNotify.Send("Brama jest zamknięta");
                        //    TileNotify.Send("Zamknięta brama");
                        //}
                        //first = false;
                        break;
                    }
                case Stan.Otwieranie:
                    {
                        //BadgeNotify.Set(BadgeValue.activity);
                        info.Text = "Otwieranie bramy...";
                        kolko.IsActive = true;
                        break;
                    }
                       
                case Stan.Zamykanie:
                    {
                        //BadgeNotify.Set(BadgeValue.activity);
                        info.Text = "Zamykanie bramy...";
                        kolko.IsActive = true;
                        break;                        
                    }
                case Stan.NieOtwartaNieZamknięta:
                    {
                        kolko.IsActive = false;
                        //BadgeNotify.Set(BadgeValue.none);
                        info.Text = "Brama nie jest ani zamknięta, ani otwarta";
                        break;
                    }
                case Stan.CzujnikPrzerwaniaWiązki:
                    {
                        kolko.IsActive = false;
                        //BadgeNotify.Set(BadgeValue.paused);
                        //info.Text = "Aktywny czujnik przerwana wiązki IR";
                        //if (!first)
                        //{
                        //    ToastNotify.Send("Aktywny czujnik przerwana wiązki IR");
                        //}
                        //first = false;
                        break;
                    }
                case Stan.Awaria:
                    {
                        kolko.IsActive = false;
                        //BadgeNotify.Set(BadgeValue.error);
                        info.Text = "Awaria czujników krańcowych";
                        //ToastNotify.Send("Awaria Bramy");
                        //TileNotify.Send("Brama zgłasza awarię");
                        break;
                    }
                case Stan.BrakPolaczenia:
                    {
                        //BadgeNotify.Set(BadgeValue.error);
                        info.Text = "Oczekiwanie na połączenie Bramy z serwerem";
                        break;
                    }
            }
        }

        private void Otworz_Click(object sender, RoutedEventArgs e)
        {
            Client.BramaSend(Commands.AkcjaBramy.Otwórz);
        }

        private void Zamknij_Click(object sender, RoutedEventArgs e)
        {
            Client.BramaSend(Commands.AkcjaBramy.Zamknij);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Client.BramaSend(Commands.AkcjaBramy.Stop);        
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            ConnectProgressBar.Width = e.NewSize.Width;


            if ((this.Width != 500) & (this.Height != 650))
            {
                this.Width = 400;
                this.Height = 640;
            }
        }


        /// <summary>
        /// Włącznik kontrolki TextBox debugarea
        /// </summary>
        private void Debugswitch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((bool)debugswitch.IsChecked)
            {
                debugarea.Visibility = Visibility.Visible;
            }
            else
            {
                debugarea.Visibility = Visibility.Collapsed;
            }

        }

    }
}
