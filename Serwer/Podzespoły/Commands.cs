using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serwer.Podzespoły
{
    class Commands
    {
        /// <summary>
        /// W Tym Typie Wyliczeniowym zawaarte są Moduły
        /// </summary>
        public enum Moduł
        {
            Brama = 2,
            Biurko = 3,
        };

        public enum Podmiot
        {
            Niezidentyfikowany = 1,
            Telefon = 2,
            PC = 3,
            Przycisk = 4,
            Pilot = 5,
        }

        // -------------------BRAMA-------------------
        /// <summary>
        /// Polecenia do sterowania Bramą
        /// </summary>
        public enum AkcjaBramy
        {
            Otwórz = 2,
            Stop = 3,
            Zamknij = 4,
            Stan = 5,
        };        

        // ------------------BIURKO------------------

        public enum CzęściBiurka
        {
            Klawiatura = 1,
            GornaPolka = 2,
            DolnaPolka = 3,
            GornaSzafka = 4,
            DolnaSzafka = 5,
            Tyl = 6,
            Dodatkowy = 7,
        };

        public enum TypDanychBiurka
        {
            Jasność = 1,
            Kolor = 2,
            Część = 3,
            Sync = 4,
        }
    }
}
