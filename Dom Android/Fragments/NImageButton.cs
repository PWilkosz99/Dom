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

namespace Dom_android
{
    class NImageButton : ImageButton
    {

        public NImageButton(Context context) : base(context) { }
        public NImageButton(Context context, Android.Util.IAttributeSet attrs) : base(context, attrs) { }
        public NImageButton(Context context, Android.Util.IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { }
        public NImageButton(Context context, Android.Util.IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) { }
        protected NImageButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }




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
                SetBackgroundResource(color);
            }
            else
            {
                SetBackgroundResource(Resource.Drawable.abc_btn_default_mtrl_shape);
            }

        }

    }
}