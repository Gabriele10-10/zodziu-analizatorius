using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using BendrasModelis; 

namespace ScannerB
{
    class Program
    {
        static void Main(string[] args)
        {
            // Priskiriam programą CPU branduoliui 2
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x2;

            // Nustatom katalogą: arba argumentu, arba automatiškai
            string katalogas = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($" Skaitymo katalogas: {katalogas}");

            // Naudojam kito vamzdžio pavadinimą
            string vamzdzioPavadinimas = "kanalasB";

            // Paleidžiam analizės ir siuntimo funkciją atskiroje gijoje
            Thread analizavimoGija = new Thread(() => ApdorokIrPerduok(katalogas, vamzdzioPavadinimas));
            analizavimoGija.Start();
        }

        // Metodas, kuris skaito failus, analizuoja ir siunčia duomenis į Master
        static void ApdorokIrPerduok(string kelias, string vamzdis)
        {
            List<FailoZodis> rezultatai = new();

            foreach (var failas in Directory.GetFiles(kelias, "*.txt"))
            {
                string turinys = File.ReadAllText(failas);
                var zodziai = turinys.Split(new[] { ' ', '\n', '\r', '.', ',', '!', '?', '-', ';' }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, int> zodziuSkaicius = new();
                foreach (var z in zodziai)
                {
                    string tvarkingasZodis = z.ToLower();
                    if (!zodziuSkaicius.ContainsKey(tvarkingasZodis))
                        zodziuSkaicius[tvarkingasZodis] = 0;
                    zodziuSkaicius[tvarkingasZodis]++;
                }

                foreach (var pora in zodziuSkaicius)
                {
                    rezultatai.Add(new FailoZodis
                    {
                        FailoPavadinimas = Path.GetFileName(failas),
                        Zodis = pora.Key,
                        Kiekis = pora.Value
                    });
                }
            }

            using var kanalas = new NamedPipeClientStream(".", vamzdis, PipeDirection.Out);
            kanalas.Connect();

            using var rasytojas = new StreamWriter(kanalas) { AutoFlush = true };
            foreach (var zodis in rezultatai)
                rasytojas.WriteLine(zodis.ToString());

            Console.WriteLine("ScannerB baigė siųsti duomenis.");
        }
    }
}
