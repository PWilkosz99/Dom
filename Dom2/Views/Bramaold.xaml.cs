using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
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

using System.Text;


enum Ster : byte { otwórz = 145, zamknij = 167, stop = 155 };

namespace Dom.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Bramaold : Page
    {
        StreamSocket klient = new StreamSocket();
        bool done = false;

        public Bramaold()
        {
            this.InitializeComponent();
        }
        private async void Conn()
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
                        klient.Control.KeepAlive = true;   
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

        private void Send(Ster code)
        {
            if (done)
            {
                byte[] tmp = new byte[1];
                tmp[0] = (byte)code;

                //klient.OutputStream.WriteAsync(ASCIIEncoding.ASCII.GetBytes(code).AsBuffer()).Completed += async delegate (IAsyncOperationWithProgress<uint, uint> asyncAction, AsyncStatus asyncStatus)
                klient.OutputStream.WriteAsync(tmp.AsBuffer()).Completed += async delegate (IAsyncOperationWithProgress<uint, uint> asyncAction, AsyncStatus asyncStatus)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (asyncStatus == AsyncStatus.Error)
                        {
                            debugarea.Text += "Błąd Wysyłania danych" + Environment.NewLine + asyncAction.ErrorCode.ToString() + Environment.NewLine;
                            Conn();
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


                        if (text == "opning")
                        {
                            BadgeNotify.Set(BadgeValue.activity);
                            info.Text = "Otwieranie bramy...";
                            kolko.IsActive = true;
                        }
                        else if (text == "cloing")
                        {
                            BadgeNotify.Set(BadgeValue.activity);
                            info.Text = "Zamykanie bramy...";
                            kolko.IsActive = true;
                        }
                        else if (text == "opened")
                        {
                            kolko.IsActive = false;
                            BadgeNotify.Set(BadgeValue.none);
                            info.Text = "Brama jest otwarta";
                            SendToast("Brama jest otwarta");
                            TileNotify.Send("Otwarta brama");
                        }
                        else if (text == "closed")
                        {
                            kolko.IsActive = false;
                            BadgeNotify.Set(BadgeValue.none);
                            info.Text = "Brama jest zamknięta";
                            SendToast("Brama jest zamknięta");
                            TileNotify.Send("Zamknięta brama");

                        }
                        else if (text == "brokee")
                        {
                            kolko.IsActive = false;
                            BadgeNotify.Set(BadgeValue.error);
                            info.Text = "Aktywny czujnik przerwana wiązki IR";
                        }
                        else if (text == "none..")
                        {
                            kolko.IsActive = false;
                            BadgeNotify.Set(BadgeValue.none);
                            info.Text = "Brama nie jest ani zamknięta, ani otwarta";
                        }

                        //   }
                    });
                      //};

                    //   await Task.Delay(1);
                }
                catch (Exception e)
                {
                    debugarea.Text += "Błąd połączenia" + Environment.NewLine + e.Message + Environment.NewLine;
                }
            }

        }

        private void Otworz_Click(object sender, RoutedEventArgs e)
        {
            Send(Ster.otwórz);
        }

        private void Zamknij_Click(object sender, RoutedEventArgs e)
        {
            Send(Ster.zamknij);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Send(Ster.stop);
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

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((this.Width != 500) & (this.Height != 650))
            {
                this.Width = 400;
                this.Height = 640;
            }
        }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Conn();
        }
    }
}
