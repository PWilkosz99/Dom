
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Dom_android
{
    public class BootBroadcastReceiver :BroadcastReceiver
    {
        static String ACTION = "android.intent.action.BOOT_COMPLETED";  
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(ACTION))
            {
                Intent handler = new Intent(context, typeof(DemoIntentService));
                context.StartService(handler);
                Android.Util.Log.WriteLine(Android.Util.LogPriority.Info, "Dom", "send on boot");
            //    //Service    
            //    Intent serviceIntent = new Intent(context, StartOnBootService.class);       
            //context.startService(serviceIntent);   
            }
}
    }
}