using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;
using BendrasModelis;

namespace Valdiklis
{
    class Program
    {
        static Dictionary<string, Dictionary<string, int>> BendrasZodziuZemelapis = new();

        static void Main()
        {
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x4;

            Console.WriteLine(" Valdiklis pasiruošęs. Laukia agentų...");

            Thread gija1 = new Thread(() => GautiZodziusIs("kanalasA"));
            Thread gija2 = new Thread(() => GautiZodziusIs("kanalasB"));

            gija1.Start();
            gija2.Start();

            gija1.Join();
            gija2.Join();

            Console.WriteLine("\n Galutinis žodžių sąrašas:");
            foreach (var failas in BendrasZodziuZemelapis)
                foreach (var zodis in failas.Value)
                    Console.WriteLine($"{failas.Key}:{zodis.Key}:{zodis.Value}");
        }

        static void GautiZodziusIs(string vamzdzioPav)
        {
            using var serveris = new NamedPipeServerStream(vamzdzioPav, PipeDirection.In);
            serveris.WaitForConnection();
            Console.WriteLine($" Prisijungė: {vamzdzioPav}");

            using var skaitytojas = new StreamReader(serveris);
            while (!skaitytojas.EndOfStream)
            {
                var eilute = skaitytojas.ReadLine();
                if (string.IsNullOrWhiteSpace(eilute)) continue;

                var objektas = FailoZodis.IsEilutes(eilute);
                lock (BendrasZodziuZemelapis)
                {
                    if (!BendrasZodziuZemelapis.ContainsKey(objektas.FailoPavadinimas))
                        BendrasZodziuZemelapis[objektas.FailoPavadinimas] = new();

                    if (!BendrasZodziuZemelapis[objektas.FailoPavadinimas].ContainsKey(objektas.Zodis))
                        BendrasZodziuZemelapis[objektas.FailoPavadinimas][objektas.Zodis] = 0;

                    BendrasZodziuZemelapis[objektas.FailoPavadinimas][objektas.Zodis] += objektas.Kiekis;
                }
            }
        }
    }
}
