using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;
using BendrasModelis;

namespace Valdiklis
{
    class Program
    {
        static void Main()
        {
            // Priskiriame Master procesui konkretų CPU branduolį
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x4;

            // Informuojame, kad Master laukia prisijungimų
            Console.WriteLine(" Valdiklis pasiruošes. Laukia agentu...");

            // Paleidžiame pirmą giją agentui A
            Thread gija1 = new Thread(() => GautiZodziusIs("kanalasA"));

            // Paleidžiame antrą giją agentui B
            Thread gija2 = new Thread(() => GautiZodziusIs("kanalasB"));

            gija1.Start();
            gija2.Start();

            gija1.Join();
            gija2.Join();
        }

        static void GautiZodziusIs(string vamzdzioPav)
        {
            using var serveris = new NamedPipeServerStream(vamzdzioPav);
            serveris.WaitForConnection();

            using var skaitytojas = new StreamReader(serveris);
            var eilute = skaitytojas.ReadToEnd();

            var zodziai = FailoZodis.IsEilutes(eilute);
            foreach (var z in zodziai)
                Console.WriteLine(z);
        }
    }
}
