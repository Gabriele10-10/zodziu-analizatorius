using System;

namespace BendrasModelis // Galima naudoti bendrą namespace, jei nori bendros klasės
{
    [Serializable]
    public class FailoZodis
    {
        public string FailoPavadinimas { get; set; }
        public string Zodis { get; set; }
        public int Kiekis { get; set; }

        public override string ToString()
        {
            return $"{FailoPavadinimas}|{Zodis}|{Kiekis}";
        }

        public static FailoZodis IsEilutes(string eilute)
        {
            var dalys = eilute.Split('|');
            return new FailoZodis
            {
                FailoPavadinimas = dalys[0],
                Zodis = dalys[1],
                Kiekis = int.Parse(dalys[2])
            };
        }
    }
}
