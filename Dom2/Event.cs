
//using SQLitePCL;

using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;


namespace Dom
{
    class Event
    {
        static SQLiteConnection baza;

       

        static public void Connect()
        {
            try
            {

                var sqlpath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,"Rejestr.db");

                //var sqlpath = @"D:\Dom\Rejestr.db";
                Debug.WriteLine(sqlpath);
                
                baza = new SQLiteConnection(sqlpath);
                
               
            }
            catch 
            {
               
            }

        }

        static public void Write(string Pochodzenie, string Podmiot, string Zdarzenie, string Opis)
        {
            try
            {
                var command = baza.Prepare("INSERT INTO Dom(Pochodzenie, Data, Podmiot,Zdarzenie, Opis)  VALUES(?, ?, ?, ?, ?)");
                command.Bind(1, Pochodzenie);
                command.Bind(2, DateTime.Now.ToString());
                command.Bind(3, Podmiot);
                command.Bind(4, Zdarzenie);
                command.Bind(5, Opis);

                command.Step();
            }
            catch 
            {
               
            }                  

        }

        static public void Write(string Pochodzenie, string Zdarzenie, string Opis)
        {
            try
            {
                var command = baza.Prepare("INSERT INTO Dom(Pochodzenie, Data, Zdarzenie, Opis)  VALUES(?, ?, ?, ?)");
                command.Bind(1, Pochodzenie);
                command.Bind(2, DateTime.Now.ToString());
                command.Bind(3, Zdarzenie);
                command.Bind(4, Opis);

                command.Step();
            }
            catch 
            {
           
            }

        }
    }
}
