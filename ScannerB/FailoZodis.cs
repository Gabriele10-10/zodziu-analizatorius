using System;
using System.Collections.Generic;
using System.Linq;

namespace BendrasModelis
{
    // Klasė aprašo vieno žodžio įrašą: žodį, kiekį ir failo pavadinimą
    public class FailoZodis
    {
        // Žodžio dažnis (kiek kartų pasikartojo)
        public int Kiekis { get; set; }

        // Pats žodis
        public string Zodis { get; set; }

        // Iš kurio failo šis žodis rastas
        public string FailoPavadinimas { get; set; }

        // Konvertuoja sąrašą žodžių į vieną teksto eilutę 
        public static string ToString(List<FailoZodis> zodziai)
        {
            // Naudojame '|' kaip atskyrimo ženklą, eilutės atskiriamos '\n'
            return string.Join("\n", zodziai.Select(z => $"{z.FailoPavadinimas}|{z.Zodis}|{z.Kiekis}"));
        }

        // Sukuria žodžių sąrašą iš teksto eilutės 
        public static List<FailoZodis> IsEilutes(string eilute)
        {
            var zodziai = new List<FailoZodis>();

            foreach (var eil in eilute.Split('\n'))
            {
                var dalys = eil.Split('|');
                if (dalys.Length != 3) continue;

                zodziai.Add(new FailoZodis
                {
                    FailoPavadinimas = dalys[0],
                    Zodis = dalys[1],
                    Kiekis = int.TryParse(dalys[2], out int k) ? k : 0
                });
            }

            return zodziai;
        }

        // Gražina suprantamą žodžio aprašymą
        public override string ToString()
        {
            return $"Failas: {FailoPavadinimas}, Žodis: {Zodis}, Kiekis: {Kiekis}";
        }
    }
}
