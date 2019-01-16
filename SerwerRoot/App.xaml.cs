using SerwerRoot.Podzespoły;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SerwerRoot
{
    /// <summary>
    /// Zapewnia zachowanie specyficzne dla aplikacji, aby uzupełnić domyślną klasę aplikacji.
    /// </summary>
    sealed public partial class App : Application
    {

        Dictionary<int, Client> AllClients = new Dictionary<int, Client>();
        private TcpListener Serwer = new TcpListener(IPAddress.Parse(Dom_Background.Data.SerwerIp), Dom_Background.Data.SerwerPort);

        static public Dictionary<ModulesId, ModuleBody> Modules = new Dictionary<ModulesId, ModuleBody>();
        
        public enum ModulesId : short
        {
            Brama,
            Biurko,
        }

        /// <summary>
        /// Inicjuje pojedynczy obiekt aplikacji. Jest to pierwszy wiersz napisanego kodu
        /// wykonywanego i jest logicznym odpowiednikiem metod main() lub WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
           
            // Debug Logi
            Log.BeginAsync();

            Modules.Add(Brama.id, new Brama());
            Modules.Add(Biurko.id, new Biurko());





            // Event do oczyszczenia klienta
            Client.EndConnection += Client_EndConnection;

            //Start modułów
            Event.Begin();


            //Start serwera
            try
            {
                Serwer.Start();
                SerwerWorkAsync();
                Log.Write("Uruchomiono Serwer");
                System.Diagnostics.Debug.WriteLine("Uruchomiono Serwer");
            }
            catch
            {
                Log.Write("Błąd podczas uruchamiania serwera");
                System.Diagnostics.Debug.WriteLine("Błąd podczas uruchamiania serwera");
            }

        }


        public async void SerwerWorkAsync()
        {
            while (true)
            {
                try
                {
                    TcpClient _Client = await Serwer.AcceptTcpClientAsync();
                    int FreeIndex = 0;
                    while (true)
                    {
                        if (!AllClients.ContainsKey(FreeIndex))
                        {
                            AllClients.Add(FreeIndex, new Client(_Client, FreeIndex));
                            Log.Write("Nowy klient id: " + FreeIndex);
                            break;
                        }
                        FreeIndex++;
                    }
                }
                catch (Exception e)
                {
                    Log.Write(e);
                }
            }
        }

        private void Client_EndConnection(int id)
        {
            if (AllClients.ContainsKey(id))
            {
                AllClients[id].Close();
                AllClients[id] = null;
                AllClients.Remove(id);
            }
        }



        /// <summary>
        /// Wywoływane, gdy aplikacja jest uruchamiana normalnie przez użytkownika końcowego. Inne punkty wejścia
        /// będą używane, kiedy aplikacja zostanie uruchomiona w celu otworzenia określonego pliku.
        /// </summary>
        /// <param name="e">Szczegóły dotyczące żądania uruchomienia i procesu.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Nie powtarzaj inicjowania aplikacji, gdy w oknie znajduje się już zawartość,
            // upewnij się tylko, że okno jest aktywne
            if (rootFrame == null)
            {
                // Utwórz ramkę, która będzie pełnić funkcję kontekstu nawigacji, i przejdź do pierwszej strony
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Załaduj stan z wstrzymanej wcześniej aplikacji
                }

                // Umieść ramkę w bieżącym oknie
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Kiedy stos nawigacji nie jest przywrócony, przejdź do pierwszej strony,
                    // konfigurując nową stronę przez przekazanie wymaganych informacji jako
                    // parametr
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Upewnij się, ze bieżące okno jest aktywne
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Wywoływane, gdy nawigacja do konkretnej strony nie powiedzie się
        /// </summary>
        /// <param name="sender">Ramka, do której nawigacja nie powiodła się</param>
        /// <param name="e">Szczegóły dotyczące niepowodzenia nawigacji</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Wywoływane, gdy wykonanie aplikacji jest wstrzymywane. Stan aplikacji jest zapisywany
        /// bez wiedzy o tym, czy aplikacja zostanie zakończona, czy wznowiona z niezmienioną zawartością
        /// pamięci.
        /// </summary>
        /// <param name="sender">Źródło żądania wstrzymania.</param>
        /// <param name="e">Szczegóły żądania wstrzymania.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Zapisz stan aplikacji i zatrzymaj wszelkie aktywności w tle
            deferral.Complete();
        }
    }
}
