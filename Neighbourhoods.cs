using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Ramsey
{
    internal class Neighbourhoods
    {
        static public void GroundStateDeltas(int N, int[] x, int n, int dE)
        {
            int binsize = 10;

            Dictionary<int, List<int>> counts = new Dictionary<int, List<int>>();
            Dictionary<int, int> countsUnbinned = new Dictionary<int, int>();
            Dictionary<int, double> variances = new Dictionary<int, double>();
            Dictionary<int, double> means = new Dictionary<int, double>();

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(i);

                Histogram<int> hist = new Histogram<int>();
                
                Graph g = new Graph(N, x);
                g.Randomize();
                Simulation sim = new Simulation(g);
                int rounds = sim.Optimize(false, 100);
                
                if (g.E0 != 0) continue;

                double dE0;
                int count = 0;
                for (int j = 0; j < N; j++)
                    for (int k = j + 1; k < N; k++)
                    {
                        (dE0, _, _) = g.FlipImpact(j, k);
                        if (dE0 == dE) count++;
                        hist.IncrementCount((int)Math.Round(dE0));
                    }

                List<int> values = hist.Keys.Aggregate(new List<int>(), (vals, key) => vals.Concat(Enumerable.Repeat(key, hist[key])).ToList());
                double mean = values.Average();
                double variance = values.Average(v => Math.Pow(v - mean, 2));

                g.CalculateE1();

                int E1 = (int)Math.Round(g.E1);
                int E1_binned = (int)Math.Round(g.E1/binsize) * binsize;

                if (!countsUnbinned.Keys.Contains(E1))
                    countsUnbinned.Add(E1, count);

                if (!counts.Keys.Contains(E1_binned))
                    counts.Add(E1_binned, new List<int> { count });
                else
                    counts[E1_binned].Add(count);

                if (!means.Keys.Contains(E1))
                    means.Add(E1, mean);

                if (!variances.Keys.Contains(E1))
                    variances.Add(E1, variance);
            }

            Dictionary<int, int> countAverages = counts.ToDictionary(p => p.Key, p => (int)Math.Round(p.Value.Average()));
            Dictionary<int, int> averages = means.ToDictionary(p => p.Key, p => (int)Math.Round(p.Value * 1000));
            Dictionary<int, int> vars = variances.ToDictionary(p => p.Key, p => (int)Math.Round(p.Value * 1000));
            
            Console.WriteLine($"Flips with dE0 = {dE} from ground state as a function of E1");
            int bins = countAverages.AsEnumerable().Count();
            Console.WriteLine("number of bins: {0}", bins);

            Histogram<int> histCounts = ShowHistogram.FromDictionary(countAverages);
            Histogram<int> histCountsUnbinned = ShowHistogram.FromDictionary(countsUnbinned);
            Histogram<int> histAverages = ShowHistogram.FromDictionary(averages);
            Histogram<int> histVariances = ShowHistogram.FromDictionary(vars);
            //ShowHistogram.PrintStats(hist);
            //histAverages.Print();
            //histVariances.Print();
            //ShowHistogram.Show(histAverages);
            //ShowHistogram.Show(histVariances);
            histCounts.Save($"E1_dE0={dE}_{N}.txt");
            histCountsUnbinned.Save($"E1_dE0={dE}_{N}_unbinned.txt");
            histAverages.Save($"E1_dE0mu_{N}.txt");
            histVariances.Save($"E1_dE0var_{N}.txt");
            Console.WriteLine();
        }

        static public void StableFlips(int[] x, int n)
        {
            Dictionary<int, List<int>> N_stableFlips = new Dictionary<int, List<int>>();
            for (int N = 25; N < 36; N++)
            {
                Console.Write($"N = {N}: ");
                List<int> stableFlips = new List<int>(); 
                for (int i = 0; i < n; i++)
                {
                    Graph g = new Graph(N, x);
                    g.Randomize();
                    Simulation sim = new Simulation(g);
                    sim.Optimize(false, 100);

                    double dE0;
                    int count = 0;
                    for (int j = 0; j < N; j++)
                        for (int k = j + 1; k < N; k++)
                        {
                            (dE0, _, _) = g.FlipImpact(j, k);
                            if (dE0 == 0) count++;
                        }
                    Console.Write($"{count}, ");
                    stableFlips.Add(count);
                }
                Console.WriteLine();
                N_stableFlips.Add(N, stableFlips);
            }

            string path = "C:\\Users\\bjorn\\OneDrive - Universiteit Utrecht\\UNI\\Bachelor\\Jaar 4\\Blok 3\\Scriptie\\Resultaten\\Python\\Histograms\\";
            string name = "stable_flips.txt";
            using (StreamWriter writer = new StreamWriter(path + name))
            {
                foreach (var kvp in N_stableFlips)
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
        static public void GroundStateFlips(int N, int[] x, int n, Graph graph = null)
        {
            Histogram<int> hist;
            Graph g;
            for (int i = 0; i < n; i++)
            {
                hist = new Histogram<int>();

                if (graph == null)
                {
                    g = new Graph(N, x);
                    Simulation sim = new Simulation(g);
                    int rounds = sim.Optimize(false, 10000);
                    if (g.E1 != 0) continue;
                }
                else g = graph;

                double dE0; double dE1;
                for (int j = 0; j < N; j++)
                    for (int k = j + 1; k < N; k++)
                    {
                        (dE0, dE1, _) = g.FlipImpact(j, k);
                        //if (dE0 == 0) Console.WriteLine($"Stabilizer {j}, {k}");
                        hist.IncrementCount((int)Math.Round(dE0));
                        //Console.Clear();
                        //Console.WriteLine($"j={j}, k={k}");
                    }

                Console.WriteLine($"dE0 after flipping one edge, E0 = {(int)Math.Round(g.E0, 1)}");
                //Console.WriteLine($"number of rounds: {rounds}");

                g.CalculateE1();
                Console.WriteLine($"E1: {g.E1}");
                ShowHistogram.PrintStats(hist);
                hist.Print();
                //hist.Save($"Neighbourhoods_{N}.txt");
                //ShowHistogram.Show(hist);

                Console.WriteLine();
            }

        }
    }
}
