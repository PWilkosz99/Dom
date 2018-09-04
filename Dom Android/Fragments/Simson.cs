
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
using Java.Util;
using System.IO;
using Android.Bluetooth;
using Android.Support.Design.Widget;
using System.Threading.Tasks;
using System.Threading;
using Android.Graphics;
using Android.Telephony;
using RadialProgress;


namespace Dom_android
{
    #region Enum y
    //Kody komunikacyjne
    enum Typ
    {
        Swiatla = 2,
        Migacz = 3,
        Akcelerometr = 4,
        EEProm = 6,
        Dodatk = 7,
        Errors = 5,
    };

    enum Swiatlo
    {
        LewyMigacz = 3,
        PrawyMigacz = 4,
        Blinda = 5,
        SwiatloMijania = 2,
        SwiatloDrogowe = 0,
        SwiatloTyl = 8,
        Stop = 7,
        Postojowka = 6,
        Awaryjne = 9,
    };

    enum Dodatkowe
    {
        DzienNoc = 5,
        KeepAliveONOFF = 3,
        KeepAlive = 4,
        Sync = 2,
        Ucc = 6,
        EngineStart = 7,
        MigaczAutoONOFF = 8,

        Temperatura = 1,
        Kat = 9,
    };

    enum EEPROMDataType
    {
        T_MigaczCzas = 5,
        TolerancjaPrzechyluMPU = 6,
        EKeepAlive = 4,
        Migaczautooff = 7,
    };

    enum ErrorCodes
    {
        MPU = 3,
        Ekspander = 4,
    };
    #endregion

    public class Simson : Android.Support.V4.App.Fragment
    {
        public delegate void BTDataEventHandler(string reciverdata);
        public static event BTDataEventHandler BTData;


        private static int REQUEST_ENABLE_BT = 1;
        
        ProgressBar ConnectProgressBar = null;

        BluetoothAdapter BTAdapter;
        BluetoothSocket BTSocket;
        byte[] Buff;       

        Thread TCnn;
        bool BTCnn = false;
        bool BTrdytogo = false; // gotow do odpalenia
        bool BTend = false; // gotow do wylaczenia silnika

        //Adres modułu
        private readonly static string address = "20:15:11:02:77:41";

        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        bool keepAlive = false;

        // dashboard
        NImageButton Lewa;
        NImageButton Prawa;
        NImageButton Drogowe;
        NImageButton Postojowe;
        NImageButton Kierunkowe;

        NImageButton Polacz;
        NImageButton DzienNoc;
        NImageButton Awaryjne;
        NImageButton Stop;
        NImageButton Tyl;

        Switch KeepAlive;

        NProgressBar shift;

        View view;

        bool[] MigaczState;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SimsonUstawienia.BeginWrite += SimsonUstawienia_Write;

            new Thread(() => // keep Alive Thread
            {
                while (true)
                {
                    if (keepAlive)
                    {
                        if (BTCnn)
                        {
                            Write(new byte[] { (byte)Typ.Dodatk, (byte)Dodatkowe.KeepAlive });
                        }
                    }
                    Thread.Sleep(2000);
                }
            }).Start();
        }

        private void SimsonUstawienia_Write(byte [] codes)
        {
            Write(codes);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
           view = inflater.Inflate(Resource.Layout.Simson, container, false);

            MigaczState = new bool[5];

            BTAdapter = BluetoothAdapter.DefaultAdapter;

            ConnectProgressBar = view.FindViewById<ProgressBar>(Resource.Id.ConnectProgressBar);


            Lewa = view.FindViewById<NImageButton>(Resource.Id.lewa);
            Prawa = view.FindViewById<NImageButton>(Resource.Id.prawa);
            Drogowe = view.FindViewById<NImageButton>(Resource.Id.drogowe);
            Postojowe = view.FindViewById<NImageButton>(Resource.Id.postojowe);
            Kierunkowe = view.FindViewById<NImageButton>(Resource.Id.kierunkowe);

            Polacz = view.FindViewById<NImageButton>(Resource.Id.conn);
            DzienNoc = view.FindViewById<NImageButton>(Resource.Id.dziennoc);
            Awaryjne = view.FindViewById<NImageButton>(Resource.Id.awaryjne);
            Stop = view.FindViewById<NImageButton>(Resource.Id.stop);
            Tyl = view.FindViewById<NImageButton>(Resource.Id.tylne);

            KeepAlive = view.FindViewById<Switch>(Resource.Id.keepalive);
            shift = view.FindViewById<NProgressBar>(Resource.Id.bestshift);

            Lewa.OnColor = Resource.Drawable.zielony;
            Prawa.OnColor = Resource.Drawable.zielony;
            Drogowe.OnColor = Resource.Drawable.niebieski;
            Postojowe.OnColor = Resource.Drawable.zolty;
            Kierunkowe.OnColor = Resource.Drawable.zolty;

            //Polacz.OnColor = Resource.Drawable.zielony;
            DzienNoc.OnColor = Resource.Drawable.abc_btn_default_mtrl_shape;
            Awaryjne.OnColor = Resource.Drawable.pomaranczowy;
            Stop.OnColor = Resource.Drawable.czerwony;
            Tyl.OnColor = Resource.Drawable.czerwony;



            Lewa.Click += Lewa_Click;
            Prawa.Click += Prawa_Click;
            Drogowe.Click += Drogowe_Click;
            Postojowe.Click += Postojowe_Click;
            Kierunkowe.Click += Kierunkowe_Click;


            Polacz.Click += Polacz_Click;
            DzienNoc.Click += DzienNoc_Click;
            Awaryjne.Click += Awaryjne_Click;
            Stop.Click += Stop_Click;
            Tyl.Click += Tyl_Click;
            
            KeepAlive.CheckedChange += KeepAlive_CheckedChange;

            
            return view;
        }

        private void KeepAlive_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Write(new byte[] { (byte)Typ.Dodatk, (byte)Dodatkowe.KeepAliveONOFF, FromBool(e.IsChecked) });
            keepAlive = e.IsChecked;
        }

        private void Polacz_Click(object sender, EventArgs e)
        {
            var drawerLayout = Activity.FindViewById<Android.Support.V4.Widget.DrawerLayout>(Resource.Id.drawer_layout);
            if(BTend) 
            {
                // wylacz silnik
                Write(new byte[] { (byte)Typ.Dodatk, (byte)Dodatkowe.EngineStart, 0 });
            }
shift.Progress = shift.Progress + 100;

            if (BTrdytogo)
            {
                // gotow do odpalenia silnika
                Write(new byte[] { (byte)Typ.Dodatk, (byte)Dodatkowe.EngineStart, 1});

            }
                       

            if (BTCnn)
            {
                return;
            }


            TCnn = new Thread(async () =>
            {
                BTCnn = true;
                BTrdytogo = false;
                BTend = false;
                Activity.RunOnUiThread(() => ConnectProgressBar.Visibility = ViewStates.Visible);
                // Activity.RunOnUiThread(() => InfoPolacz.Text = "Łączenie...");

                if (BTAdapter == null)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        BTData?.Invoke(null);
                        Snackbar.Make(drawerLayout, "Bluetooth nie dostępne", Snackbar.LengthShort).Show();
                        Polacz.SetBackgroundResource(Resource.Drawable.czerwony);
                    }
                    );
                    BTCnn = false;
                    Activity.RunOnUiThread(() => ConnectProgressBar.Visibility = ViewStates.Invisible);
                    return;
                }

                if (!BTAdapter.IsEnabled)
                {
                    BTAdapter.Enable();
                    for (; !BTAdapter.IsEnabled;)
                    {
                    }
                    // var enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                    //  StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);

                }


                

                BluetoothDevice device = (from bd in BTAdapter.BondedDevices where bd.Name == "Simson" select bd).FirstOrDefault();

                if (device == null)
                {
                    BTData?.Invoke(null);
                    Activity.RunOnUiThread(() =>
                    {
                        Snackbar.Make(drawerLayout, "Urządzenie nie sparowane", Snackbar.LengthShort).Show();
                        Polacz.SetBackgroundResource(Resource.Drawable.czerwony);
                    });
                    Activity.RunOnUiThread(() => ConnectProgressBar.Visibility = ViewStates.Invisible);
                    return;
                }


                BTSocket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));

                try
                {
                    Activity.RunOnUiThread(() => ConnectProgressBar.Visibility = ViewStates.Visible);
                    await BTSocket.ConnectAsync();
                    BTCnn = true;
                    Activity.RunOnUiThread(() =>
                    {
                        Polacz.SetBackgroundResource(Resource.Drawable.zielony);
                        Polacz.SetImageResource(Resource.Drawable.klucz);
                        ConnectProgressBar.Visibility = ViewStates.Invisible;
                    });
                    BTend = true;
                    Write(new byte[] { (byte)Typ.Dodatk, (byte)Dodatkowe.Sync });
                    try
                    {
                        new Thread(() => IsNight()).Start();
                    }
                    catch
                    { }
                }
                catch
                {
                    BTData?.Invoke(null);
                    Activity.RunOnUiThread(() =>
                    {
                        Snackbar.Make(drawerLayout, "Błąd połączenia", Snackbar.LengthShort).Show();
                        Polacz.SetBackgroundResource(Resource.Drawable.czerwony);
                    });

                    BTCnn = false;
                    BTrdytogo = false;
                    Activity.RunOnUiThread(() => ConnectProgressBar.Visibility = ViewStates.Invisible);
                    return;
                }
                new Thread(()=> BeginReading()).Start();
            });

            if (TCnn.ThreadState != ThreadState.Running)
            {
                TCnn.Start();
            }
        }

        private void BeginReading()
        {
            
            Stream stream = BTSocket.InputStream;
            String tmp = String.Empty;
            for (;;)
            {
                if (stream.CanRead)
                {
                    try
                    {
                        char ch = (char)stream.ReadByte();

                        if (ch == '#')
                        {
                            //end
                            Code(tmp);
                            tmp = String.Empty;
                        }
                        else
                        {
                            // no end
                            tmp += ch;
                        }
                    }
                    catch
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            BTData?.Invoke(null);
                            BTrdytogo = false;
                            BTend = false;
                            BTCnn = false;
                            var drawerLayout = Activity.FindViewById<Android.Support.V4.Widget.DrawerLayout>(Resource.Id.drawer_layout);
                            Snackbar.Make(drawerLayout, "Utracono połączenie", Snackbar.LengthShort).Show();
                            Polacz.SetImageResource(Resource.Drawable.bt);
                            Polacz.SetBackgroundResource(Resource.Drawable.czerwony);
                            KeepAlive.Checked = false;

                        });
                    }
                }
                else
                {
                    Task.Delay(10);
                }

            }
        }

        private void Code(string message)
        {
            message = message.Replace("\n", String.Empty);
            message = message.Replace("\r", String.Empty);
            message = message.Replace("\t", String.Empty);
            message = message.Trim();

           // Console.WriteLine( "in: " + message);
            BTData?.Invoke(message);
            Activity.RunOnUiThread(() =>
            {
                String[] code;
                Typ typ;
                try
                {
                    code = message.Split(';');
                    typ = (Typ)Int16.Parse(code[0]);


                    switch (typ)
                    {
                        case Typ.Swiatla:
                            {
                                try
                                {
                                    Swiatlo swiatlo = (Swiatlo)Int16.Parse(code[1]);
                                    bool on = ToBool(code[2]);
                                    switch (swiatlo)
                                    {
                                        case Swiatlo.LewyMigacz:
                                            {
                                                Lewa.SetState(on);
                                                break;
                                            }
                                        case Swiatlo.PrawyMigacz:
                                            {
                                                Prawa.SetState(on);
                                                break;
                                            }
                                        case Swiatlo.Blinda:
                                            {
                                                break;
                                            }
                                        case Swiatlo.SwiatloMijania:
                                            {
                                                Kierunkowe.SetState(on);
                                                break;
                                            }
                                        case Swiatlo.SwiatloDrogowe:
                                            {
                                                Drogowe.SetState(on);
                                                break;
                                            }
                                        case Swiatlo.SwiatloTyl:
                                            {
                                                Tyl.SetState(on);
                                                break;
                                            }
                                        case Swiatlo.Stop:
                                            {
                                                Stop.SetState(on);
                                                break;
                                            }
                                        case Swiatlo.Postojowka:
                                            {
                                                Postojowe.SetState(on);
                                                break;
                                            }
                                        case Swiatlo.Awaryjne:
                                            {
                                                Awaryjne.SetState(on);
                                                break;
                                            }
                                        default:
                                            {
                                                break;
                                            }
                                    }
                                }
                                catch { }
                                break;
                            }
                        case Typ.Migacz:
                            {
                                try
                                {
                                    Swiatlo swiatlo = (Swiatlo)Int16.Parse(code[1]);
                                    bool on = ToBool(code[2]);

                                    switch (swiatlo)
                                    {
                                        case Swiatlo.LewyMigacz:
                                            {
                                                MigaczState[(byte)Swiatlo.LewyMigacz] = on;
                                                break;
                                            }
                                        case Swiatlo.PrawyMigacz:
                                            {
                                                MigaczState[(byte)Swiatlo.PrawyMigacz] = on;
                                                break;
                                            }
                                        default:
                                            {
                                                break;
                                            }
                                    }
                                }
                                catch { }
                                break;
                            }
                        case Typ.Dodatk:
                            {
                                try
                                {
                                    Dodatkowe swiatlo = (Dodatkowe)Int16.Parse(code[1]);

                                    switch (swiatlo)
                                    {
                                        case Dodatkowe.DzienNoc:
                                            {
                                                bool on = ToBool(code[2]);
                                                if (on)
                                                {
                                                    DzienNoc.SetImageResource(Resource.Drawable.dzien);
                                                }
                                                else
                                                {
                                                    DzienNoc.SetImageResource(Resource.Drawable.noc);
                                                }
                                                DzienNoc.SetState(on);
                                                break;
                                            }
                                        case Dodatkowe.Sync:
                                            {
                                                try
                                                {
                                                    Lewa.SetState(ToBool(code[2]));
                                                    Prawa.SetState(ToBool(code[3]));
                                                    Kierunkowe.SetState(ToBool(code[4]));
                                                    Drogowe.SetState(ToBool(code[5]));
                                                    Tyl.SetState(ToBool(code[6]));
                                                    Stop.SetState(ToBool(code[7]));
                                                    Postojowe.SetState(ToBool(code[8]));                                                   
                                                    DzienNoc.SetState(ToBool(code[9]));

                                                    if (DzienNoc.GetState())  //jak dzien
                                                    {
                                                        DzienNoc.SetImageResource(Resource.Drawable.dzien);
                                                    }
                                                    else
                                                    {
                                                        DzienNoc.SetImageResource(Resource.Drawable.noc);
                                                    }

                                                    if (ToBool(code[10]))// zaplon
                                                    {
                                                        Polacz.SetBackgroundResource(Resource.Drawable.zielony);
                                                        BTrdytogo = false;
                                                        BTend = true;
                                                    }
                                                    else  // jak nie
                                                    {
                                                        Polacz.SetBackgroundResource(Resource.Drawable.zolty);
                                                        BTrdytogo = true;
                                                        BTend = false;
                                                    }

                                                    Awaryjne.SetState(false);

                                                    // Int16 voltage = Int16.Parse(code[1]);
                                                }
                                                catch { }
                                                break;
                                            }
                                        case Dodatkowe.Ucc:
                                            {
                                                try
                                                {
                                                    //Int16 voltage = Int16.Parse(code[1]);
                                                }
                                                catch { }
                                                break;
                                            }
                                        case Dodatkowe.EngineStart:
                                            {
                                                if(ToBool(code[2]))// jak wlaczony silnik
                                                {
                                                    Polacz.SetBackgroundResource(Resource.Drawable.zielony);
                                                    BTrdytogo = false;
                                                    BTend = true;
                                                }
                                                else  // jak nie
                                                {
                                                    Polacz.SetBackgroundResource(Resource.Drawable.zolty);
                                                    BTrdytogo = true;
                                                    BTend = false;
                                                }
                                                break;
                                            }
                                        case Dodatkowe.KeepAliveONOFF:
                                            {
                                                if (!ToBool(code[2]))
                                                {
                                                    KeepAlive.Checked = false;
                                                }                                               
                                                break;
                                            }

                                        default:
                                            break;
                                    }
                                }
                                catch { }
                                break;
                            }
                        case Typ.EEProm:
                            {
                                try
                                {
                                    EEPROMDataType eppromtype = (EEPROMDataType)Int16.Parse(code[1]);

                                    switch (eppromtype)
                                    {
                                        case EEPROMDataType.EKeepAlive:
                                            {
                                                if (ToBool(code[2]))
                                                {
                                                    Snackbar.Make(view.FindViewById<LinearLayout>(Resource.Id.layout1), "Zablokowano", Snackbar.LengthLong).Show();
                                                }
                                                else
                                                {
                                                    Snackbar.Make(view.FindViewById<LinearLayout>(Resource.Id.layout1), "Odblokowano", Snackbar.LengthLong).Show();
                                                }
                                                break;
                                            }
                                    }
                                }
                                catch { }
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                catch
                {
                    Console.WriteLine("Convert error");
                }
            });
        }

        private void Write(string text)
        {
            Console.WriteLine( "out: " + text);
            if (BTCnn)
            {
                text = text + "#";
                byte[] buf = ASCIIEncoding.ASCII.GetBytes(text);
                BTSocket.OutputStream.Write(buf, 0, buf.Length);
            }
        }

        private void Write(byte[] codes)
        {
            string tmp = String.Empty;
            for (int i = 0; i < codes.Length - 1; i++)
            {
                tmp += codes[i].ToString();
                tmp += ';';
            }
            tmp += codes[codes.Length - 1].ToString();
            Write(tmp);
        }

        bool ToBool(string code)
        {
            if (code == "1")
            {
                return true;
            }
            if (code == "0")
            {
                return false;
            }
            throw new ArgumentException();
        }

        byte FromBool(bool code)
        {
            if (code)
            {
                return 1;
            }
            else
            {
                return 0;
            }
            throw new ArgumentException();
        }

      



        void IsNight() // Sprawdza czy jest dzien czy noc
        {

            int H = DateTime.Now.Hour;
            int M = DateTime.Now.Hour;

            Sun Czasy = new Sun();

            bool dzien = false;

            if((H > Czasy.Wschod[0]) &&  (H < Czasy.Zachod[0]))
            {
                // dzien
                dzien = true;
            }
            if((H == Czasy.Wschod[0]) && (H == Czasy.Zachod[0]))
            {
                if ((M > Czasy.Wschod[1]) && (M < Czasy.Zachod[1]))
                {
                    // dzien
                    dzien = true;
                }
            }

            if(dzien)
            {
                Write(new byte[] { (byte)Typ.Dodatk, (byte)Dodatkowe.DzienNoc, 1 });
            }
            else
            {
                Write(new byte[] { (byte)Typ.Dodatk, (byte)Dodatkowe.DzienNoc, 0 });
            }

        }

        #region Funkcje do wysyłki

        private void Kierunkowe_Click(object sender, EventArgs e)
        {
            Write(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.SwiatloMijania, FromBool(Kierunkowe.NegState) });
        }

        private void Postojowe_Click(object sender, EventArgs e)
        {
            Write(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.Postojowka, FromBool(Postojowe.NegState) });
        }

        private void Drogowe_Click(object sender, EventArgs e)
        {
            Write(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.SwiatloDrogowe, FromBool(Drogowe.NegState) });
        }

        private void Prawa_Click(object sender, EventArgs e)
        {
            Write(new byte[] { (byte)Typ.Migacz, (byte)Swiatlo.PrawyMigacz, FromBool(!(MigaczState[(byte)Swiatlo.PrawyMigacz])) });
        }

        private void Lewa_Click(object sender, EventArgs e)
        {
            Write(new byte[] { (byte)Typ.Migacz, (byte)Swiatlo.LewyMigacz, FromBool(!(MigaczState[(byte)Swiatlo.LewyMigacz])) });
        }

        private void Tyl_Click(object sender, EventArgs e)
        {
            Write(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.SwiatloTyl, FromBool(Tyl.NegState) });
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            Write(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.Stop, FromBool(Stop.NegState) });
        }

        private void Awaryjne_Click(object sender, EventArgs e)
        {
            Write(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.Awaryjne, FromBool(Awaryjne.NegState) });
        }

        private void DzienNoc_Click(object sender, EventArgs e)
        {
           Write(new byte[] { (byte)Typ.Dodatk, (byte)Dodatkowe.DzienNoc, FromBool(DzienNoc.NegState) });
        }
    }
#endregion
}