using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ramsey
{
    internal static class Energy1
    {

        public static void E1plot(int N, int[] x, int n, bool optimize)
        {

            Graph g;
            List<double> E1values = new List<double>();
            for (int i = 0; i < n; i++)
            {
                g = new Graph(N, x);
                g.Randomize();
                if (optimize)
                {
                    Simulation sim = new Simulation(g);
                    sim.Optimize(false, 30);
                    if (g.E0 != 0) continue;
                }
                g.CalculateE1();
                E1values.Add(Math.Round(g.E1, 10));
                Console.WriteLine(g.E1);
            }

            string path = "C:\\Users\\bjorn\\OneDrive - Universiteit Utrecht\\UNI\\Bachelor\\Jaar 4\\Blok 3\\Scriptie\\Resultaten\\Python\\Histograms\\";
            string name;
            if (optimize) { name = $"E1_{N}.txt"; }
            else name = $"E1_{N}_random.txt";
            using (StreamWriter writer = new StreamWriter(path + name))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                foreach (double E1 in E1values)
                    writer.WriteLine(E1);
            }

            Console.WriteLine($"Distinct: {E1values.Distinct().Count()}");
        }

        static public void N_E1(int[] x, int n)
        {
            Dictionary<int, List<int>> N_E1 = new Dictionary<int, List<int>>();
            for (int N = 25; N < 36; N++)
            {
                Console.Write($"N = {N}: ");
                List<int> E1s = new List<int>();
                for (int i = 0; i < n; i++)
                {
                    Graph g = new Graph(N, x);
                    g.Randomize();
                    Simulation sim = new Simulation(g);
                    sim.Optimize(false, 100);

                    if (g.E0 != 0) { i--; continue; }
                    g.CalculateE1();

                    Console.Write($"{g.E1}, ");
                    E1s.Add((int)Math.Round(g.E1));
                }
                Console.WriteLine();
                N_E1.Add(N, E1s);
            }

            string path = "C:\\Users\\bjorn\\OneDrive - Universiteit Utrecht\\UNI\\Bachelor\\Jaar 4\\Blok 3\\Scriptie\\Resultaten\\Python\\Histograms\\";
            string name = "N_E1.txt";
            using (StreamWriter writer = new StreamWriter(path + name))
            {
                foreach (var kvp in N_E1)
                {
                    writer.Write(kvp.Key);
                    foreach (int count in kvp.Value)
                    {
                        writer.Write("; ");
                        writer.Write(count);
                    }
                    writer.Write("\n");
                }
            }
        }
    }
}
