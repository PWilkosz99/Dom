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
using Windows.ApplicationModel.Background;

namespace Dom.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TileNotify.Clear();
        }

        private void Button_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RegisterTask();
        }

        private async Task RegisterTask()
        {
            try
            {
                await BackgroundExecutionManager.RequestAccessAsync();
                BackgroundTaskBuilder Builder = new BackgroundTaskBuilder();
                Builder.Name = typeof(Dom_Background.Start).FullName;

                foreach (var cur in BackgroundTaskRegistration.AllTasks)
                {

                    if (cur.Value.Name == Builder.Name)
                    {

                        Debug.WriteLine("Task already registred");

                        return;
                    }
                }

                Builder.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, true));
                Builder.TaskEntryPoint = Builder.Name;
                Builder.Register();
            }
            catch { }
        }


    }
}