using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Net.Sockets;
using MetroFramework;




namespace aplikacja_pc_do_esp
{
    public partial class Main : MetroForm
    {
        Socket klient = null;
        Thread conn;        
        bool isconnect = false;
        Thread recv;
        public Main()
        {
            InitializeComponent();
            if (!isconnect)
            {
                conn = new Thread(() =>
                {
                    while (!isconnect)
                    {
                        try
                        {
                            Thread.Sleep(100);
                            Invoke((MethodInvoker)delegate { infobox.Text += "Łączenie..." + Environment.NewLine; });
                            klient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                            klient.Connect("192.168.1.4", 80);
                            isconnect = klient.Connected;
                            Invoke((MethodInvoker)delegate { infobox.Text = "Nawiązano połączenie"; });
                            break;
                        }
                        catch (Exception e)
                        {
                            Invoke((MethodInvoker)delegate { infobox.Text += "Błąd połączenia z bramą" + Environment.NewLine + e.Source + Environment.NewLine + e.Message; });
                            isconnect = false;
                        }
                        Thread.Sleep(900);
                    }

                });
                conn.Start();
               // conn.Join();
            }

            recv = new Thread(() =>
            {
                for (;;)
                {
                    if (klient != null)
                    {
                        byte[] tmp = new byte[256];
                        if (klient.Available != 0)
                        {
                            klient.Receive(tmp);
                            String stmp = Encoding.ASCII.GetString(tmp);
                            Invoke((MethodInvoker) delegate
                            {
                                if (String.Compare(stmp, "opning") == 0)
                                {
                                    kolko.Speed = 1.5F;
                                    info.Text = "Otwieranie bramy...";
                                    
                                }
                                else if (String.Compare(stmp, "cloing") == 0)
                                {
                                    kolko.Speed = 1.5F;
                                    info.Text = "Zamykanie bramy...";

                                }
                                else if (String.Compare(stmp, "opened") == 0)
                                {
                                    kolko.Speed = 0.01F;
                                    info.Text = "Brama jest otwarta";

                                }
                                else if (String.Compare(stmp, "closed") == 0)
                                {
                                    kolko.Speed = 0.01F;
                                    info.Text = "Brama jest zamknięta";
                                    
                                }
                                else if (String.Compare(stmp, "brokee") == 0)
                                {
                                    kolko.Speed = 0.01F;
                                    info.Text = "Atywny czujnik przerwana wiązki IR";

                                }
                                else if (String.Compare(stmp, "none..") == 0)
                                {
                                    kolko.Speed = 0.01F;
                                    info.Text = "Brama nie jest ani zamknięta, ani otwarta";
                                }
                            });


                        }
                    }
                    Thread.Sleep(1);

                }

            });
            recv.Start();


        }

        private void metroTile1_Click(object sender, EventArgs e)
        {

            Form1 testwindow = new Form1();
            testwindow.Show();
        }

        private void otworz_Click(object sender, EventArgs e)
        {
            send("‘");
        }

        private void stop_Click(object sender, EventArgs e)
        {
            send("s");
        }

        private void zamknij_Click(object sender, EventArgs e)
        {
            send("z");
        }




        private void send(String code)
        {
            try
            {
                if (!isconnect)
                {
                    conn = new Thread(() =>
                    {
                        while (!isconnect)
                        {
                            try
                            {
                                Invoke((MethodInvoker)delegate { infobox.Text += "Łączenie..." + Environment.NewLine; });
                                klient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                                klient.Connect("192.168.1.4", 80);
                                isconnect = klient.Connected;
                                Invoke((MethodInvoker)delegate { infobox.Text = "Nawiązano połączenie"; });
                            }
                            catch (Exception ex)
                            {
                                Invoke((MethodInvoker)delegate { infobox.Text += "Błąd połączenia z bramą, " + Environment.NewLine + ex.Source + Environment.NewLine + ex.Message + Environment.NewLine; });
                                isconnect = false;
                            }
                        }
                           

                    });
                    conn.Start();
                }
                klient.Send(ASCIIEncoding.ASCII.GetBytes(code));
            }
            catch (Exception e)
            {
                infobox.Text += "Błąd połączenia z bramą" + Environment.NewLine + e.Source + Environment.NewLine + e.Message + Environment.NewLine;
                isconnect = false;
            }

        }
         


        private void time_Tick(object sender, EventArgs e)
       { 
            if (klient != null)
            {
                new Thread(() =>
               {
                   try
                   {
                       byte[] tmp = new byte[1];
                       tmp[0] = 0;
                       klient.Send(tmp, 0);
                   }
                   catch (Exception ex)
                   {
                       Invoke((MethodInvoker)delegate { infobox.Text += "Błąd połączenia z bramą, " + Environment.NewLine + ex.Source + Environment.NewLine + ex.Message + Environment.NewLine; });
                       isconnect = false;
                   }

               }).Start();
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (klient != null)
            {
                recv.Abort();
                klient.Close();
            }
        }

        private void more_Click(object sender, EventArgs e)
        {
            if (!infobox.Visible)
            {
                Size = new Size(400, 450);
                more.Text = "Pokaż mniej";
                kolko.Location = new Point(45, 375);
                more.Location = new Point(204, 385);
                test.Location = new Point(305, 385);
                infobox.Visible = true;
                
            }
            else
            {
                Size = new Size(400, 285);
                more.Text = "Pokaż więcej";
                kolko.Location = new Point(45, 220);
                more.Location = new Point(204, 220);
                test.Location = new Point(305, 220);
                infobox.Visible = false;
            }
        }


    }
}
