using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ramsey
{
    internal class Gibbs
    {
        int N;
        int[] x;
        int nEdges;
        Random rand = new Random();

        public Gibbs(int N, int[] x, bool sample = false, int r_sample = 10)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            this.N = N;
            this.x = x;
            nEdges = PermsAndCombs.nCr(N, 2);
            FreeEnergyPlot(sample, r_sample);

            watch.Stop();
            Console.WriteLine($"Time: {(double)watch.ElapsedMilliseconds / 1000} seconds");

        }
        public List<double> GibbsPartition(List<double> betas, int greens)
        {
            
            List<double> Qs = new List<double>(new double[betas.Count()]);
            if (greens == 0)
            {
                double E0 = (new Graph(N, x)).E0;
                return betas.ConvertAll(beta => Math.Exp(-beta * E0));
            }
               
            IEnumerable<List<int>> edges = Combs.GetCombs(Enumerable.Range(0, N), 2).ToList();
            IEnumerable < List < List<int> >> colourings = Combs.GetCombs(edges, greens);
            foreach (List<List<int>> colouring in colourings)
            {
                Graph g = new Graph(N, x, false);
                foreach (List<int> edge in colouring)
                {
                    g.FlipEdge(edge[0], edge[1]);
                }
                g.CalculateE0();
                for (int i = 0; i < betas.Count(); i++)
                    Qs[i] += Math.Exp(-betas[i] * g.E0);
            }

            

            return Qs;

            

        }

        public List<double> GibbsPartitionSample(List<double> betas, int greens, int r_sample)
        {
            List<double> Qs = new List<double>(new double[betas.Count()]);
            if (greens == 0)
            {
                double E0 = (new Graph(N, x)).E0;
                return betas.ConvertAll(beta => Math.Exp(-beta * E0));
            }

            
            int n_edges = PermsAndCombs.nCr(N, 2);
            int n_colourings = PermsAndCombs.nCr(n_edges, greens);
            List<double> E0_sample = new List<double>();

            int n_sample = n_colourings / r_sample;
            if (n_sample < 100) n_sample = n_colourings;
            for (int i = 0; i < n_sample; i++)
            {
                Graph g = Graph.RandomGraph(N, x, greens);
                g.CalculateE0();
                E0_sample.Add(g.E0);
            }
            for (int i = 0; i < betas.Count(); i++)
                Qs[i] += n_colourings * Math.Exp(-betas[i] * E0_sample.Average());
            return Qs;

        }

        //public List<double> GibbsPartitionIsomorphism(List<double> betas, int greens, int r_sample)
        //{
            
        //}
        public List<double> FreeEnergy(List<double> betas, int greens, bool sample, int r_sample)
        {
            List<double> Qs;
            if (sample) Qs = GibbsPartitionSample(betas, greens, r_sample);
            else Qs = GibbsPartition(betas, greens);
            List<double> fs = new List<double>();
            for (int i = 0; i < betas.Count(); i++)
                fs.Add(-1 / (betas[i] * nEdges) * Math.Log(Qs[i]));
            return fs;
        }

        public static IEnumerable<double> Range(double min, double max, double step)
        {
            double result = min;
            for (int i = 0; result < max; i++)
            {
                result = min + (step * i);
                yield return result;
            }
        }

        public void FreeEnergyPlot(bool sample, int r_sample)
        {
            List<double> betas = Range(9.9, 10, 0.1).ToList();
            double nRho = 10;

            string path = "C:\\Users\\bjorn\\OneDrive - Universiteit Utrecht\\UNI\\Bachelor\\Jaar 4\\Blok 3\\Scriptie\\Resultaten\\Python\\Plots\\";
            string name;
            if (sample) name = $"Gibbs_Sample_{N}_{x[0]}.txt";
            else name = $"Gibbs_{N}_{x[0]}.txt";
            using (StreamWriter writer = new StreamWriter(path+name))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                int greens = -1;
                for (double rho = 0; rho <= 0.5; rho += 0.5/nRho)
                {
                    if (greens == (int)Math.Round(rho * nEdges)) continue;
                    greens = (int)Math.Round(rho * nEdges);
                    List<double> fs = FreeEnergy(betas, greens, sample, r_sample);
                    for (int i = 0; i < betas.Count(); i++)
                    {
                        writer.WriteLine("{0}; {1}; {2}", betas[i], (double)greens / nEdges, fs[i]);
                        writer.WriteLine("{0}; {1}; {2}", betas[i], 1 - (double)greens / nEdges, fs[i]);
                    }
                    Console.WriteLine(greens);
                }
            }
        }
    }
}
