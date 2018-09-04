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

namespace Serwer
{
     class Biurko
    {
        public  enum EnumBiurko
        {        
            Klawiatura = 1,
            GornaPolka = 2,
            DolnaPolka = 3,
            GornaSzafka = 4,
            DolnaSzafka = 5,
            Tyl = 6,
        };

        private static SerialDevice serialPort;

        private static DataWriter DataWriter;

        private static bool conn = false;

        public  static bool Isconnect
        {
            get { return conn; }
        }

        public  static async Task ConnectAsync()
        {       

            // !!!! Za pierwszym razem użyj .Wait():

            var dis = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector("UART0"));

            try
            {
                serialPort = await SerialDevice.FromIdAsync(dis[0].Id);
                // ...
                // Configure serial settings
                serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                serialPort.BaudRate = 115200;
                serialPort.Parity = SerialParity.None;
                serialPort.StopBits = SerialStopBitCount.One;
                serialPort.DataBits = 8;


                DataWriter = new DataWriter(serialPort.OutputStream);
                conn = true;

            }
            catch
            {
                conn = false;
            }          
            
        }

        private static async void Write(String TxData)
        {
            try
            {
                DataWriter.WriteString(TxData);
                await DataWriter.StoreAsync();
                
            }
            catch
            {
                await ConnectAsync();
                conn = false;                
            }
        }

        public  static bool Set(short R, short G, short B)
        {
            // Paleta RGB   wartości od 0 do 255
            if(R > 255 || G > 255 || B > 255 )
            {
                return false;
            }
            Write("+|" + Code(R) + "|" + Code(G) + "|" + Code(B) + "\n");
            return true;
        }

        public  static bool Set(short alpha)
        {
            // Jasność  od 0 do 100 %

            if(alpha > 100)
            {
                return false;
            }
            Write("-|" + Code(alpha) + "\n");
            return true;
        }
                
        public  static void Set(EnumBiurko part, bool Power)
        {
            //Część biurka

            Write("=|" + Code((short)part) + "|" + Convert.ToInt16(Power) + "\n");
        }

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
    }
}