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
using Android.Graphics;
using System.Threading;

namespace Dom_android
{
    class NProgressBar : ProgressBar
    {
        public NProgressBar(Context context) : base(context) { new Thread(() => TimerHelper()).Start(); }
        public NProgressBar(Context context, IAttributeSet attrs) : base(context, attrs) { new Thread(() => TimerHelper()).Start(); }
        public NProgressBar(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) { new Thread(() => TimerHelper()).Start(); }
        public NProgressBar(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) { new Thread(() => TimerHelper()).Start(); }
        protected NProgressBar(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private static bool TimeIsOn;
        
        public override  int Progress
        {
            get
            {
                return base.Progress;
            }
            set
            { 
                if(value > Max)
                {
                    if(TimeIsOn)
                    {
                        if(Visibility == ViewStates.Visible)
                        {
                            Visibility = ViewStates.Invisible;
                        }
                        else
                        {
                            Visibility = ViewStates.Visible;
                        }
                        TimeIsOn = false;
                    }
                }
                else
                {
                    Visibility = ViewStates.Visible;
                }
                base.Progress = value;
            }
        }
        static void TimerHelper()
        {
            for (;;)
            {
                TimeIsOn = true;
                Thread.Sleep(200);
            }
        }
    }
}