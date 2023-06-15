using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ramsey
{
    internal static class GraphEnumeration
    {

        public static int[,] NumberOfGraphs = new int[,]
        {
            {0, 1, 1, 1,   1, 1, 1,   1, 1, 1 },
            {0, 0, 1, 1,   1, 1, 1,   1, 1, 1 },
            {0, 0, 0, 1,   2, 2, 2,   2, 2, 2 },
            {0, 0, 0, 1,   3, 4, 5,   5, 5, 5 },
            {0, 0, 0, 0,   2, 6, 9,   10,11,11 },
            {0, 0, 0, 0,   1, 6, 15,  21,24,25 },
            {0, 0, 0, 0,   1, 6, 21,  41,56,63 },
            {0, 0, 0, 0,   0, 4, 24,  65,115,148 },
            {0, 0, 0, 0,   0, 2, 24,  97,221,345 },
            {0, 0, 0, 0,   0, 1, 21,  131,402,771 },
            {0, 0, 0, 0,   0, 1, 15,  148,663,1637 },
            {0, 0, 0, 0,   0, 0, 9,   148,980,3252 },
            {0, 0, 0, 0,   0, 0, 5,   131,1312,5995 },
            {0, 0, 0, 0,   0, 0, 2,   97,1557,21933},
            {0, 0, 0, 0,   0, 0, 1,   65,1646,15615},
            {0, 0, 0, 0,   0, 0, 1,   41,1557,21933},
            {0, 0, 0, 0,   0, 0, 0,   21,1312,27987},
            {0, 0, 0, 0,   0, 0, 0,   10,980,32403},
            {0, 0, 0, 0,   0, 0, 0,   5, 663,34040}
        };

        public static List<double> EnumerateGraphs(int N, int[] x, int greens)
        {
            int n = NumberOfGraphs[greens, N];
            Console.WriteLine($"n = {n}");

            List<double> energies = new List<double>();
            List<double> IDs = new List<double>();

            Graph g; double ID;

            int count = 0;
            int i = 0;
            while (count < n)
            {
                g = Graph.RandomGraph(N, x, greens);
                ID = g.ID();
                if (IDs.Any(id => Math.Round(ID, 12) == Math.Round(id, 12))) continue;
                g.CalculateE0();
                energies.Add(g.E0); count++;
                IDs.Add(ID);

                if (count % 100 == 0 || (double)count / n > 0.9) Console.WriteLine(count);
                i++;

                
            }

            Console.WriteLine($"Found all {n} graphs of size {N} with {greens} green edges.");

            return energies;
        }
    }
}
