using System;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Networking.Sockets;
using Windows.Networking;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.System.Profile;

using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

enum Ster : byte { otwórz = 145, zamknij = 167, stop = 155};


namespace Brama_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StreamSocket klient = new StreamSocket();
        bool done = false;
        public MainPage()
        {
            this.InitializeComponent();
            conn();
        }

        private async void conn()
        {
            debugarea.Text += ("Windows " + ((ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion) & 0xFFFF000000000000L) >> 48).ToString() + " (" + AnalyticsInfo.VersionInfo.DeviceFamily + ")" + Environment.NewLine);
            short l = 1;
            done = false;
            for (;;)
            {
                if (!done)
                {
                    kolko.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 174, 0));
                    kolko.IsActive = true;
                    debugarea.Text += "Próba połączenia #" + l + Environment.NewLine;
                    Task hconn;
                    try
                    {
                        hconn = klient.ConnectAsync(new HostName("192.168.1.4"), "80").AsTask();
                        await hconn;
                        //---------------------------------------------//
                        // Albo wyrzuci błąd połączenia albo, przejdzie dalej..       
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            kolko.IsActive = false;
                            kolko.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 147, 219));
                            debugarea.Text = "Połączono";
                            recv();
                        });
                        done = true;
                        return;
                    }
                    catch (Exception e)
                    {
                        debugarea.Text += "Błąd połączenia" + Environment.NewLine + e.Message + Environment.NewLine;
                    }
                    l++;
                }
                await Task.Delay(500);
            }
        }

        private void send(Ster code)
        {
            if (done)
            {
                byte[] tmp = new byte[1];
                tmp[0] = (byte) code;               

                //klient.OutputStream.WriteAsync(ASCIIEncoding.ASCII.GetBytes(code).AsBuffer()).Completed += async delegate (IAsyncOperationWithProgress<uint, uint> asyncAction, AsyncStatus asyncStatus)
                klient.OutputStream.WriteAsync(tmp.AsBuffer()).Completed += async delegate (IAsyncOperationWithProgress<uint, uint> asyncAction, AsyncStatus asyncStatus)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (asyncStatus == AsyncStatus.Error)
                        {
                            debugarea.Text += "Błąd Wysyłania danych" + Environment.NewLine + asyncAction.ErrorCode.ToString() + Environment.NewLine;
                            conn();                   
                        }
                    });
                };
            }
        }

        private async Task recv()
        {
            for (;;)
            {
                IBuffer buf = ASCIIEncoding.ASCII.GetBytes("123456").AsBuffer();
                IAsyncOperationWithProgress<IBuffer, uint> hrecv;
                try
                {
                    hrecv = klient.InputStream.ReadAsync(buf, 6, InputStreamOptions.Partial);
                    //Task aa = hrecv.AsTask();
                    // await aa;

                    await hrecv;

                    // hrecv.Completed += async delegate (IAsyncOperationWithProgress<IBuffer, uint> asyncAction, AsyncStatus asyncStatus)
                    // {

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {


                        //     if (asyncStatus == AsyncStatus.Error)
                        //    {
                        //         debugarea.Text += "Błąd podczas odbioru danych" + Environment.NewLine + asyncAction.ErrorCode.ToString() + Environment.NewLine;
                        //conn();
                        //  }
                        //      if (asyncStatus == AsyncStatus.Completed)
                        //      {
                        String text = Encoding.ASCII.GetString(buf.ToArray());
                        //  debugarea.Text = text;
                        kolko.IsActive = false;
                        if (text == "opning")
                        {
                            info.Text = "Otwieranie bramy...";
                            kolko.IsActive = true;
                        }
                        else if (text == "cloing")
                        {
                            info.Text = "Zamykanie bramy...";
                            kolko.IsActive = true;
                        }
                        else if (text == "opened")
                        {
                            info.Text = "Brama jest otwarta";
                            SendToast("Brama jest otwarta");

                        }
                        else if (text == "closed")
                        {
                            info.Text = "Brama jest zamknięta";
                            SendToast("Brama jest zamknięta");

                        }
                        else if (text == "brokee")
                        {
                            info.Text = "Aktywny czujnik przerwana wiązki IR";

                        }
                        else if (text == "none..")
                        {
                            info.Text = "Brama nie jest ani zamknięta, ani otwarta";
                        }

                        //   }
                    });
                    //  };

                    //   await Task.Delay(1);
                }
                catch (Exception e)
                {
                    debugarea.Text += "Błąd połączenia" + Environment.NewLine + e.Message + Environment.NewLine;
                }
            }

        }

        private void otworz_Click(object sender, RoutedEventArgs e)
        {
            send(Ster.otwórz);
        }

        private void zamknij_Click(object sender, RoutedEventArgs e)
        {
            send(Ster.zamknij);
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            send(Ster.stop);
        }

        private void SendToast(string msg)
        {

            String xmlToastTemplate = "<toast launch=\"app-defined-string\">" +
                         "<visual>" +
                           "<binding template =\"ToastGeneric\">" +
                             "<text>Bram wjazdowa</text>" +
                             "<text>" +
                               msg +
                             "</text>" +
                              "<image placement=\"appLogoOverride\" hint-crop=\"circle\" src=\"Assets\\StoreLogo.png\"/> " +
                           "</binding>" +
                         "</visual>" +
                       "</toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(xmlToastTemplate);
            var Toastnotify = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(Toastnotify);




        }
    }
}
