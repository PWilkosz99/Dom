using Dom.Podzespoły;
using System;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;

namespace Dom.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Biurko : Page
    {
        bool one = false;

        public Biurko()
        {
            this.InitializeComponent();

            Client.BiurkoEvent += SyncResp;

            Begin();

            if (Client.Connected)
            {
                ConnectProgressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                Client.ConnectedEvent += Client_ConnectedEvent;
            }

        }

        private void Client_ConnectedEvent(bool Connect)
        {
            if (Connect)
            {
                ConnectProgressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                ConnectProgressBar.Visibility = Visibility.Visible;
            }
        }

        private void Biurko_ColorChanged(byte a, byte r, byte g, byte b)
        {
            Color(new Windows.UI.Xaml.Shapes.Path[] { gp1, gp2, dp1, dp2, gs1, gs2, ds1, ds2, tyl1, kl1, kl2}, a, r, g, b);
        }

        private void A_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
           Set(Convert.ToByte(e.NewValue));
        }

        private void R_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Set(Convert.ToByte(e.NewValue), RGB.R);
        }

        private void G_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Set(Convert.ToByte(e.NewValue), RGB.G);
        }

        private void B_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Set(Convert.ToByte(e.NewValue), RGB.B);
        }

        private void Ds(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            DolnaSzafka.Set();
            if (DolnaSzafka.Power)
            {
                ds1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ds2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                ds1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ds2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void Gs(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            GornaSzafka.Set();
            if(GornaSzafka.Power)
            {
                gs1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                gs2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                gs1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                gs2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

        }

        private void Dp(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            DolnaPolka.Set();
            if (DolnaPolka.Power)
            {
                dp1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                dp2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                dp1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                dp2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void Gp(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            GornaPolka.Set();
            if (GornaPolka.Power)
            {
                gp1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                gp2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                gp1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                gp2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void Kl(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Klawiatura.Set();
            if (Klawiatura.Power)
            {
                kl1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                kl2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                kl1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                kl2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }        
        }

        private void Tl(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Tyl.Set();
            if (Tyl.Power)
            {
                tyl1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                tyl1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }            
        }

        private void Color(Windows.UI.Xaml.Shapes.Path[] Element, byte A, byte R, byte G, byte B)
        {
            LinearGradientBrush Gradient = new LinearGradientBrush();
            Gradient.StartPoint = new Windows.Foundation.Point(-0.027, 0.021);
            Gradient.EndPoint = new Windows.Foundation.Point(0.619, 0.84);
             
            
            GradientStop One = new GradientStop();
            One.Color = Windows.UI.Color.FromArgb(A, R, G, B);
            One.Offset = 0.3;
            
            if(A < 31)
            {
                A = 1;
            }
            else
            {
                A = Convert.ToByte(Convert.ToInt32(A) - 31);
            }                  

            GradientStop Two = new GradientStop();
            Two.Color = Windows.UI.Color.FromArgb(A , R, G, B);
            Two.Offset = 1; 

            Gradient.GradientStops.Add(One);
            Gradient.GradientStops.Add(Two);

            for(int i = 0; i < Element.Length;i++)
            {
                if (Element[i].Visibility == Windows.UI.Xaml.Visibility.Visible)  // najpierw widoczne elementy
                {
                    Element[i].Fill = Gradient;
                }
            }

            for (int i = 0; i < Element.Length; i++)
            {
                if (Element[i].Visibility == Windows.UI.Xaml.Visibility.Collapsed)  // potem niewidoczne elementy
                {
                    Element[i].Fill = Gradient;
                }
            }
        }
        
        public void SyncResp(string[] Code)
        {
            Commands.TypDanychBiurka Value = (Commands.TypDanychBiurka)int.Parse(Code[1]);

            Debug.WriteLine(Value);

            switch (Value)
            {
                case Commands.TypDanychBiurka.Jasność:
                    {
                        byte tmp;
                        byte.TryParse(Code[2], out tmp);
                        alpha = tmp;                       
                        
                        break;
                    }
                case Commands.TypDanychBiurka.Kolor:
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
                case Commands.TypDanychBiurka.Część:
                    {
                        Commands.CzęściBiurka Part = (Commands.CzęściBiurka)int.Parse(Code[2]);

                        bool Power = bool.Parse(Code[3]);

                        switch (Part)
                        {
                            case Commands.CzęściBiurka.Klawiatura:
                                {
                                    Klawiatura.Power = Power;
                                    break;
                                }
                            case Commands.CzęściBiurka.GornaPolka:
                                {
                                    GornaPolka.Power = Power;
                                    break;
                                }
                            case Commands.CzęściBiurka.DolnaPolka:
                                {
                                    DolnaPolka.Power = Power;
                                    break;
                                }
                            case Commands.CzęściBiurka.GornaSzafka:
                                {
                                    GornaSzafka.Power = Power;
                                    break;
                                }
                            case Commands.CzęściBiurka.DolnaSzafka:
                                {
                                    DolnaSzafka.Power = Power;
                                    break;
                                }
                            case Commands.CzęściBiurka.Tyl:
                                {
                                    Tyl.Power = Power;
                                    break;
                                }
                            case Commands.CzęściBiurka.Dodatkowy:
                                {
                                    Dodatkowy.Power = Power;
                                    break;
                                }
                        }

                        break;
                    }
                case Commands.TypDanychBiurka.Sync:
                    {
                        one = false;

                        // Sync?.Invoke(r, g, b, alpha, Klawiatura.Power, GornaPolka.Power, DolnaPolka.Power, GornaSzafka.Power, DolnaSzafka.Power, Tyl.Power, Dodatkowy.Power);

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

                        if(Klawiatura.Power || GornaPolka.Power || DolnaPolka.Power || GornaSzafka.Power || DolnaSzafka.Power|| Tyl.Power)
                        {
                            one = true;
                            PowerAll.IsChecked = true;
                            
                        }
                        break;
                    }
            }



            A.Value = alpha;
            R.Value = r;
            G.Value = g;
            B.Value = b;



            if (DolnaSzafka.Power)
            {
                ds1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ds2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                ds1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ds2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            if (GornaSzafka.Power)
            {
                gs1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                gs2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                gs1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                gs2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            if (DolnaPolka.Power)
            {
                dp1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                dp2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                dp1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                dp2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            if (GornaPolka.Power)
            {
                gp1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                gp2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                gp1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                gp2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            if (Klawiatura.Power)
            {
                kl1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                kl2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                kl1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                kl2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }


            if (Tyl.Power)
            {
                tyl1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                tyl1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if(one)
            {
                one = false;
                return;
            }

            DolnaSzafka.Set(true);
            ds1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            ds2.Visibility = Windows.UI.Xaml.Visibility.Visible;


           GornaSzafka.Set(true);
            gs1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            gs2.Visibility = Windows.UI.Xaml.Visibility.Visible;


           DolnaPolka.Set(true);
            dp1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            dp2.Visibility = Windows.UI.Xaml.Visibility.Visible;


           GornaPolka.Set(true);
            gp1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            gp2.Visibility = Windows.UI.Xaml.Visibility.Visible;


           Klawiatura.Set(true);
            kl1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            kl2.Visibility = Windows.UI.Xaml.Visibility.Visible;

           Tyl.Set(true);
            tyl1.Visibility = Windows.UI.Xaml.Visibility.Visible;

        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)  // wylacz wszystko
        {

           DolnaSzafka.Set(false);
            ds1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ds2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;


           GornaSzafka.Set(false);
            gs1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            gs2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            
            
           DolnaPolka.Set(false);
            dp1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            dp2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            
           GornaPolka.Set(false);
            gp1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            gp2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            
           Klawiatura.Set(false);
            kl1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            kl2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            
           Tyl.Set(false);
            tyl1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }



        //-------------------------------------------------------


          Part Klawiatura = new Part(Commands.CzęściBiurka.Klawiatura);
          Part GornaPolka = new Part(Commands.CzęściBiurka.GornaPolka);
          Part DolnaPolka = new Part(Commands.CzęściBiurka.DolnaPolka);
          Part GornaSzafka = new Part(Commands.CzęściBiurka.GornaSzafka);
          Part DolnaSzafka = new Part(Commands.CzęściBiurka.DolnaSzafka);
          Part Tyl = new Part(Commands.CzęściBiurka.Tyl);
          Part Dodatkowy = new Part(Commands.CzęściBiurka.Dodatkowy);





        public enum RGB
        {
            R = 1,
            G = 2,
            B = 3,
        }

         class Part
        {

            private Commands.CzęściBiurka ID;
            public bool Power;

            public Part(Commands.CzęściBiurka part)
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

                var data = ((int)Commands.TypDanychBiurka.Część).ToString() + ";" + ((int)ID).ToString() + ";" + Power.ToString();
                Client.BiurkoSend(((int)Commands.Moduł.Biurko).ToString() + ";" + data);
            }


        }

        public  byte r = 0, g = 0, b = 0, alpha = 0;

        private void Begin()
        {
            Write(((int)Commands.TypDanychBiurka.Sync).ToString());
        }

        private void Write(string data)
        {
            Client.BiurkoSend(((int)Commands.Moduł.Biurko).ToString() + ";" + data);
            Debug.WriteLine("Nie z klasy");
        }

        private void Write(byte R, byte G, byte B)
        {
            Write(((int)Commands.TypDanychBiurka.Kolor).ToString() + ";" + R.ToString() + ";" + G.ToString() + ";" + B.ToString());
        }

        private void Write(byte Alpha)
        {
            Write(((int)Commands.TypDanychBiurka.Jasność) + ";" + Alpha.ToString());
        }

        private void Write(Commands.CzęściBiurka Part, bool Power)
        {
            Write(((int)Commands.TypDanychBiurka.Część).ToString() + ";" + ((int)Part).ToString() + ";" + Power.ToString());
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

            Debug.WriteLine("R: " + R + " G: " + G + " B: " + B);

            return true;
        }

        public  bool Set(byte color, RGB rgb)
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

        public  bool Set(byte Alpha)
        {
            // Jasność  od 0 do 100 %

            if (Alpha > 100)
            {
                return false;
            }
            alpha = Alpha;

            Biurko_ColorChanged(alpha, r, g, b);

            Write(alpha);

            Debug.WriteLine("Alpha: " + alpha);
            return true;
        }

    }    
}