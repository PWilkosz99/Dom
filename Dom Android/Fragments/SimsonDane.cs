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
    class SimsonDane : Android.Support.V4.App.Fragment
    {
        public static string Nazwa { get { return "Dane"; } }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.SimsonDane, container, false);
            return view;
        }
    }
}