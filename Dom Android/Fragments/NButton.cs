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
using Android.Graphics;
using System.Threading;
using Android.Util;

namespace Dom_android
{
    class NButton : Button
    {

#region Konstruktory
        public NButton(Context context) : base(context)
        {
            Text = "OFF";
        }

        public NButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Text = "OFF";
        }

        public NButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Text = "OFF";
        }

        public NButton(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Text = "OFF";
        }

        protected NButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Text = "OFF";
        }
#endregion

        private bool stan = false;
        private int color;
        public int OnColor { set { color = value; } get { return color; } }
        public bool NegState { get { return !stan; } }

        public bool GetState()
        {
            return stan;
        }

        public void SetState(bool value)
        {

            stan = value;
            if (stan)
            {
                if(color == Resource.Drawable.czarny)
                {
                    SetTextColor(Color.White);
                }
                Text = "ON";
                SetBackgroundResource(color);
            }
            else
            {
                if (color == Resource.Drawable.czarny)
                {
                    SetTextColor(Color.Black);
                }
                Text = "OFF";
                SetBackgroundResource(Resource.Drawable.abc_btn_default_mtrl_shape);
            }  
        }

        

    }
}