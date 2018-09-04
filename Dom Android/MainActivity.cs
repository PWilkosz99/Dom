using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;

using Android.Support.V4;
using SupportFragment = Android.Support.V4.App.Fragment;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Text;
using Android.Content.PM;
using System.Net;
using System.Threading.Tasks;
using Android.Views;
using Android.Provider;
using Android.Content;
using Android.Net.Wifi;

namespace Dom_android
{
	[Activity (Name = "com.Dom_android.activity", Label = "Dom", MainLauncher = true,LaunchMode=LaunchMode.SingleTop, Icon = "@drawable/icon", Theme="@style/MyTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private SupportFragment mCurrentFragment;
		private Brama brama;
		private Biurko biurko; 
		private Empty empty;
        private Simson simson;
        private SimsonWiecej simsonwiecej;

        public Button Wiecej;

        DrawerLayout drawerLayout;
        NavigationView navigationView;
        Android.Support.V7.Widget.Toolbar toolbar;

        BroadcastReceiver Reciver = new DomBroadcastReceiver();
        IntentFilter ReciverFilter = new IntentFilter(WifiManager.NetworkStateChangedAction);

        protected override void OnCreate(Bundle bundle)  // Activity podczas inicjalizacji aplikacji
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            brama = new Brama();
            biurko = new Biurko();
            empty = new Empty();
            simson = new Simson();
            simsonwiecej = new SimsonWiecej();


            var trans = SupportFragmentManager.BeginTransaction();


            trans.Add(Resource.Id.fragmentContainer, empty, "Empty");
            trans.Hide(empty);

            trans.Add(Resource.Id.fragmentContainer, simson, "Simson");
            trans.Hide(simson);           

            trans.Add(Resource.Id.fragmentContainer, biurko, "Biurko");
            trans.Hide(biurko);

            trans.Add(Resource.Id.fragmentContainer, brama, "Brama");

            trans.Commit();

            mCurrentFragment = brama;


            // Init toolbar
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // Attach item selected handler to navigation view
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            Wiecej = FindViewById<Button>(Resource.Id.more);
            Wiecej.Visibility = ViewStates.Invisible;
            Wiecej.Click += Wiecej_Click;


            Intent handler = new Intent(this, typeof(DemoIntentService));
            StartService(handler);


            Client.Begin();
        }


        protected override void OnResume()
        {
            base.OnResume();
            //RegisterReceiver(Reciver, ReciverFilter);
        }

        protected override void OnPause()
        {
            base.OnPause();
           // UnregisterReceiver(Reciver);
        }

        protected override void OnRestart()  // Activity pozejściu z pierwszego planu i podczas wchodzenia na pierwszy plan
        {
            //Client.Connect();
            base.OnRestart();
            //Toast.MakeText(this, "Siemka znowu", ToastLength.Short).Show();
        }

        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.Brama):
                    {
                        // React on 'Home' selection
                        //Snackbar.Make(drawerLayout, "Brama wjazdowa", Snackbar.LengthLong).SetAction("OK", (v) => { }).Show();
                        Android:Title = "Brama wjazdowa";
                        Wiecej.Visibility = ViewStates.Invisible;
                        ShowFragment(brama);                    
                        break;
                    }

                case (Resource.Id.Biurko):
                    {
                        // React on 'Friends' selection
                        //Snackbar.Make(drawerLayout, "Biurko", Snackbar.LengthLong).SetAction("OK", (v) => { }).Show();
                        Android: Title = "Biurko";
                        Wiecej.Visibility = ViewStates.Invisible;
                        ShowFragment(biurko);
                        break;
                    }
                case (Resource.Id.Empty):
                    {
                        // React on 'Discussion' selection
                        //Snackbar.Make(drawerLayout, "Biurko", Snackbar.LengthLong).SetAction("OK", (v) => { }).Show();
                        Android:Title = "Empty";
                        Wiecej.Visibility = ViewStates.Invisible;
                        ShowFragment(empty);
                        break;
                    }
                case (Resource.Id.Simson):
                    {
                        //Snackbar.Make(drawerLayout, "Simson", Snackbar.LengthLong).SetAction("OK", (v) => { }).Show();
                        Android: Title = "Simson";
                        Wiecej.Visibility = ViewStates.Visible;
                        ShowFragment(simson);
                        break;
                    }
            }
            // Close drawer
            drawerLayout.CloseDrawers();
        }

        private void ShowFragment (SupportFragment fragment)
		{
            if (fragment.IsVisible){
            	return;
            }

            var trans = SupportFragmentManager.BeginTransaction();

            trans.Hide(mCurrentFragment);
            trans.Show(fragment);
            trans.AddToBackStack(null);
            trans.Commit();

            mCurrentFragment = fragment;
		}

        private void Wiecej_Click(object sender, EventArgs e)
        {
            RunOnUiThread(() => StartActivity(typeof(SimsonWiecej)));
        }

       


    }
}