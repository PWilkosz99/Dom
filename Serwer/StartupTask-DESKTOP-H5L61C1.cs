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
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            // 
            
            ThreadPool.RunAsync( workitem => { new Main(); }, WorkItemPriority.High).AsTask().Wait();            
            taskInstance.GetDeferral().Complete();   
        }        
    }
}
