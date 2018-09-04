
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Threading.Tasks;

namespace Dom_android
{
	public class BiurkoP : Android.Support.V4.App.Fragment
	{
        ProgressBar ConnectProgressBar = null;

        public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.BiurkoP, container, false);
            ConnectProgressBar = view.FindViewById<ProgressBar>(Resource.Id.ConnectProgressBar);

            if (Client.Connected)
            {
                ConnectProgressBar.Visibility = ViewStates.Invisible;
            }
            else
            {
                Client.ConnectedEvent += Client_ConnectedEvent;
            }

            return view;
		}


        private void Client_ConnectedEvent(bool Connect)
        {
            if (Connect)
            {
                ConnectProgressBar.Visibility = ViewStates.Invisible;
            }
            else
            {
                ConnectProgressBar.Visibility = ViewStates.Visible;
            }
        }


    }
}

