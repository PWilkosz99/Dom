using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace SerwerRoot.Podzespoły
{
    static class Log
    {
        private static StreamWriter sw;

        /// <summary>
        /// Blokada przed czynnościami na stream ie podczas pracy w funkcji BeginAsync
        /// </summary>
        private static ManualResetEvent BeginWorker = new ManualResetEvent(false);

        /// <summary>
        /// Otwórz plik
        /// </summary>
        public static async void BeginAsync()
        {
            BeginWorker.Reset();
            StorageFolder DebugFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Debug logs", CreationCollisionOption.OpenIfExists);
            StorageFolder YearDebugFolder = await DebugFolder.CreateFolderAsync(DateTime.Now.Year.ToString(), CreationCollisionOption.OpenIfExists);
            StorageFolder MonthDebugFolder = await YearDebugFolder.CreateFolderAsync(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month), CreationCollisionOption.OpenIfExists);
            StorageFile LogFile =  await MonthDebugFolder.CreateFileAsync(String.Format("{0:d.MM.yyy HH.mm.ss}.txt", DateTime.Now), CreationCollisionOption.OpenIfExists);
            sw = new StreamWriter((await LogFile.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.AllowReadersAndWriters)).AsStream())
            {
                AutoFlush = true,
            };
           // BeginWorker.Set();
        }

        /// <summary>
        /// Wypisz log do pliku
        /// </summary>
        /// <param name="text">Tekst</param>
        /// <param name="PrintToConsole">Czy napisac do konsoli</param>
        public static async void Write(string text, bool PrintToConsole)
        {
            DateTime date = DateTime.Now;
           // BeginWorker.WaitOne();    
            try
            {
                sw.WriteLine(date + ": " +  text);               
            }
            catch (Exception e)
            {
                Debug.WriteLine("Błąd w zapisie do log-u, lub brak wywołania funkcji Begin() " + e.Message);
               // BeginAsync();
               // Write("Błąd w zapisie do log-u, lub brak wywołania funkcji Begin() " + e.Message);
            }
#if DEBUG
            if(PrintToConsole)
            {
                Debug.WriteLine(text);
                return;
            }
#else
            return;
#endif
        }

        /// <summary>
        /// Wypisz log do pliku
        /// </summary>
        /// <param name="text">Tekst</param>
        public static async void Write(string text) => Write(text, false);



        /// <summary>
        /// Wypisz wyjątek
        /// </summary>
        /// <param name="e">Exception</param>
        public static void Write(Exception e)
        {
            Write(e.Message);
        }
    }
}