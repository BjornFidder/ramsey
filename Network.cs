using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace Ramsey
{
    internal class Network
    {
        public static void GroundStateConnections(int N, int[] x, int n, Graph graph = null)
        {
            Histogram<int> histTotal = new Histogram<int>();
            Graph g;
            for (int i = 0; i < n; i++)
            {
                Histogram<int> hist = new Histogram<int>();
                if (graph == null)
                {
                    g = new Graph(N, x);
                    g.Randomize();
                    Simulation sim = new Simulation(g);
                    sim.Optimize(true, 10000);
                    if (g.E1 != 0) continue;
                }
                else g = graph;
                
                for (int j = 0; j < N; j++)
                {
                    hist.IncrementCount(g.degrees[j]);
                    histTotal.IncrementCount(g.degrees[j]);
                }

                Console.WriteLine($"Number of connections per person, E1 = {(int)Math.Round(g.E1, 2)}");
                ShowHistogram.PrintStats(hist);
                hist.Print();
                //hist.Save($"Connections_{N}_{i}.txt");
                Console.WriteLine();
            }

            Console.WriteLine($"Average number of connections per person for tried cases");
            ShowHistogram.PrintStats(histTotal);
            histTotal.Print();
            histTotal.Save($"Connections_{N}.txt");
            ShowHistogram.Show(histTotal);

            Console.WriteLine();
        }

        public static void GroundStateVariance(int N, int[] x, int n, bool optimize)
        {
            Graph g;
            List<double> variances = new List<double>();
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
                double var = g.CorrelationCoefficient();
                if (!Double.IsNaN(var)) variances.Add(Math.Round(var, 10));
                Console.WriteLine(var);
            }

            string path = "C:\\Users\\bjorn\\OneDrive - Universiteit Utrecht\\UNI\\Bachelor\\Jaar 4\\Blok 3\\Scriptie\\Resultaten\\Python\\Histograms\\";
            string name;
            if (optimize) { name = $"CC_{N}.txt"; }
            else name = $"CC_{N}_random.txt";
            using (StreamWriter writer = new StreamWriter(path + name))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                foreach (double var in variances)
                    writer.WriteLine(var);
            }

            Console.WriteLine($"Distinct: {variances.Distinct().Count()}");
        }

        public static void KVariance(int N, int[] x, int n, double Kvar)
        {
            List<int> rs = new List<int>();
            for (int i = 0; i < n; i++)
            { 
                int rounds = Simulation.RunSimulation(N, x, null, 0.5, true, false, false, Kvar, 0, 40);
                Console.WriteLine($"\n\n {rounds} \n\n");
                rs.Add(rounds);
            }

            string path = "C:\\Users\\bjorn\\OneDrive - Universiteit Utrecht\\UNI\\Bachelor\\Jaar 4\\Blok 3\\Scriptie\\Resultaten\\Python\\KVariance\\";
            string name = $"Rounds_{Kvar}_{N}.txt";
            using (StreamWriter writer = new StreamWriter(path + name))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                foreach (int rounds in rs)
                    writer.WriteLine(rounds);
            }


        }
    }

    
}
