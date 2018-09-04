using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

using Android.Content.PM;
using Android.Support.V7.App;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;

namespace Dom_android
{
    [Activity(Label = "Dane", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SimsonWiecej : AppCompatActivity
    {

        TabLayout tabLayout;
        Toolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SimsonWiecej);

            tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            FnInitTabLayout();
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);




            // Create your application here
        }

        void FnInitTabLayout()
        {
            tabLayout.SetTabTextColors(Android.Graphics.Color.Aqua, Android.Graphics.Color.AntiqueWhite);
            //Fragment array
            var fragments = new Android.Support.V4.App.Fragment[]  { new SimsonMieszanka(), new SimsonUstawienia(), new SimsonDane()};
            //Tab title array
            var titles = CharSequence.ArrayFromStringArray(new[] {  SimsonMieszanka.Nazwa, SimsonUstawienia.Nazwa, SimsonDane.Nazwa });

            var viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            //viewpager holding fragment array and tab title text
            viewPager.Adapter = new TabsFragmentPagerAdapter(SupportFragmentManager, fragments, titles);

            // Give the TabLayout the ViewPager 
            tabLayout.SetupWithViewPager(viewPager);
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            dialog.Dismiss();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    {
                        Finish();
                        return true;
                    }
                default:
                    {
                        return base.OnOptionsItemSelected(item);
                    }
            }
        }
    }
}