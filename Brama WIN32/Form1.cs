using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Net.Sockets;
using System.Net;
using MetroFramework;

namespace aplikacja_pc_do_esp
{
    public partial class Form1 : MetroForm
    {

        bool isconn;
        Socket klient;

        String sip = "192.168.1.4";

        int sport = 80;

        public Form1()
        {
            InitializeComponent();
        }
        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (!isconn)
            {
                    Thread conn =  new Thread(() =>
                    {
                        try
                        {
                            klient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                            klient.Connect(sip, sport);
                            isconn = true;
                        }
                        catch { MessageBox.Show("lol"); isconn = false; }

                    });
                    conn.Start();
            }
            try
            {
                klient.Send(ASCIIEncoding.ASCII.GetBytes(metroTextBox1.Text));
            }
            catch { isconn = false; }
            
        }

        private void ip_Click(object sender, EventArgs e)
        {
            ip.Text = "";
        }

        private void port_Click(object sender, EventArgs e)
        {
            port.Text = "";
        }

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            if (metroToggle1.Checked)
            {
                try
                {
                    sip = ip.Text;
                }
                catch
                {
                    metroToggle1.Checked = false;
                    MetroMessageBox.Show(this, "Niepoprawne dane w polu: Ip adress\n Exception: " + e.ToString());
                }
                try
                {
                    sport = Convert.ToInt32(port.Text);
                }
                catch
                {
                    metroToggle1.Checked = false;
                    MetroMessageBox.Show(this, "Niepoprawne dane w polu: Port\n Exception: " + e.ToString());
                }
            }
            else
            {
                sip = "192.168.1.4";
                sport = 80;
            }
             
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (klient != null)
            {
                klient.Close();
            }
        }
    }
}
