using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Dom_android
{
    class Test
    {
        public static byte[] buffer = new byte[20];

        static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);      

        public static void Connect()
        {
            client.BeginConnect(new IPEndPoint(IPAddress.Parse("192.168.1.4"), 80), new AsyncCallback(ConnectCallback), client);


            //Thread th = 
            //new Thread(() =>
            //{  

            //    if(!client.Connected)
            //    {
            //        try
            //        {
            //            Console.WriteLine("--------------------------------------------------\nConnection try");
            //            // Change IPAddress.Loopback to a remote IP to connect to a remote host.
            //            client.Connect(IPAddress.Parse("192.168.1.4"), 80);
            //            Console.WriteLine("Connected to: " + client.RemoteEndPoint.ToString());
            //            Recive();
            //        }
            //        catch (SocketException e)
            //        {
            //            Console.WriteLine(e.Message);
            //        }
            //    }

            //}).Start();



            //th.Start();

            //Console.WriteLine("--------------------------------------------------------------------------------------------------------------");

            //client.BeginConnect( new IPEndPoint(IPAddress.Parse("192.168.1.4"), 80), new AsyncCallback(ConnectCallback), client);

            //try
            //{

            //    client.Connect(IPAddress.Parse("192.168.1.4"), 80);

            //   // await client.ConnectAsync(new IPEndPoint(IPAddress.Parse("192.168.1.4"), 80));
            //}
            //catch
            //{
            //    Console.WriteLine("----------Błąd");
            //}
            //Console.Write("ok");

            //SocketAsyncEventArgs data = new SocketAsyncEventArgs();

            //data.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.4"), 80);
            //data.Completed += new EventHandler<SocketAsyncEventArgs>(Lol);
            //Console.WriteLine("connecting..");

            //await client.ConnectAsync(data);

        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                Recive();

                // Signal that the connection has been made.
            }
            catch (Exception e)
            {
                client.BeginConnect(new IPEndPoint(IPAddress.Parse("192.168.1.4"), 80), new AsyncCallback(ConnectCallback), client);
                Console.WriteLine(e.ToString());
            }
        }

        public static void Disconnect()
        {
            new Thread(() =>
            {
                try
                {
                    client.Disconnect(false);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                }               

            }).Start();
        }

        public static void Recive()
        {
            client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {

            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.


                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);
                Console.WriteLine("---------------------------------------------");

                

                Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, bytesRead));

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    // Get the rest of the data.
                    client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        
    }
}