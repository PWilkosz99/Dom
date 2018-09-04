using System;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Diagnostics;

namespace Dom_android
{
	public class BiurkoL : Android.Support.V4.App.Fragment
	{
        ProgressBar ConnectProgressBar = null;

        SeekBar A;
        SeekBar R;
        SeekBar G;
        SeekBar B;

        Android.Widget.Switch SKlawiatura, SGornaPolka, SDolnaPolka, SGornaSzafka, SDolnaSzafka, STyl;

        Part Klawiatura, GornaPolka, DolnaPolka, GornaSzafka, DolnaSzafka, Tyl, Dodatkowy;

        bool one = false;

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
			View view = inflater.Inflate(Resource.Layout.BiurkoL, container, false);

            ConnectProgressBar = view.FindViewById<ProgressBar>(Resource.Id.ConnectProgressBar);
            A = view.FindViewById<SeekBar>(Resource.Id.seekA);
            R = view.FindViewById<SeekBar>(Resource.Id.seekR);
            G = view.FindViewById<SeekBar>(Resource.Id.seekG);
            B = view.FindViewById<SeekBar>(Resource.Id.seekB);

            A.ProgressChanged += A_ProgressChanged;
            R.ProgressChanged += R_ProgressChanged;
            G.ProgressChanged += G_ProgressChanged;
            B.ProgressChanged += B_ProgressChanged;

            Client.BiurkoEvent += SyncResp;

            if (Client.Connected)
            {
                ConnectProgressBar.Visibility = ViewStates.Invisible;
            }
            else
            {
                Client.ConnectedEvent += Client_ConnectedEvent;
            }

            SKlawiatura = view.FindViewById<Android.Widget.Switch>(Resource.Id.p1);
            SGornaPolka = view.FindViewById<Android.Widget.Switch>(Resource.Id.p2);
            SDolnaPolka = view.FindViewById<Android.Widget.Switch>(Resource.Id.p3);
            SGornaSzafka = view.FindViewById<Android.Widget.Switch>(Resource.Id.p4);
            SDolnaSzafka = view.FindViewById<Android.Widget.Switch>(Resource.Id.p5);
            STyl = view.FindViewById<Android.Widget.Switch>(Resource.Id.p6);

            SKlawiatura.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) { Klawiatura.Set(e.IsChecked); };
            SGornaPolka.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) { GornaPolka.Set(e.IsChecked); };
            SDolnaPolka.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) { DolnaPolka.Set(e.IsChecked); };
            SGornaSzafka.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) { GornaSzafka.Set(e.IsChecked); };
            SDolnaSzafka.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) { DolnaSzafka.Set(e.IsChecked); };
            STyl.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) { Tyl.Set(); };




            Klawiatura = new Part(Commands.BiurkoEnum.Klawiatura);
            GornaPolka = new Part(Commands.BiurkoEnum.GornaPolka);
            DolnaPolka = new Part(Commands.BiurkoEnum.DolnaPolka);
            GornaSzafka = new Part(Commands.BiurkoEnum.GornaSzafka);
            DolnaSzafka = new Part(Commands.BiurkoEnum.DolnaSzafka);
            Tyl = new Part(Commands.BiurkoEnum.Tyl);
            Dodatkowy = new Part(Commands.BiurkoEnum.Dodatkowy);

            return view;
		}

        private void B_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            Set((byte)e.Progress, RGB.B);
        }

        private void G_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            Set((byte)e.Progress, RGB.G);
        }

        private void R_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            Set((byte)e.Progress, RGB.R);
        }

        private void A_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            Set((byte)(e.Progress));
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









        public void SyncResp(string[] Code)
        {
            Commands.BiurkoValueType Value = (Commands.BiurkoValueType)int.Parse(Code[1]);

            Console.WriteLine(Value);

            switch (Value)
            {
                case Commands.BiurkoValueType.Jasność:
                    {
                        byte tmp;
                        byte.TryParse(Code[2], out tmp);
                        alpha = tmp;

                        break;
                    }
                case Commands.BiurkoValueType.Kolor:
                    {
                        byte _r, _g, _b;

                        byte.TryParse(Code[2], out _r);
                        byte.TryParse(Code[3], out _g);
                        byte.TryParse(Code[4], out _b);

                        r = _r;
                        g = _g;
                        b = _b;

                        break;
                    }
                case Commands.BiurkoValueType.Część:
                    {
                        Commands.BiurkoEnum Part = (Commands.BiurkoEnum)int.Parse(Code[2]);

                        bool Power = bool.Parse(Code[3]);

                        /*switch (Part)
                        {
                            case Commands.BiurkoEnum.Klawiatura:
                                {
                                    Klawiatura.Power = Power;
                                    break;
                                }
                            case Commands.BiurkoEnum.GornaPolka:
                                {
                                    GornaPolka.Power = Power;
                                    break;
                                }
                            case Commands.BiurkoEnum.DolnaPolka:
                                {
                                    DolnaPolka.Power = Power;
                                    break;
                                }
                            case Commands.BiurkoEnum.GornaSzafka:
                                {
                                    GornaSzafka.Power = Power;
                                    break;
                                }
                            case Commands.BiurkoEnum.DolnaSzafka:
                                {
                                    DolnaSzafka.Power = Power;
                                    break;
                                }
                            case Commands.BiurkoEnum.Tyl:
                                {
                                    Tyl.Power = Power;
                                    break;
                                }
                            case Commands.BiurkoEnum.Dodatkowy:
                                {
                                    Dodatkowy.Power = Power;
                                    break;
                                }
                        }*/

                        break;
                    }
                case Commands.BiurkoValueType.Sync:
                    {
                        one = false;
                        Debug.WriteLine("RESPSync");

                        r = byte.Parse(Code[2]);
                        g = byte.Parse(Code[3]);
                        b = byte.Parse(Code[4]);
                        alpha = byte.Parse(Code[5]);
                        
                        Klawiatura.Power = bool.Parse(Code[6]);
                        GornaPolka.Power = bool.Parse(Code[7]);
                        DolnaPolka.Power = bool.Parse(Code[8]);
                        GornaSzafka.Power = bool.Parse(Code[9]);
                        DolnaSzafka.Power = bool.Parse(Code[10]);
                        Tyl.Power = bool.Parse(Code[11]);
                        Dodatkowy.Power = bool.Parse(Code[12]);
                        /*
                        if (Klawiatura.Power || GornaPolka.Power || DolnaPolka.Power || GornaSzafka.Power || DolnaSzafka.Power || Tyl.Power)
                        {
                            one = true;
                            PowerAll.IsChecked = true;

                        }*/
                        break;
                    }
            }


            
            A.Progress = alpha;
            R.Progress = r;
            G.Progress = g;
            B.Progress = b;


            SKlawiatura.Checked = Klawiatura.Power;
            SGornaPolka.Checked = GornaPolka.Power;
            SDolnaPolka.Checked = GornaPolka.Power;
            SGornaSzafka.Checked = GornaSzafka.Power;
            SDolnaSzafka.Checked = DolnaSzafka.Power;
            STyl.Checked = Tyl.Power;
           
            
        }




        public enum RGB
        {
            R = 1,
            G = 2,
            B = 3,
        }

        class Part
        {

            private Commands.BiurkoEnum ID;
            public bool Power;

            public Part(Commands.BiurkoEnum part)
            {
                ID = part;
            }

            public void Set()
            {
                Set(!Power);
            }
            public void Set(bool power)
            {
                Power = power;

                var data = ((int)Commands.BiurkoValueType.Część).ToString() + ";" + ((int)ID).ToString() + ";" + Power.ToString();
                Client.BiurkoSend(((int)Commands.Command.Biurko).ToString() + ";" + data);
            }


        }

        public byte r = 0, g = 0, b = 0, alpha = 0;

        private void Begin()
        {
            Write(((int)Commands.BiurkoValueType.Sync).ToString());
        }

        private void Write(string data)
        {
            Client.BiurkoSend(((int)Commands.Command.Biurko).ToString() + ";" + data);
        }

        private void Write(byte R, byte G, byte B)
        {
            Write(((int)Commands.BiurkoValueType.Kolor).ToString() + ";" + R.ToString() + ";" + G.ToString() + ";" + B.ToString());
        }

        private void Write(byte Alpha)
        {
            Write(((int)Commands.BiurkoValueType.Jasność) + ";" + Alpha.ToString());
        }

        private void Write(Commands.BiurkoEnum Part, bool Power)
        {
            Write(((int)Commands.BiurkoValueType.Część).ToString() + ";" + ((int)Part).ToString() + ";" + Power.ToString());
        }


        public bool Set(byte R, byte G, byte B)
        {
            // Paleta RGB   wartości od 0 do 255
            if (R > 255 || G > 255 || B > 255)
            {
                return false;
            }
            r = R;
            g = G;
            b = B;

            Biurko_ColorChanged(alpha, r, g, b);

            Write(r, g, b);

            Console.WriteLine("R: " + R + " G: " + G + " B: " + B);

            return true;
        }

        public bool Set(byte color, RGB rgb)
        {
            // Paleta RGB   wartości od 0 do 255
            if (color > 255)
            {
                return false;
            }
            switch (rgb)
            {
                case RGB.R: // podał tylko R
                    {
                        Set(color, g, b);
                        break;
                    }

                case RGB.G: // podał tylko G
                    {
                        Set(r, color, b);
                        break;
                    }
                case RGB.B: // podał tylko B
                    {
                        Set(r, g, color);
                        break;
                    }
            }

            return true;
        }

        public bool Set(byte Alpha)
        {
            // Jasność  od 0 do 100 %

            if (Alpha > 100)
            {
                return false;
            }
            alpha = Alpha;

            Biurko_ColorChanged(alpha, r, g, b);

            Write(alpha);

            Console.WriteLine("Alpha: " + alpha);
            return true;
        }


        private void Biurko_ColorChanged(byte a, byte r, byte g, byte b)
        {
            //Color(new Windows.UI.Xaml.Shapes.Path[] { gp1, gp2, dp1, dp2, gs1, gs2, ds1, ds2, tyl1, kl1, kl2 }, a, r, g, b);
        }












    }
}

