using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using BendrasModelis;

namespace ScannerA
{
    class Program
    {
        // Pagrindinis ScannerA metodas
        static void Main(string[] args)
        {
            // Priskiriame šiam agentui konkretų CPU branduolį
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x1;

            // Nustatome katalogą, jei nepateiktas – naudojame programos aplanką
            string katalogas = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($" Skaitymo aplankas: {katalogas}");

            // Naudojame vamzdžio pavadinimą "kanalasA"
            string vamzdzioPavadinimas = "kanalasA";

            // Paleidžiame žodžių analizę ir siuntimą atskiroje gijoje
            Thread analizavimoGija = new Thread(() => ApdorokIrPerduok(katalogas, vamzdzioPavadinimas));
            analizavimoGija.Start();
        }

        static void ApdorokIrPerduok(string kelias, string vamzdis)
        {
            var rezultatai = new List<FailoZodis>();

            foreach (var failas in Directory.GetFiles(kelias, "*.txt"))
            {
                var tekstas = File.ReadAllText(failas);
                var zodziai = tekstas.Split(' ', '\n', '\r', '.', ',', ';', ':', '-', '!', '?');

                var grupes = zodziai
                    .Where(z => !string.IsNullOrWhiteSpace(z))
                    .GroupBy(z => z)
                    .ToDictionary(gr => gr.Key, gr => gr.Count());

                foreach (var item in grupes)
                {
                    rezultatai.Add(new FailoZodis
                    {
                        Zodis = item.Key,
                        Kiekis = item.Value,
                        FailoPavadinimas = Path.GetFileName(failas)
                    });
                }
            }

            using var kanalas = new NamedPipeClientStream(".", vamzdis, PipeDirection.Out);
            kanalas.Connect();

            using var rasytojas = new StreamWriter(kanalas) { AutoFlush = true };
            rasytojas.Write(FailoZodis.ToString(rezultatai));
        }
    }
}
