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
using Android.Support.Design.Widget;
using Android.Views.InputMethods;
using Android.Telephony;

namespace Dom_android
{
    class SimsonUstawienia : Android.Support.V4.App.Fragment
    {
        public delegate void WriteEventHandler(byte[] code);
        public static event WriteEventHandler BeginWrite;


        NButton LewyMigacz;
        NButton PrawyMigacz;
        NButton Drogowe;
        NButton Postojowe;
        NButton Kierunkowe;

        NButton DzienNoc;
        NButton Awaryjne;
        NButton Stop;
        NButton Tyl;

        NButton Zaplon;
        NButton LewyMigaczStaly;
        NButton PrawyMigaczStaly;
        NButton Blinda;

        EditText MigaczCzasEditText;
        Button MigaczCzasSet;

        Button Zablokuj;
        Button Odblokuj;

        Button AWMOn;
        Button AWMOff;
        EditText AWMEditTxt;
        Button AWMSet;


        View view;

        public static string Nazwa { get { return "Ustawienia"; } }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.SimsonUstawienia, container, false);     
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
            InitUI();
        }

        public override void OnPause()
        {
            base.OnPause();
            Simson.BTData -= Simson_BTData;
        }

        private void InitUI()
        {
            LewyMigacz = view.FindViewById<NButton>(Resource.Id.lewymigacz);
            PrawyMigacz = view.FindViewById<NButton>(Resource.Id.prawymigacz);
            Drogowe = view.FindViewById<NButton>(Resource.Id.swiatldrogowe);
            Postojowe = view.FindViewById<NButton>(Resource.Id.postojowka);
            Kierunkowe = view.FindViewById<NButton>(Resource.Id.swiatlomijania);

            DzienNoc = view.FindViewById<NButton>(Resource.Id.dziennoc);
            Awaryjne = view.FindViewById<NButton>(Resource.Id.awaryjne);
            Stop = view.FindViewById<NButton>(Resource.Id.stop);
            Tyl = view.FindViewById<NButton>(Resource.Id.swiatlotyl);

            Zaplon = view.FindViewById<NButton>(Resource.Id.zaplon);
            LewyMigaczStaly = view.FindViewById<NButton>(Resource.Id.lewymigaczstaly);
            PrawyMigaczStaly = view.FindViewById<NButton>(Resource.Id.prawymigaczstaly);
            Blinda = view.FindViewById<NButton>(Resource.Id.blinda);

            MigaczCzasEditText = view.FindViewById<EditText>(Resource.Id.MigaczCzasEditText);
            MigaczCzasSet = view.FindViewById<Button>(Resource.Id.tmigaczset);

            Zablokuj = view.FindViewById<Button>(Resource.Id.zablokuj);
            Odblokuj = view.FindViewById<Button>(Resource.Id.odblokuj);

            AWMOn = view.FindViewById<Button>(Resource.Id.awmon);
            AWMOff = view.FindViewById<Button>(Resource.Id.awmoff);
            AWMEditTxt = view.FindViewById<EditText>(Resource.Id.awmEditText);
            AWMSet = view.FindViewById<Button>(Resource.Id.awmset);

            LewyMigacz.OnColor = Resource.Drawable.zielony;
            PrawyMigacz.OnColor = Resource.Drawable.zielony;
            Drogowe.OnColor = Resource.Drawable.niebieski;
            Postojowe.OnColor = Resource.Drawable.zolty;
            Kierunkowe.OnColor = Resource.Drawable.zolty;            

            DzienNoc.OnColor = Resource.Drawable.czarny;
            Awaryjne.OnColor = Resource.Drawable.pomaranczowy;
            Stop.OnColor = Resource.Drawable.czerwony;
            Tyl.OnColor = Resource.Drawable.czerwony;

            Zaplon.OnColor = Resource.Drawable.zielony;
            LewyMigaczStaly.OnColor = Resource.Drawable.zielony;
            PrawyMigaczStaly.OnColor = Resource.Drawable.zielony;
            Blinda.OnColor = Resource.Drawable.niebieski;

            Simson.BTData += Simson_BTData;

            LewyMigacz.Click += LewyMigacz_Click;
            PrawyMigacz.Click += PrawyMigacz_Click;
            Drogowe.Click += Drogowe_Click;
            Postojowe.Click += Postojowe_Click;
            Kierunkowe.Click += Kierunkowe_Click;


            DzienNoc.Click += DzienNoc_Click;
            Awaryjne.Click += Awaryjne_Click;
            Stop.Click += Stop_Click;
            Tyl.Click += Tyl_Click;

            Zaplon.Click += Zaplon_Click;
            LewyMigaczStaly.Click += LewyMigaczStaly_Click;
            PrawyMigaczStaly.Click += PrawyMigaczStaly_Click;
            Blinda.Click += Blinda_Click;

            Zablokuj.Click += Zablokuj_Click;
            Odblokuj.Click += Odblokuj_Click;

            AWMOn.Click += AWMOn_Click;
            AWMOff.Click += AWMOff_Click;
            AWMSet.Click += AWMSet_Click;

            MigaczCzasSet.Click += MigaczCzasSet_Click;

            BeginWrite?.Invoke(new byte[] { (byte)Typ.Dodatk, (byte)Dodatkowe.Sync });
        }

        private void AWMSet_Click(object sender, EventArgs e)
        {
            InputMethodManager imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(MigaczCzasEditText.WindowToken, HideSoftInputFlags.None);
            byte value = 0;
            try
            {
                value = byte.Parse(AWMEditTxt.Text);
                if (!((value > 0) && (value <= 180))) // jak poza zakresem
                {
                    throw new ArgumentException("Poza zakresem");
                }
            }

            catch
            {
                Snackbar.Make(view.FindViewById<LinearLayout>(Resource.Id.layout1), "Wprować liczbę stopni z zakresu 1-180", Snackbar.LengthLong).Show();
                return;
            }

            string svalue = value.ToString("D3");
            BeginWrite?.Invoke(new byte[] { (byte)Typ.EEProm, (byte)EEPROMDataType.TolerancjaPrzechyluMPU, byte.Parse(svalue[0].ToString()), byte.Parse(svalue[1].ToString()), byte.Parse(svalue[2].ToString()) });

        }

        private void AWMOff_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.EEProm, (byte)EEPROMDataType.Migaczautooff, 0 });
        }

        private void AWMOn_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.EEProm, (byte)EEPROMDataType.Migaczautooff, 1 });
        }

        private void Odblokuj_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.EEProm, (byte)EEPROMDataType.EKeepAlive, 0 });
        }

        private void Zablokuj_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.EEProm, (byte)EEPROMDataType.EKeepAlive, 1 });
        }

        private void Simson_BTData(string reciverdata)
        {
            if (String.IsNullOrEmpty(reciverdata))
            {
                //blad polaczenia
                return;
            }

            Activity.RunOnUiThread(() =>
            {
                String[] code;
                Typ typ;
                try
                {                    
                    code = reciverdata.Split(';');
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
                                                LewyMigaczStaly.SetState(on);
                                                break;
                                            }
                                        case Swiatlo.PrawyMigacz:
                                            {
                                                PrawyMigaczStaly.SetState(on);
                                                break;
                                            }
                                        case Swiatlo.Blinda:
                                            {
                                                Blinda.SetState(on);
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
                                                LewyMigacz.SetState(on);
                                                break;
                                            }
                                        case Swiatlo.PrawyMigacz:
                                            {
                                                PrawyMigacz.SetState(on);
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
                                                DzienNoc.SetState(on);
                                                break;
                                            }
                                        case Dodatkowe.Sync:
                                            {
                                                try
                                                {
                                                    LewyMigacz.SetState(ToBool(code[2]));
                                                    PrawyMigacz.SetState(ToBool(code[3]));
                                                    Kierunkowe.SetState(ToBool(code[4]));
                                                    Drogowe.SetState(ToBool(code[5]));
                                                    Tyl.SetState(ToBool(code[6]));
                                                    Stop.SetState(ToBool(code[7]));
                                                    Postojowe.SetState(ToBool(code[8]));
                                                    DzienNoc.SetState(ToBool(code[9]));
                                                    Zaplon.SetState(ToBool(code[10]));
                                                    //LewyMigaczStaly.SetState(ToBool(code[11]));
                                                    //PrawyMigaczStaly.SetState(ToBool(code[12]));

                                                    Awaryjne.SetState(false);
                                                    Blinda.SetState(false);
                                                    LewyMigaczStaly.SetState(ToBool(code[2]));
                                                    PrawyMigaczStaly.SetState(ToBool(code[3]));

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
                                                bool on = ToBool(code[2]);
                                                Zaplon.SetState(on);
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
                                        case EEPROMDataType.T_MigaczCzas:
                                            {
                                                Snackbar.Make(view.FindViewById<LinearLayout>(Resource.Id.layout1), "Ustawiono: " +  code[2].ToString()+ " ms" , Snackbar.LengthLong).Show();
                                                break;
                                            }
                                        
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
                                        case EEPROMDataType.Migaczautooff:
                                            {
                                                bool on = ToBool(code[2]);
                                                if (on)
                                                {
                                                    Snackbar.Make(view.FindViewById<LinearLayout>(Resource.Id.layout1), "Włączono automatyczne wyłączanie migaczy", Snackbar.LengthLong).Show();
                                                }
                                                else
                                                {
                                                    Snackbar.Make(view.FindViewById<LinearLayout>(Resource.Id.layout1), "Wyłączono automatyczne wyłączanie migaczy", Snackbar.LengthLong).Show();
                                                }
                                                break;
                                            }
                                        case EEPROMDataType.TolerancjaPrzechyluMPU:
                                            {
                                                String txt = "Ustawiono tolerancję: ";
                                                try
                                                {
                                                    for (int i = 2; i < code.Length ; i++)
                                                    {
                                                        txt += code[i];
                                                    }
                                                }
                                                catch { }

                                                Snackbar.Make(view.FindViewById<LinearLayout>(Resource.Id.layout1),txt + " stopni", Snackbar.LengthLong).Show();
                                                break;
                                            }
                                        default:
                                            break;
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


        private void MigaczCzasSet_Click(object sender, EventArgs e)
        {
            InputMethodManager imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(MigaczCzasEditText.WindowToken, HideSoftInputFlags.None);


            string txt = MigaczCzasEditText.Text;
            var Layout = view.FindViewById<LinearLayout>(Resource.Id.layout1);
            int time = 0;
            
            
            if(!int.TryParse(txt, out time))
            {
                Snackbar.Make(Layout, "Wprowadź liczbę", Snackbar.LengthLong).Show();// Jest to w sumie nie możlliwe, bo kontrolka ma ustalone tylko cyfry, ale jakby co
                return;
            }

            if (!((time <= 2550) && (time >= 100)))
            {
                Snackbar.Make(Layout, "Wprowadź liczbę z zakresu 100 - 2550 ms", Snackbar.LengthLong).Show();
            }
            else
            {
                time = time / 10;
                string stime = time.ToString("D3");
                BeginWrite?.Invoke(new byte[] { (byte)Typ.EEProm, (byte)EEPROMDataType.T_MigaczCzas, byte.Parse(stime[0].ToString()) , byte.Parse(stime[1].ToString()), byte.Parse(stime[2].ToString()) });
            }
        }

        #region Funkcje do wysyłki

        private void Kierunkowe_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.SwiatloMijania, FromBool(Kierunkowe.NegState) });
        }

        private void Postojowe_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.Postojowka, FromBool(Postojowe.NegState) });
        }

        private void Drogowe_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.SwiatloDrogowe, FromBool(Drogowe.NegState) });
        }

        private void PrawyMigacz_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Migacz, (byte)Swiatlo.PrawyMigacz, FromBool(PrawyMigacz.NegState) });
        }

        private void LewyMigacz_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Migacz, (byte)Swiatlo.LewyMigacz, FromBool(LewyMigacz.NegState) });
        }

        private void Tyl_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.SwiatloTyl, FromBool(Tyl.NegState) });
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.Stop, FromBool(Stop.NegState) });
        }

        private void Awaryjne_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.Awaryjne, FromBool(Awaryjne.NegState) });
        }

        private void DzienNoc_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Dodatk, (byte)Dodatkowe.DzienNoc, FromBool(DzienNoc.NegState) });
        }

        private void Zaplon_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Dodatk, (byte)Dodatkowe.EngineStart, FromBool(Zaplon.NegState) });
        }
        private void PrawyMigaczStaly_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.PrawyMigacz, FromBool(PrawyMigaczStaly.NegState) });
        }

        private void LewyMigaczStaly_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.LewyMigacz, FromBool(LewyMigaczStaly.NegState) });
        }

        private void Blinda_Click(object sender, EventArgs e)
        {
            BeginWrite?.Invoke(new byte[] { (byte)Typ.Swiatla, (byte)Swiatlo.Blinda, FromBool(Blinda.NegState) });
        }

    }
    #endregion
}