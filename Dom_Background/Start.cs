using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using static Dom_Background.Commands;

namespace Dom_Background
{
    public sealed class Start : IBackgroundTask
    {
        static Windows.Foundation.Collections.IPropertySet AppData = Windows.Storage.ApplicationData.Current.RoamingSettings.Values;  // Access roaming chce
        private const string socketId = "SampleSocket";
        string hostname = "192.168.1.8";
        string port = "80";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {



           // TileNotify.Send("Działam");

            var deferral = taskInstance.GetDeferral();
            try
            {
                var details = taskInstance.TriggerDetails as SocketActivityTriggerDetails;
                var socketInformation = details.SocketInformation;
                switch (details.Reason)
                {
                    case SocketActivityTriggerReason.SocketActivity:
                        var socket = socketInformation.StreamSocket;
                        DataReader reader = new DataReader(socket.InputStream);
                        reader.InputStreamOptions = InputStreamOptions.Partial;
                        await reader.LoadAsync(250);
                        var dataString = reader.ReadString(reader.UnconsumedBufferLength);

                        GetData(dataString);

                        socket.TransferOwnership(socketInformation.Id);
                        break;
                    case SocketActivityTriggerReason.KeepAliveTimerExpired:
                        socket = socketInformation.StreamSocket;
                        DataWriter writer = new DataWriter(socket.OutputStream);
                        writer.WriteBytes(Encoding.UTF8.GetBytes("Keep alive"));
                        await writer.StoreAsync();
                        writer.DetachStream();
                        writer.Dispose();
                        socket.TransferOwnership(socketInformation.Id);
                        break;
                    case SocketActivityTriggerReason.SocketClosed:
                        socket = new StreamSocket();
                        socket.EnableTransferOwnership(taskInstance.Task.TaskId, SocketActivityConnectedStandbyAction.Wake);
                        await socket.ConnectAsync(new Windows.Networking.HostName(hostname), port);
                        socket.TransferOwnership(socketId);
                        break;
                    default:
                        break;
                }
                deferral.Complete();
            }
            catch (Exception exception)
            {
                ToastNotify.Send(exception.Message);
                deferral.Complete();
            }
           // ToastNotify.Send("Kończę");
        }
        

        void GetData(string RawData)
        {
            int lenght = 0;
            try
            {
                lenght = int.Parse(RawData.Substring(0, 3));
            }
            catch
            {

            }
           // ToastNotify.Send(RawData);

            string Data = Crypt.DecryptAes(RawData.Substring(3, lenght));


            String[] Code = Data.Split(new char[] { ';' });

            Commands.Command Command = (Commands.Command)int.Parse(Code[0]);

            // Przetwórz dane w każdym module

            switch (Command)
            {
                case Commands.Command.Brama:
                    {
                        Commands.StanBramy NowyStan = (Commands.StanBramy)int.Parse(Code[1]);   
                        switch (NowyStan)
                        {
                            case StanBramy.Otwarta:
                                {
                                    BadgeIndexUp();
                                    ToastNotify.Send("Brama jest otwarta");
                                    TileNotify.Send("Brama jest otwarta");
                                    BadgeNotify.Set(BadgeValue.none);
                                    break;
                                }
                            case StanBramy.Zamknięta:
                                {
                                    BadgeIndexUp();
                                    ToastNotify.Send("Brama jest zamknięta");
                                    TileNotify.Send("Brama jest zamknięta");
                                    BadgeNotify.Set(BadgeValue.none);
                                    break;
                                }
                            case StanBramy.Otwieranie:
                                {
                                    BadgeNotify.Set(BadgeValue.activity);
                                    break;
                                }

                            case StanBramy.Zamykanie:
                                {
                                    BadgeNotify.Set(BadgeValue.activity);
                                    break;
                                }
                            case StanBramy.NieOtwartaNieZamknięta:
                                {
                                    BadgeIndexUp();
                                    ToastNotify.Send("Brama nie jest ani zamknięta, ani otwarta");
                                    TileNotify.Send("Brama nie jest ani zamknięta, ani otwarta");
                                    BadgeNotify.Set(BadgeValue.none);
                                    break;
                                }
                            case StanBramy.CzujnikPrzerwaniaWiązki:
                                {
                                    BadgeIndexUp();
                                    ToastNotify.Send("Aktywny czujnik przerwana wiązki IR");
                                    TileNotify.Send("Aktywny czujnik przerwana wiązki IR");
                                    BadgeNotify.Set(BadgeValue.paused);
                                    break;
                                }
                            case StanBramy.Awaria:
                                {
                                    BadgeIndexUp();
                                    ToastNotify.Send("Awaria Bramy");
                                    TileNotify.Send("Awaria Bramy");
                                    BadgeNotify.Set(BadgeValue.error);
                                    break;
                                }
                            case StanBramy.BrakPolaczenia:
                                {
                                    BadgeIndexUp();
                                    ToastNotify.Send("Oczekiwanie na połączenie Bramy z serwerem");
                                    TileNotify.Send("Oczekiwanie na połączenie Bramy z serwerem");
                                    BadgeNotify.Set(BadgeValue.error);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        break;
                    }
                //case Commands.Command.Biurko:
                //    {
                //        ToastNotify.Send("Ktoś zmienia Led-y w biurku");
                //        break;
                //    }
                default:
                    {
                        Debug.WriteLine("Nieprawidłowa Komenda");
                        break;
                    }
            }

        }
        void BadgeIndexUp()
        {
            int tmp = 0;
            int step = 0;
            if (int.TryParse(Convert.ToString(AppData["BadgeIndex"]), out tmp))
            {
                step = tmp;
            }
            step++;
            BadgeNotify.Set(step);
            AppData["BadgeIndex"] = step.ToString();
        }
    }
}
