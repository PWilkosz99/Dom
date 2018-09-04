using System;
using System.Diagnostics;

namespace Dom.Podzespoły
{
    class Biurkoo
    {               
        public enum RGB
        {
            R = 1,
            G = 2, 
            B = 3,
        }

        public class Part
        {            

            private Commands.CzęściBiurka ID;            
            public bool Power;

            public Part(Commands.CzęściBiurka Part)
            {
                ID =  Part;
            }

            public void Set()
            {
                Set(!Power);
            }
            public void Set(bool power)
            {
                Power = power;
                Write(ID, Power);
            }                          
               
        }

        public delegate void ColorChangedEventHandler (byte a, byte r, byte g, byte b);
        public static event ColorChangedEventHandler ColorChanged;

        public delegate void SyncEventHandler(string[] Code);
        public static event SyncEventHandler Synchronize;

        public static byte r = 0, g = 0, b = 0, alpha = 0;

        static public Part Klawiatura = new Part(Commands.CzęściBiurka.Klawiatura);
        static public Part GornaPolka = new Part(Commands.CzęściBiurka.GornaPolka);
        static public Part DolnaPolka = new Part(Commands.CzęściBiurka.DolnaPolka);
        static public Part GornaSzafka = new Part(Commands.CzęściBiurka.GornaSzafka);
        static public Part DolnaSzafka = new Part(Commands.CzęściBiurka.DolnaSzafka);
        static public Part Tyl = new Part(Commands.CzęściBiurka.Tyl);
        static public Part Dodatkowy = new Part(Commands.CzęściBiurka.Dodatkowy);

        
        private static async void Write(string data)
        {
           // Client.BiurkoSend( ((int)Commands.Command.Biurko).ToString() + ";" + data  );
           // Debug.WriteLine(data);
        }

        private static async void Write(byte R, byte G, byte B)
        {
            Write(((int)Commands.TypDanychBiurka.Kolor).ToString() + ";" + R.ToString() + ";" + G.ToString() + ";" + B.ToString());
        }

        private static async void Write(byte Alpha)
        {
            Write(((int)Commands.TypDanychBiurka.Jasność) + ";" + Alpha.ToString());
        }

        private static async void Write(Commands.CzęściBiurka Part, bool Power)
        {
            Write( ((int)Commands.TypDanychBiurka.Część).ToString() + ";" + ((int)Part).ToString() + ";" + Power.ToString());
        }
        
        public  static bool Set(byte R, byte G, byte B)
        {
            // Paleta RGB   wartości od 0 do 255
            if(R > 255 || G > 255 || B > 255 )
            {
                return false;
            }
            r = R;
            g = G;
            b = B;

            ColorChanged?.Invoke(alpha, r, g, b);

            Write(r, g, b);

            Debug.WriteLine("R: " + R + " G: " + G + " B: " + B);

            return true;
        }

        public static bool Set(byte color, RGB rgb)
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
        
        public  static bool Set(byte Alpha)
        {
            // Jasność  od 0 do 100 %

            if(Alpha > 100)
            {
                return false;
            }
            alpha = Alpha;

            ColorChanged?.Invoke(alpha, r, g, b);

            Write(alpha);

            Debug.WriteLine("Alpha: " + alpha);
            return true;
        }

        public static void Begin()
        {
            Write(((int)Commands.TypDanychBiurka.Sync).ToString());
        }

        public static void SyncResp(string[] Code)
        {
           // Synchronize?.Invoke(Code);     
        }
    }
}