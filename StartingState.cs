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
    internal class StartingState
    {
        public static void StartingStateRounds(int N, int[] x, bool random, int n, int maxRounds, double T0)
        {
            List<int> roundsValues = new List<int>();
            for (int i = 0; i < n; i++)
            {
                Graph g = new Graph(N, x);
                if (random) g.Randomize();
                Simulation sim = new Simulation(g);
                int rounds = sim.Optimize(false, maxRounds, T0);
                roundsValues.Add(rounds);
                Console.WriteLine($"{i}: {rounds}");
            }

            string path = "C:\\Users\\bjorn\\OneDrive - Universiteit Utrecht\\UNI\\Jaar 4\\Blok 3\\Scriptie\\Resultaten\\Python\\Histograms\\";
            string name;
            if (random) { name = $"starting_random_{N}.txt"; }
            else name = $"starting_monochromatic_{N}.txt";
            using (StreamWriter writer = new StreamWriter(path + name))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                foreach (int rounds in roundsValues)
                    writer.WriteLine(rounds);
            }

        }
    }
}
