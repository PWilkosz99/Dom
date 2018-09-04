using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dom.Podzespoły
{
    class Commands
    {
        /// <summary>
        /// Pochodzenie żądań
        /// </summary>
        public enum Podmiot
        {
            Niezidentyfikowany = 1,
            Telefon = 2,
            PC = 3,
            Przycisk = 4,
            Pilot = 5,
        }
        
        /// <summary>
        /// Polecenia do sterowania Bramą
        /// </summary>
        public enum BramaEnum
        {
            Otwórz = 2,
            Stop = 3,
            Zamknij = 4,
        };

        /// <summary>
        /// Identyfikatory Podzespołów
        /// </summary>
        public enum Command
        {
            Brama = 2,
            Biurko = 3,
        };

        /// <summary>
        /// Części Biurka
        /// </summary>
        public enum BiurkoEnum
        {
            Klawiatura = 1,
            GornaPolka = 2,
            DolnaPolka = 3,
            GornaSzafka = 4,
            DolnaSzafka = 5,
            Tyl = 6,
            Dodatkowy = 7,
        };

        /// <summary>
        /// Typy zmiennych do zmiany
        /// </summary>
        public enum BiurkoValueType
        {
            Jasność = 1,
            Kolor = 2,
            Część = 3,
            Sync = 4,
        }
    }
}
