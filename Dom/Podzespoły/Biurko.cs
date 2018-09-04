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

            private Commands.BiurkoEnum ID;            
            public bool Power;

            public Part(Commands.BiurkoEnum Part)
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

        static public Part Klawiatura = new Part(Commands.BiurkoEnum.Klawiatura);
        static public Part GornaPolka = new Part(Commands.BiurkoEnum.GornaPolka);
        static public Part DolnaPolka = new Part(Commands.BiurkoEnum.DolnaPolka);
        static public Part GornaSzafka = new Part(Commands.BiurkoEnum.GornaSzafka);
        static public Part DolnaSzafka = new Part(Commands.BiurkoEnum.DolnaSzafka);
        static public Part Tyl = new Part(Commands.BiurkoEnum.Tyl);
        static public Part Dodatkowy = new Part(Commands.BiurkoEnum.Dodatkowy);

        
        private static async void Write(string data)
        {
           // Client.BiurkoSend( ((int)Commands.Command.Biurko).ToString() + ";" + data  );
           // Debug.WriteLine(data);
        }

        private static async void Write(byte R, byte G, byte B)
        {
            Write(((int)Commands.BiurkoValueType.Kolor).ToString() + ";" + R.ToString() + ";" + G.ToString() + ";" + B.ToString());
        }

        private static async void Write(byte Alpha)
        {
            Write(((int)Commands.BiurkoValueType.Jasność) + ";" + Alpha.ToString());
        }

        private static async void Write(Commands.BiurkoEnum Part, bool Power)
        {
            Write( ((int)Commands.BiurkoValueType.Część).ToString() + ";" + ((int)Part).ToString() + ";" + Power.ToString());
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
            Write(((int)Commands.BiurkoValueType.Sync).ToString());
        }

        public static void SyncResp(string[] Code)
        {
           // Synchronize?.Invoke(Code);     
        }
    }
}