using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serwer.Podzespoły
{
    class Debug_File
    {
        StreamWriter sw;
        public Debug_File()
        {
            sw = new StreamWriter(new FileStream(@"C:\Debug.txt", FileMode.OpenOrCreate));
            WriteAsync("------------------------------------------------------------------------");
        }
        ~Debug_File()
        {
            sw.Dispose();
        }
        public async void WriteAsync(string text)
        {
            await sw.WriteLineAsync(DateTime.Now + ": " + text);
        }
    }
}