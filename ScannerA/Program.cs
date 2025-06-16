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
        static void Main(string[] args)
        {
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x1; // CPU branduolys

            string kelias = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($" Skaitymo aplankas: {kelias}");

            string vamzdzioPavadinimas = "kanalasA";
            Thread darba = new Thread(() => ApdorokIrPerduok(kelias, vamzdzioPavadinimas));
            darba.Start();
        }

        static void ApdorokIrPerduok(string kelias, string vamzdis)
        {
            List<FailoZodis> rezultatai = new();

            foreach (var failas in Directory.GetFiles(kelias, "*.txt"))
            {
                string turinys = File.ReadAllText(failas);
                var zodziai = turinys.Split(new[] { ' ', '\n', '\r', '.', ',', '!', '?', '-', ';' }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, int> zodziuSk = new();
                foreach (var z in zodziai)
                {
                    string apdorotas = z.ToLower();
                    if (!zodziuSk.ContainsKey(apdorotas))
                        zodziuSk[apdorotas] = 0;
                    zodziuSk[apdorotas]++;
                }

                foreach (var entry in zodziuSk)
                {
                    rezultatai.Add(new FailoZodis
                    {
                        FailoPavadinimas = Path.GetFileName(failas),
                        Zodis = entry.Key,
                        Kiekis = entry.Value
                    });
                }
            }

            using var kanalas = new NamedPipeClientStream(".", vamzdis, PipeDirection.Out);
            kanalas.Connect();

            using var rasytojas = new StreamWriter(kanalas) { AutoFlush = true };
            foreach (var zodis in rezultatai)
                rasytojas.WriteLine(zodis.ToString());

            Console.WriteLine(" ScannerA baigė siųsti duomenis.");
        }
    }
}
