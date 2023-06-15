using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Ramsey
{
    

    public class Degeneracy
    {
        static void colourGraph(Graph g, string ib)
        {
            int N = g.N;
            for (int j = 0; j < N; j++)
            {
                for (int k = j + 1; k < N; k++)
                {
                    g.colours[j, k] = (Colour)(ib[j / 2 * (2 * N - j - 1) + k - j] - '0');
                    g.colours[k, j] = (Colour)(ib[j / 2 * (2 * N - j - 1) + k - j] - '0');
                }
            }
            g.CalculateE0();
        }
        static public void AllStates(int N, int[] x)
        {
            Histogram<double> hist = new Histogram<double>();
            Histogram<double> hist2 = new Histogram<double>();
            Histogram<double> hist3 = new Histogram<double>();
            Graph g = new Graph(N, x);
            g.K0 = x.ToList().Select(xi => (double)1).ToArray();
            g.K1 = x.ToList().Select(x2 => (double)1).ToArray();
            int nEdges = (int)Math.Pow(2, PermsAndCombs.nCr(N, 2));
            for (int i = 0; i < nEdges; i++)
            {
                string ib = Convert.ToString(i + nEdges, 2); //i in binary
                colourGraph(g, ib);
                hist.IncrementCount(Math.Round(g.E0, 1));
                hist2.IncrementCount(Math.Round(g.E1, 2));
                if (g.E0 == 0) hist3.IncrementCount(Math.Round(g.E1, 2));
                if (i % 10_000 == 0) { Console.Clear(); Console.WriteLine(i); }
            }

            Console.WriteLine("E0");
            hist.Print();

            Console.WriteLine("\nE1");
            hist2.Print();

            Console.WriteLine("\nE1 where E0 = 0");
            int count = hist3.AsEnumerable().Count();
            Console.WriteLine("number of bins: {0}", count);
            Console.WriteLine("percentage: {0}%", Math.Round(100*(double)count / nEdges, 1));
            hist3.Print();
        }

        static public void GroundStates(int N, int[] x)
        {
            Histogram<double> hist = new Histogram<double>();
            
            int cycles = 100_000;

            for (int c = 0; c < cycles; c++)
            {
                Graph g = new Graph(N, x);
                Simulation sim = new Simulation(g);
                sim.Optimize(false, 100);
                if (g.E1 == 0) hist.IncrementCount(Math.Round(g.E1, 4 ));
                if (c % 1000 == 0) { Console.Clear(); Console.Write(c); }
            }

            Console.Clear();
            Console.WriteLine("E1 where E0 = 0");
            int count = hist.AsEnumerable().Count();
            Console.WriteLine("number of bins: {0}", count);
            hist.Print();
        }
    }
}
