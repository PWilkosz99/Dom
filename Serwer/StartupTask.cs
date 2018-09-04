using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Networking.Sockets;
using Windows.Networking;
using System.Threading.Tasks;
using System.IO;
using Serwer.Podzespoły;
using SQLitePCL;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using Windows.System.Threading;




// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Serwer
{
    public sealed class StartupTask : IBackgroundTask
    {

        private Dictionary<int, Client> AllClients = new Dictionary<int, Client>();
        private TcpListener Serwer = new TcpListener(IPAddress.Parse("192.168.1.8"), 80);

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            // 
            System.Threading.ManualResetEvent DontDie = new System.Threading.ManualResetEvent(false);
            DontDie.Reset();

            // Debug Logi
            Log.BeginAsync();

            // Event do oczyszczenia klienta
            Client.EndConnection += Client_EndConnection;

            //Start modułów
            Event.Begin();
            Biurko.BeginAsync();
            Brama.BeginAsync();

            //Start serwera
            Serwer.Start();
            SerwerWorkAsync();
            Log.Write("Uruchomiono Serwer");
            Debug.WriteLine("Uruchomiono Serwer");

            DontDie.WaitOne();
            deferral.Complete();
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
                            break;
                        }
                        FreeIndex++;
                    }
                }
                catch(Exception e)
                {
                    Log.Write(e);
                }
            }
        }
        
        private void Client_EndConnection(int id)
        {
            if(AllClients.ContainsKey(id))
            {
                AllClients[id].Close();
                AllClients[id] = null;
                AllClients.Remove(id);
            }
        }
    }  
}