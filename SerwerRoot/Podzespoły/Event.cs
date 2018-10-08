
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Data.Sqlite;


namespace SerwerRoot.Podzespoły
{
    public static class Event
    {
        /// <summary>
        /// Baza
        /// </summary>
        static SqliteConnection db = new SqliteConnection("Filename=Rejestr.db");

        /// <summary>
        /// Łącznie z bazą
        /// </summary>
        static public void Begin()
        {
            try
            {
                db.Open();

            }
            catch (SqliteException e)
            {
                Log.Write("Bład bazy banych, kod: "+ e.SqliteErrorCode + "; " + e.Message);
                Debug.WriteLine("Bład bazy banych, kod: " + e.SqliteErrorCode + "; " + e.Message);
            }

            String tableCommand = @"CREATE TABLE IF NOT EXISTS Dom (
                                    Id INTEGER NOT NULL
                                    PRIMARY KEY AUTOINCREMENT,
                                    Pochodzenie TEXT    NOT NULL,
                                    Data        DATE NOT NULL,
                                    Podmiot TEXT,
                                    Zdarzenie   TEXT NOT NULL,
                                    Opis TEXT    NOT NULL)";

            SqliteCommand createTable = new SqliteCommand(tableCommand, db);

            createTable.ExecuteReader();
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
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO Dom(Pochodzenie, Data, Podmiot, Zdarzenie, Opis) VALUES ('" + Pochodzenie + "', '" + DateTime.Now.ToString() + "', '" + Podmiot + "', '" + Zdarzenie + "', '" + Opis + "')";
                
                insertCommand.ExecuteReader();
                db.Close();
            }
            catch (SqliteException e)
            {
                Log.Write("Bład bazy banych, kod: " + e.SqliteErrorCode + "; " + e.Message);
                Debug.WriteLine("Bład bazy banych, kod: " + e.SqliteErrorCode + "; " + e.Message);
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
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO Dom(Pochodzenie, Data, Zdarzenie, Opis) VALUES ('" + Pochodzenie + "', '" + DateTime.Now.ToString() + "', '" + Zdarzenie + "', '" + Opis + "')";

                insertCommand.ExecuteReader();
                db.Close();
            }
            catch (SqliteException e)
            {
                Log.Write("Bład bazy banych, kod: " + e.SqliteErrorCode + "; " + e.Message);
                Debug.WriteLine("Bład bazy banych, kod: " + e.SqliteErrorCode + "; " + e.Message);
            }
        }
                
        public static List<String> GetData()
        {
            List<String> entries = new List<string>();

            using (SqliteConnection db =
                new SqliteConnection("Filename=Rejestr.db"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT ID, Zdarzenie from Dom", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0) + ": " + query.GetString(1));
                }

                db.Close();
            }

            return entries;
        }
                              
    }
}
