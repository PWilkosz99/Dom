
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


namespace SerwerRoot.Podzespoły
{
    class Event
    {
        /// <summary>
        /// Baza
        /// </summary>
        private static SQLiteConnection Baza;

        /// <summary>
        /// Lokalizacja
        /// </summary>
        private static string SqlPath => Path.Combine(ApplicationData.Current.LocalFolder.Path, "Rejestr.db");

        /// <summary>
        /// Łącznie z bazą
        /// </summary>
        static public void Begin()
        {
            try
            {
                Baza = new SQLiteConnection(SqlPath);
            }
            catch (Exception e)
            {
                Log.Write(e.Message);
                Debug.WriteLine(e.Message);
            }

        }

        /// <summary>
        /// Rejestrowanie zdarzenia
        /// </summary>
        /// <param name="Pochodzenie">Jaki Moduł</param>
        /// <param name="Podmiot">Skąd jest, pilot, telefon, komputer</param>
        /// <param name="Zdarzenie">Zdarzenie krótko</param>
        /// <param name="Opis">Opis zdarzenia</param>
        public static void Write(string Pochodzenie, string Podmiot, string Zdarzenie, string Opis)
        {
            try
            {
                ISQLiteStatement command = Baza.Prepare("INSERT INTO Dom(Pochodzenie, Data, Podmiot,Zdarzenie, Opis)  VALUES(?, ?, ?, ?, ?)");
                command.Bind(1, Pochodzenie);
                command.Bind(2, DateTime.Now.ToString());
                command.Bind(3, Podmiot);
                command.Bind(4, Zdarzenie);
                command.Bind(5, Opis);

                command.Step();
            }
            catch (Exception e)
            {
                Log.Write(e.Message);
            }

        }

        /// <summary>
        /// Rejestrowanie zdarzenia
        /// </summary>
        /// <param name="Pochodzenie">Jaki Moduł</param>
        /// <param name="Zdarzenie">Zdarzenie krótko</param>
        /// <param name="Opis">Opis zdarzenia</param>
        public static void Write(string Pochodzenie, string Zdarzenie, string Opis)
        {
            try
            {
                ISQLiteStatement command = Baza.Prepare("INSERT INTO Dom(Pochodzenie, Data, Zdarzenie, Opis)  VALUES(?, ?, ?, ?)");
                command.Bind(1, Pochodzenie);
                command.Bind(2, DateTime.Now.ToString());
                command.Bind(3, Zdarzenie);
                command.Bind(4, Opis);

                command.Step();
            }
            catch (Exception e)
            {
                Log.Write(e.Message);
            }            
        }                       
    }
}
