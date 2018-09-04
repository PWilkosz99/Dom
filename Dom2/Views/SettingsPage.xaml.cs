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
        string TaskName = "Dom_Background";


        private const string socketId = "SampleSocket";
        string hostname = "192.168.1.8";
        string port = "80";
        private IBackgroundTaskRegistration task = null;
        private StreamSocket socket = null;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TileNotify.Clear();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {

            //RegisterBackgroundTask(TaskName + ".Start", TaskName, new SystemTrigger(SystemTriggerType.SessionConnected, false), null);



            try
            {
                foreach (var current in BackgroundTaskRegistration.AllTasks)
                {
                    if (current.Value.Name == TaskName)
                    {
                        task = current.Value;
                        break;
                    }
                }

                // If there is no task allready created, create a new one
                if (task == null)
                {
                    var socketTaskBuilder = new BackgroundTaskBuilder();
                    socketTaskBuilder.Name = TaskName;
                    socketTaskBuilder.TaskEntryPoint = TaskName + ".Start";
                    var trigger = new SocketActivityTrigger();
                    socketTaskBuilder.SetTrigger(trigger);
                    task = socketTaskBuilder.Register();
                }

                SocketActivityInformation socketInformation;
                if (SocketActivityInformation.AllSockets.TryGetValue(socketId, out socketInformation))
                {
                    // Application can take ownership of the socket and make any socket operation
                    // For sample it is just transfering it back.
                    socket = socketInformation.StreamSocket;
                    socket.TransferOwnership(socketId);
                    socket = null;
                   Debug.WriteLine("Connected. You may close the application");
                }

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }


            try
            {
                SocketActivityInformation socketInformation;
                if (!SocketActivityInformation.AllSockets.TryGetValue(socketId, out socketInformation))
                {
                    socket = new StreamSocket();
                    socket.EnableTransferOwnership(task.TaskId, SocketActivityConnectedStandbyAction.Wake);

                    var targetServer = new HostName(hostname);
                    await socket.ConnectAsync(targetServer, port);
                    // To demonstrate usage of CancelIOAsync async, have a pending read on the socket and call 
                    // cancel before transfering the socket. 
                    DataReader reader = new DataReader(socket.InputStream);
                    reader.InputStreamOptions = InputStreamOptions.Partial;
                    var read = reader.LoadAsync(250);
                    read.Completed += (info, status) =>
                    {

                    };
                    await socket.CancelIOAsync();
                    socket.TransferOwnership(socketId);
                    socket = null;
                }
               
                Debug.WriteLine("Connected. You may close the application");
            }
            catch (Exception exception)
            {
               Debug.WriteLine(exception.Message);
            }
















        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            UnregisterBackgroundTasks(TaskName);
        }



        /// <summary>
        /// Register a background task with the specified taskEntryPoint, name, trigger,
        /// and condition (optional).
        /// </summary>
        /// <param name="taskEntryPoint">Task entry point for the background task.</param>
        /// <param name="name">A name for the background task.</param>
        /// <param name="trigger">The trigger for the background task.</param>
        /// <param name="condition">An optional conditional event that must be true for the task to fire.</param>
        public static BackgroundTaskRegistration RegisterBackgroundTask(String taskEntryPoint, String name, IBackgroundTrigger trigger, IBackgroundCondition condition)
        {

            foreach (var ftask in BackgroundTaskRegistration.AllTasks)
            {
                if (ftask.Value.Name == name)
                {
                    Debug.WriteLine("Zadanie wcześniej zarejetrowano");
                    return null;                   
                }
            }
            


            var builder = new BackgroundTaskBuilder();

            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);

                //
                // If the condition changes while the background task is executing then it will
                // be canceled.
                //
                builder.CancelOnConditionLoss = true;
            }

            BackgroundTaskRegistration task = builder.Register();
            return task;
        }

        /// <summary>
        /// Unregister background tasks with specified name.
        /// </summary>
        /// <param name="name">Name of the background task to unregister.</param>
        public static void UnregisterBackgroundTasks(String name)
        {
            //
            // Loop through all background tasks and unregister any with SampleBackgroundTaskName or
            // SampleBackgroundTaskWithConditionName.
            //
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == name)
                {
                    cur.Value.Unregister(true);
                }
            }

            
        }

    }
}