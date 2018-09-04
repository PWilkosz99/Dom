using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Dom_android
{
    [Service(Exported = true, Name = "com.Dom_android.KeepAliveService")]
    public class DemoIntentService : IntentService
    {
        public DemoIntentService() : base("KeepAliveService")
        {
        }
        
        protected override void OnHandleIntent(Intent intent)
        {
            ShowNotify(1, "Aplikacja Dom", "Serwis rozpoczął prace", "Informacje", NotificationImportance.Low, NotificationVisibility.Public);
            Client.ConnectedEvent += Client_ConnectedEvent;
            while (true)
            {
                Client.Begin();
                IntentFilter ied = new IntentFilter(WifiManager.NetworkStateChangedAction);
                RegisterReceiver(new DomBroadcastReceiver(), ied);
                                

                Android.Util.Log.WriteLine(Android.Util.LogPriority.Info, "Dom", "Started");
                Console.WriteLine("perform some long running work");
                new ManualResetEvent(false).WaitOne();
                Thread.Sleep(1000);
            }
        }

        private void Client_ConnectedEvent(bool Connect)
        {
            ShowNotify(3, "Aplikacja Dom", "Czy połączono: " +  Connect.ToString(), "Informacje", NotificationImportance.Default, NotificationVisibility.Public);
        }


        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);
            return StartCommandResult.Sticky;
        }



        /// <summary>
        /// Funkcja pokazująca powiadomienia
        /// </summary>
        /// <param name="NotifyID">ID powiadomienia</param>
        /// <param name="NotifyTitle">Tytuł powiadomienia</param>
        /// <param name="Notifytext">Zawartość<param>
        /// <param name="ChannelName">Nazwa kanału powiadomień</param>
        /// <param name="notificationImportance">Ważność powiadomienia</param>
        /// <param name="notificationVisibility">Widoczność dla innych powiadomienia</param>
        static public void ShowNotify(int NotifyID, string NotifyTitle, string NotifyText, string ChannelName, NotificationImportance notificationImportance, NotificationVisibility notificationVisibility)
        {
            string URGENT_CHANNEL = ChannelName;
            NotificationManager notificationManager = (NotificationManager)Application.Context.GetSystemService(NotificationService);


            // Work has finished, now dispatch anotification to let the user know.
            Notification.Builder notificationBuilder = new Notification.Builder(Application.Context)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetContentTitle(NotifyTitle)
                .SetContentText(NotifyText);


            try
            {
                // Avaliable in android 8.0
                NotificationChannel chan = new NotificationChannel(URGENT_CHANNEL, ChannelName,  notificationImportance);
                chan.EnableVibration(true);
                chan.LockscreenVisibility = notificationVisibility;
                notificationManager.CreateNotificationChannel(chan);
                notificationBuilder.SetChannelId(URGENT_CHANNEL);
            }
            catch { }

            //var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(NotifyID, notificationBuilder.Build());

        }



    }
}