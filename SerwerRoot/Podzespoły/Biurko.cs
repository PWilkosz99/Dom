using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace SerwerRoot.Podzespoły
{
    class Biurko : ModuleBody
    {
        public static App.ModulesId id = App.ModulesId.Biurko;

        public Biurko()
        {
            Id = App.ModulesId.Biurko;
            Title = "Biurko Led";
            ModuleBodyWork();
        }

        public override void Start()
        {

            base.Start();
            BeginAsync();
        }

        /// <summary>
        /// Dane o kolorze oraz jasności
        /// </summary>
        private static byte r = 0, g = 0, b = 0, alpha = 0;

        /// <summary>
        /// Klasa przechowująca dane o każdej części
        /// </summary>
        public class Part
        {
            private Commands.CzęściBiurka ID;

            private bool power;

            public bool Power { get { return power; } }
            public string SPower { get { return power.ToString(); } }

            public Part(Commands.CzęściBiurka Part)
            {
                ID = Part;
            }


            /// <summary>
            /// Zmiania stanu
            /// </summary>
            public void Set()
            {
                power = !power;
                Set(power);
            }  

            ///<summary>
            //////Funkcja do ustawiania konkretnych partii biurka
            /// </summary>
            public void Set(bool Power)
            {
                power = Power;
                WriteToDeviceAsync("=|" + ((short)ID) + "|" + Convert.ToInt16(power) + "\n");
            }

        }

        /// <summary>
        /// Zgłoszenie do wysłania danych
        /// </summary>
        public static event BiurkoWriteEventHandler BiurkoWrite;
        public delegate void BiurkoWriteEventHandler(string Text, int ExceptId);
    
        /// <summary>
        /// Blokada przed czynnościami na stream ie podczas pracy w funkcji BeginAsync
        /// </summary>
        static ManualResetEvent BeginWorker = new ManualResetEvent(false);

        /// <summary>
        /// Częsci biurka w tablicy o ID CzęściBiurka
        /// </summary>
        static Dictionary<Commands.CzęściBiurka, Part> Czesci = new Dictionary<Commands.CzęściBiurka, Part>();

        /// <summary>
        /// Stream
        /// </summary>
        private static DataWriter DataWriter;

        /// <summary>
        /// Zmienna przechowująca id klienta z  żadaniem, w celu nie wysłania temu klientowi inforamcji zwrotnej
        /// </summary>
        private static int LastIdRequest;

        /// <summary>
        /// Połącz z modułem wykonawczym LED
        /// </summary>
        public static async void BeginAsync()
        {
            BeginWorker.Reset();


            try
            {
                Czesci.Add(Commands.CzęściBiurka.Klawiatura, new Part(Commands.CzęściBiurka.Klawiatura));
                Czesci.Add(Commands.CzęściBiurka.GornaPolka, new Part(Commands.CzęściBiurka.GornaPolka));
                Czesci.Add(Commands.CzęściBiurka.DolnaPolka, new Part(Commands.CzęściBiurka.DolnaPolka));
                Czesci.Add(Commands.CzęściBiurka.GornaSzafka, new Part(Commands.CzęściBiurka.GornaSzafka));
                Czesci.Add(Commands.CzęściBiurka.DolnaSzafka, new Part(Commands.CzęściBiurka.DolnaSzafka));
                Czesci.Add(Commands.CzęściBiurka.Tyl, new Part(Commands.CzęściBiurka.Tyl));
                Czesci.Add(Commands.CzęściBiurka.Dodatkowy, new Part(Commands.CzęściBiurka.Dodatkowy));
            }
            catch
            {
                Log.Write("Kolejne wywołanie funkcji wejściowej Biurko", true);
                return;
            }

            Log.Write("Biurko Begin");
            DeviceInformationCollection DIC = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector("UART0"));
            SerialDevice serialPort;
            try
            {
                serialPort = await SerialDevice.FromIdAsync(DIC[0].Id);
                // ...
                // Configure serial settings
                serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.BaudRate = 115200;
                serialPort.Parity = SerialParity.None;
                serialPort.StopBits = SerialStopBitCount.One;
                serialPort.DataBits = 8;

                DataWriter = new DataWriter(serialPort.OutputStream);
            }
            catch
            {
                Log.Write("Błąd podczas otwarcia komunikacji z modułem wykonawczym sterowania LED");
            }
            BeginWorker.Set();
        }

        /// <summary>
        /// Funkcja wysyłająca dane do modułu LED
        /// </summary>
        /// <param name="TxData"></param>
        private static async void WriteToDeviceAsync(String TxData)
        {
            BeginWorker.WaitOne();
            try
            {
                Sync(false);  // Zaaktualizuj UI apki, oprócz tej co wysłała żądanie
                DataWriter.WriteString(TxData);
                await DataWriter.StoreAsync();                
            }
            catch(Exception e)
            {
                Log.Write(e);
                BeginAsync();
            }
        }

        /// <summary>
        /// Funkcja do ustawiania Koloru
        /// </summary>
        public  static bool Set(byte R, byte G, byte B)
        {

            if(R > 255 || G > 255 || B > 255 )
            {
                Log.Write("Wartości RGB poza zakresem; R - " + R + ", G - " + G + ", B - " + B);
                return false;
            }
            r = R;
            g = G;
            b = B;
            WriteToDeviceAsync("+|" + Code(R) + "|" + Code(G) + "|" + Code(B) + "\n");


            return true;
        }

        ///<summary>
        ///Funkcja do ustawiania jasności
        /// </summary>
        public  static bool Set(byte Alpha)
        {
            if(alpha > 255)
            {
                Log.Write("Wartość alpha poza zakresem, "+ alpha);
                return false;
            }

            alpha = Alpha;

            WriteToDeviceAsync("-|" + Code(Alpha) + "\n");


            return true;
        }        

        /// <summary>
        /// Wypełnienie zamiast zerem znakiem '?'
        /// </summary>
        /// <param name="code">Wartość do uzupełnienia</param>
        private static string Code(short code)
        {
            if(code.ToString().Length == 1)  // jak 1 cyfra
            {
                return "??" + code;
            }

            if (code.ToString().Length == 2)  // jak 2 cyfry
            {
                return "?" + code;
            }

            return code.ToString();            
        }

        /// <summary>
        /// Przetwórz polecenia
        /// </summary>
        /// <param name="Code">Surowe dane</param>
        /// <param name="id">Id klienta z żądaniem</param>
        public static void PrzetworzZadanie(String [] Code, int id)
        {
            Commands.TypDanychBiurka Value = (Commands.TypDanychBiurka)int.Parse(Code[2]);

            switch (Value)
            {
                case Commands.TypDanychBiurka.Jasność:
                    {
                        byte alpha;
                        byte.TryParse(Code[3], out alpha);
                        LastIdRequest = id;
                        Set(alpha);
                        break;
                    }
                case Commands.TypDanychBiurka.Kolor:
                    {
                        byte _r, _g, _b;

                        byte.TryParse(Code[3], out _r);
                        byte.TryParse(Code[4], out _g);
                        byte.TryParse(Code[5], out _b);
                        LastIdRequest = id;
                        Set(_r, _g, _b);

                        break;
                    }
                case Commands.TypDanychBiurka.Część:
                    {
                        Commands.CzęściBiurka Part = (Commands.CzęściBiurka)int.Parse(Code[3]);
                        bool Power = bool.Parse(Code[4]);
                        LastIdRequest = id;
                        Czesci[Part].Set(Power);
                        break;
                    }
                case Commands.TypDanychBiurka.Sync:
                    {
                        Sync(true);
                        break;
                    }
            }
        }

        /// <summary>
        /// Funkcja przesyłająca stan
        /// </summary>
        /// <param name="ToAllId">Czy wysłać sync do wszystkich</param>
        private static void Sync(bool ToAllId)
        {
            var Text = ((int)Commands.TypDanychBiurka.Sync).ToString() + ";";
            Text += r.ToString() + ";";
            Text += g.ToString() + ";";
            Text += b.ToString() + ";";
            Text += alpha.ToString();

            for (byte i = 1; i < Czesci.Count + 1; i++)
            {
                Text += ";";
                Text += Czesci[(Commands.CzęściBiurka)i].SPower;
            }

            if (ToAllId)
            {
                BiurkoWrite?.Invoke(Text, -1);
            }
            else
            {
                BiurkoWrite?.Invoke(Text, LastIdRequest);
            }
        }
    }
}