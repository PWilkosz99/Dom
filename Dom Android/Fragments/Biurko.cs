using System;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using Android.App;

namespace Dom_android
{
	public class Biurko : Android.Support.V4.App.Fragment
	{
        ProgressBar ConnectProgressBar = null;

        

        static Dictionary<Commands.CzęściBiurka, Part> ListaCzesciBiurka = new Dictionary<Commands.CzęściBiurka, Part>();

        class Part
        {
            private bool State;
            private Commands.CzęściBiurka ID;
            private Android.Widget.Switch kontrolka;
            private Activity activity;
            //public Android.Widget.Switch KontrolkaUI { get { return kontrolka; } }

            public Part(Commands.CzęściBiurka part, Android.Widget.Switch Kontrolka, Activity activity)
            {
                ID = part;
                kontrolka = Kontrolka;
                this.activity = activity;
                State = kontrolka.Checked;
                kontrolka.CheckedChange += Kontrolka_CheckedChange;
            }

            private void Kontrolka_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                new Thread(new ThreadStart(() =>
                {
                bool state = e.IsChecked;
                if(state == State)
                {
                    return;// jeśli poprzedni stan jest taki jak aktualny
                }
                State = state;
                string data = ((int)Commands.TypDanychBiurka.Część).ToString() + ";" + ((int)ID).ToString() + ";" + state.ToString();
                Client.Write(data, Commands.Moduł.Biurko);
                })).Start();
                return;
            }

            public void UpdateUI(string _state)
            {
                new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        bool state = bool.Parse(_state);
                        if (state == State)
                        {
                            return;// jeśli poprzedni stan jest taki jak aktualny
                        }
                        State = state;
                        activity.RunOnUiThread(() => kontrolka.Checked = state);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }

                })).Start();
                return;
            }

        }

        static Dictionary<Kolory.Kolor, Kolory> ListaKolory = new Dictionary<Kolory.Kolor, Kolory>();

        public class Kolory
        {
            public enum Kolor
            {
                R = 1,
                G = 2,
                B = 3,
                A = 4,
            }
            public int Wartosc { get { return wartosc; } }
            Kolor ID;
            SeekBar kontrolka;
            int wartosc;
            Activity activity;
            bool brk = false;

            public Kolory(Kolor Part, SeekBar Kontrolka, Activity activity)
            {
                ID = Part;
                kontrolka = Kontrolka;
                wartosc = kontrolka.Progress;
                this.activity = activity;
                kontrolka.ProgressChanged += Kontrolka_ProgressChanged;
            }

            private void Kontrolka_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
            {
                if(brk)
                {
                    brk = false;
                    return;
                }
                if(wartosc == e.Progress)
                {
                    return;
                }
                wartosc = e.Progress;
                WriteKolor(ID);
            }

            public void UpdateUI(string Value)
            {
                new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        int value = int.Parse(Value);
                        if (wartosc == value)
                        {
                            return;
                        }

                        wartosc = value;
                        brk = true;
                        activity.RunOnUiThread(() => kontrolka.Progress = value);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                })).Start();
            }
        }

        static public void WriteKolor(Kolory.Kolor Id)
        {
            new Thread(new ThreadStart(() =>
            {
                if (Id == Kolory.Kolor.A)
                {
                    Client.Write(((int)Commands.TypDanychBiurka.Jasność).ToString() + ";" + (ListaKolory[Kolory.Kolor.A].Wartosc).ToString(), Commands.Moduł.Biurko);
                }
                else
                {
                    Client.Write(((int)Commands.TypDanychBiurka.Kolor).ToString() + ";" + (ListaKolory[Kolory.Kolor.R].Wartosc).ToString() + ";" + (ListaKolory[Kolory.Kolor.G].Wartosc).ToString() + ";" + (ListaKolory[Kolory.Kolor.B].Wartosc).ToString(), Commands.Moduł.Biurko);
                }
            })).Start();
        }

        //SeekBar A;
        //SeekBar R;
        //SeekBar G;
        //SeekBar B;

        //public byte r = 0, g = 0, b = 0, alpha = 0;

        public override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

        public override void OnResume()
        {
            Begin();
            base.OnResume();
           
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.Biurko, container, false);

            ConnectProgressBar = view.FindViewById<ProgressBar>(Resource.Id.ConnectProgressBar);


            ListaKolory.Add(Kolory.Kolor.A, new Kolory(Kolory.Kolor.A, view.FindViewById<SeekBar>(Resource.Id.seekA), Activity));
            ListaKolory.Add(Kolory.Kolor.R, new Kolory(Kolory.Kolor.R, view.FindViewById<SeekBar>(Resource.Id.seekR), Activity));
            ListaKolory.Add(Kolory.Kolor.G, new Kolory(Kolory.Kolor.G, view.FindViewById<SeekBar>(Resource.Id.seekG), Activity));
            ListaKolory.Add(Kolory.Kolor.B, new Kolory(Kolory.Kolor.G, view.FindViewById<SeekBar>(Resource.Id.seekB), Activity));


            //A = view.FindViewById<SeekBar>(Resource.Id.seekA);
            //R = view.FindViewById<SeekBar>(Resource.Id.seekR);
            //G = view.FindViewById<SeekBar>(Resource.Id.seekG);
            //B = view.FindViewById<SeekBar>(Resource.Id.seekB);

            //A.ProgressChanged += A_ProgressChanged;
            //R.ProgressChanged += R_ProgressChanged;
            //G.ProgressChanged += G_ProgressChanged;
            //B.ProgressChanged += B_ProgressChanged;

            Client.BiurkoEvent += SyncResp;
            Client_ConnectedEvent(Client.Connected);
            Client.ConnectedEvent += Client_ConnectedEvent;

            //Cześci biurka lista od 1 do 7

            ListaCzesciBiurka.Add(Commands.CzęściBiurka.Klawiatura, new Part(Commands.CzęściBiurka.Klawiatura, view.FindViewById<Android.Widget.Switch>(Resource.Id.p1), Activity));
            ListaCzesciBiurka.Add(Commands.CzęściBiurka.GornaPolka, new Part(Commands.CzęściBiurka.GornaPolka, view.FindViewById<Android.Widget.Switch>(Resource.Id.p2), Activity));
            ListaCzesciBiurka.Add(Commands.CzęściBiurka.DolnaPolka, new Part(Commands.CzęściBiurka.DolnaPolka, view.FindViewById<Android.Widget.Switch>(Resource.Id.p3), Activity));
            ListaCzesciBiurka.Add(Commands.CzęściBiurka.GornaSzafka, new Part(Commands.CzęściBiurka.GornaSzafka, view.FindViewById<Android.Widget.Switch>(Resource.Id.p4), Activity));
            ListaCzesciBiurka.Add(Commands.CzęściBiurka.DolnaSzafka, new Part(Commands.CzęściBiurka.DolnaSzafka, view.FindViewById<Android.Widget.Switch>(Resource.Id.p5), Activity));
            ListaCzesciBiurka.Add(Commands.CzęściBiurka.Tyl, new Part(Commands.CzęściBiurka.Tyl, view.FindViewById<Android.Widget.Switch>(Resource.Id.p6), Activity));

            return view;
		}

        //#region Paleta kolorów UI -> Stream

        //private void B_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        //{
        //    Set((byte)e.Progress, RGB.B);
        //}

        //private void G_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        //{
        //    Set((byte)e.Progress, RGB.G);
        //}

        //private void R_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        //{
        //    Set((byte)e.Progress, RGB.R);
        //}

        //private void A_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        //{
        //    Set((byte)(e.Progress));
        //}

        //#endregion

        private void Client_ConnectedEvent(bool Connect)
        {
            Activity.RunOnUiThread(() =>
            {
                if (ConnectProgressBar == null)
                {
                    return;
                }
                if (Connect)
                {
                    ConnectProgressBar.Visibility = ViewStates.Invisible;
                }
                else
                {
                    ConnectProgressBar.Visibility = ViewStates.Visible;
                }
            });
        }


        public void SyncResp(string[] Code)
        {
            new Thread(new ThreadStart(() =>
            {
                Commands.TypDanychBiurka Value = (Commands.TypDanychBiurka)int.Parse(Code[1]);

                Console.WriteLine(Value);

                switch (Value)
                {
                    //case Commands.TypDanychBiurka.Jasność:
                    //    {
                    //        byte tmp;
                    //        byte.TryParse(Code[2], out tmp);
                    //        alpha = tmp;

                    //        break;
                    //    }
                    //case Commands.TypDanychBiurka.Kolor:
                    //    {
                    //        byte _r, _g, _b;

                    //        byte.TryParse(Code[2], out _r);
                    //        byte.TryParse(Code[3], out _g);
                    //        byte.TryParse(Code[4], out _b);

                    //        r = _r;
                    //        g = _g;
                    //        b = _b;

                    //        break;
                    //    }
                    //case Commands.TypDanychBiurka.Część:
                    //    {
                    //        Commands.CzęściBiurka Part = (Commands.CzęściBiurka)int.Parse(Code[2]);

                    //        bool Power = bool.Parse(Code[3]);

                    //        /*switch (Part)
                    //        {
                    //            case Commands.BiurkoEnum.Klawiatura:
                    //                {
                    //                    Klawiatura.Power = Power;
                    //                    break;
                    //                }
                    //            case Commands.BiurkoEnum.GornaPolka:
                    //                {
                    //                    GornaPolka.Power = Power;
                    //                    break;
                    //                }
                    //            case Commands.BiurkoEnum.DolnaPolka:
                    //                {
                    //                    DolnaPolka.Power = Power;
                    //                    break;
                    //                }
                    //            case Commands.BiurkoEnum.GornaSzafka:
                    //                {
                    //                    GornaSzafka.Power = Power;
                    //                    break;
                    //                }
                    //            case Commands.BiurkoEnum.DolnaSzafka:
                    //                {
                    //                    DolnaSzafka.Power = Power;
                    //                    break;
                    //                }
                    //            case Commands.BiurkoEnum.Tyl:
                    //                {
                    //                    Tyl.Power = Power;
                    //                    break;
                    //                }
                    //            case Commands.BiurkoEnum.Dodatkowy:
                    //                {
                    //                    Dodatkowy.Power = Power;
                    //                    break;
                    //                }
                    //        }*/

                    //        break;
                    //    }
                    case Commands.TypDanychBiurka.Sync:
                        {
                            ListaKolory[Kolory.Kolor.R].UpdateUI(Code[2]);
                            ListaKolory[Kolory.Kolor.G].UpdateUI(Code[3]);
                            ListaKolory[Kolory.Kolor.B].UpdateUI(Code[4]);
                            ListaKolory[Kolory.Kolor.A].UpdateUI(Code[5]);

                            ListaCzesciBiurka[Commands.CzęściBiurka.Klawiatura].UpdateUI(Code[6]);
                            ListaCzesciBiurka[Commands.CzęściBiurka.GornaPolka].UpdateUI(Code[7]);
                            ListaCzesciBiurka[Commands.CzęściBiurka.DolnaPolka].UpdateUI(Code[8]);
                            ListaCzesciBiurka[Commands.CzęściBiurka.GornaSzafka].UpdateUI(Code[9]);
                            ListaCzesciBiurka[Commands.CzęściBiurka.DolnaSzafka].UpdateUI(Code[10]);
                            ListaCzesciBiurka[Commands.CzęściBiurka.Tyl].UpdateUI(Code[11]);                            
                            //ListaCzesciBiurka[Commands.CzęściBiurka.Dodatkowy].UpdateUI(Code[12]);

                            Debug.WriteLine("RESPSync");

                            //r = byte.Parse(Code[2]);
                            //g = byte.Parse(Code[3]);
                            //b = byte.Parse(Code[4]);
                            //alpha = byte.Parse(Code[5]);

                            //Klawiatura.Power = bool.Parse(Code[6]);
                            //GornaPolka.Power = bool.Parse(Code[7]);
                            //DolnaPolka.Power = bool.Parse(Code[8]);
                            //GornaSzafka.Power = bool.Parse(Code[9]);
                            //DolnaSzafka.Power = bool.Parse(Code[10]);
                            //Tyl.Power = bool.Parse(Code[11]);
                            //Dodatkowy.Power = bool.Parse(Code[12]);
                            break;
                        }
                }


              //  Activity.RunOnUiThread(() =>  // Update UI
              //  {
                    //A.Progress = alpha;
                    //R.Progress = r;
                    //G.Progress = g;
                    //B.Progress = b;

                    //SKlawiatura.Checked = Klawiatura.Power;
                    //SGornaPolka.Checked = GornaPolka.Power;
                    //SDolnaPolka.Checked = GornaPolka.Power;
                    //SGornaSzafka.Checked = GornaSzafka.Power;
                    //SDolnaSzafka.Checked = DolnaSzafka.Power;
                    //STyl.Checked = Tyl.Power;
               // });
            })).Start();
        }

        //public enum RGB
        //{
        //    R = 1,
        //    G = 2,
        //    B = 3,
        //}

        



        private void Begin()
        {
            Client.Write(((int)Commands.TypDanychBiurka.Sync).ToString(), Commands.Moduł.Biurko);
        }

        //private void Write(string data)
        //{
        //    Client.Write(data, Commands.Moduł.Biurko);
        //}

        //private void Write(byte R, byte G, byte B)
        //{
        //    Write(((int)Commands.TypDanychBiurka.Kolor).ToString() + ";" + R.ToString() + ";" + G.ToString() + ";" + B.ToString());
        //}

        //private void Write(byte Alpha)
        //{
        //    Write(((int)Commands.TypDanychBiurka.Jasność) + ";" + Alpha.ToString());
        //}

        //private void Write(Commands.CzęściBiurka Part, bool Power)
        //{
        //    Write(((int)Commands.TypDanychBiurka.Część).ToString() + ";" + ((int)Part).ToString() + ";" + Power.ToString());
        //}


        //public bool Set(byte R, byte G, byte B)
        //{
        //    // Paleta RGB   wartości od 0 do 255
        //    if (R > 255 || G > 255 || B > 255)
        //    {
        //        return false;
        //    }
        //    r = R;
        //    g = G;
        //    b = B;

        //    Write(r, g, b);

        //    Console.WriteLine("R: " + R + " G: " + G + " B: " + B);

        //    return true;
        //}

        //public bool Set(byte color, RGB rgb)
        //{
        //    // Paleta RGB   wartości od 0 do 255
        //    if (color > 255)
        //    {
        //        return false;
        //    }
        //    switch (rgb)
        //    {
        //        case RGB.R: // podał tylko R
        //            {
        //                Set(color, g, b);
        //                break;
        //            }

        //        case RGB.G: // podał tylko G
        //            {
        //                Set(r, color, b);
        //                break;
        //            }
        //        case RGB.B: // podał tylko B
        //            {
        //                Set(r, g, color);
        //                break;
        //            }
        //    }

        //    return true;
        //}

        //public bool Set(byte Alpha)
        //{
        //    // Jasność  od 0 do 100 %

        //    if (Alpha > 100)
        //    {
        //        return false;
        //    }
        //    alpha = Alpha;

        //    Write(alpha);

        //    Console.WriteLine("Alpha: " + alpha);
        //    return true;
        //}
    }
}

