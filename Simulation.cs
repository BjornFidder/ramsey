using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ramsey
{
    class Simulation
    {
        public Graph graph;
        double T;
        Random rnd = new Random();
        Stopwatch blockWatch = new Stopwatch();
        Stopwatch totalWatch = new Stopwatch();
        List<double> tabu;
        //int tabuSize;

        public Simulation(Graph g)
        {
            graph = g;
        }

        public static int RunSimulation(int N, int[] x, Graph g = null, double T = 1, bool print = true,
            bool reheat = true, bool calcE1 = false,
            double Kvar = 0, double K1 = 0, int max = 100) 
        {
            if (g == null)
            {
                g = new Graph(N, x, calcE1);
                g.Randomize();
                Console.WriteLine(Graph6.ToGraph6(g));
            }
            Simulation sim = new Simulation(g);
            int rounds = sim.Optimize(print, max, T, Kvar, K1, reheat);

            Console.WriteLine(Graph6.ToGraph6(g));
            Console.WriteLine(g.E0);

            return rounds;
        }

        public static void RunSimulations(int N , int[] x, int n, Graph g = null, double T = 1)
        {
            List<int> rounds = new List<int>();
            Parallel.ForEach(Enumerable.Range(0, n), _ => rounds.Add(Simulation.RunSimulation(N, x, null, 0.5, false)));

            Console.WriteLine($"Average: {rounds.Average()}");
        }


        public bool IsZero(double x)
        {
            return Math.Round(x, 5) == 0;
        }
        public void Print(double count, double countUp, int iters, int n, int iTot)
        {
            Console.WriteLine($"n: {n+1}");
            Console.WriteLine($"iterations: {iTot}");
            Console.WriteLine($"T: {Math.Round(T, 7)}");
            Console.WriteLine($"E0: {Math.Round(graph.E0, 3)}");
            Console.WriteLine($"E1: {Math.Round(graph.E1, 3)}");
            Console.WriteLine($"DegVar: {Math.Round(graph.DegVar(), 3)}");
            //Console.WriteLine($"EigVar: {Math.Round(graph.EigVar(), 3)}");
            Console.WriteLine($"{count} flips, {Math.Round(100 * count / iters, 3)}%");
            Console.WriteLine($"{countUp} flips up, {Math.Round(100 * countUp / iters, 3)}%");
            Console.WriteLine($"rho = {graph.rho()}");
            Console.WriteLine("total: {0:hh\\:mm\\:ss}", totalWatch.Elapsed);
            Console.WriteLine();
        }
        public int Optimize(bool print, int max, double T0 = 0.5, double Kvar = 0, double K1 = 0, bool reheat = true)
        {
            T = T0;
            double alpha = 0.95;
            int iters = PermsAndCombs.nCr(graph.N, 2)*20;
            double count;
            double countUp;

            //tabu = new List<double>();
            //tabuSize = 0;

            Stopwatch printWatch = new Stopwatch();
            int n = 0;
            //blockWatch.Start();
            printWatch.Start();
            totalWatch.Start();
            if (print) Console.WriteLine($"Attacking N={graph.N}, with initial E0 = {Math.Round(graph.E0, 2)}, E1 = {Math.Round(graph.E1, 2)}," +
                $" DegVar = {Math.Round(graph.DegVar(), 3)} and EigVar = {Math.Round(graph.EigVar(), 3)}");
            while (!IsZero(graph.E0) && n < max)
            {
                count = 0;
                countUp = 0;
                int i;
                for (i = 0; i < iters; i++)
                {
                    double E0_before = graph.E0;
                    if (flip(K1, Kvar)) count++;
                    if (graph.E0 > E0_before) countUp++;
                    
                    if (IsZero(graph.E0)) break;
                }

                blockWatch.Restart();
                bool secPrint = printWatch.Elapsed.Seconds > 1; // || count > 0;
                if (secPrint) printWatch.Restart();
                if (print && (secPrint || IsZero(graph.E0)))
                {
                    Print(count, countUp, iters, n, n * iters + i);
                    //Console.WriteLine($"{Math.Round((double)blockWatch.Elapsed.Milliseconds * 1000000 / iters)} ms per M iters");
                }

                if (reheat)
                {
                    if (countUp == 0 && !IsZero(graph.E0)) { T *= 1.5 + 0.5 * rnd.NextDouble(); if (print) Console.WriteLine("\nREHEAT"); }
                    else T *= alpha;
                }
                else T *= alpha;
               
                n++;
            }
            //Console.WriteLine($"n = {n}");
            return n;
        }

        private bool flip(double K1 = 0, double Kvar = 0)
        {
            int v1, v2; //vertices of random edge
            v1 = rnd.Next(graph.N);
            v2 = rnd.Next(graph.N);
            if (v1 == v2) return false;
            (double dE0, double dE1, double Var) = graph.FlipImpact(v1, v2);

            double dVar = Var - graph.DegVar();
            double p = Math.Exp(-(dE0 + Kvar * dVar + K1 * dE1) / T);

            
            //double tabuID = Math.Round(Var, 4) + graph.greens;
            //if (tabu.Contains(tabuID)) { int i = tabu.IndexOf(tabuID); return false; }

            if (rnd.NextDouble() < p)
            //if (dE0 < 0)
            { //perform flip
                graph.FlipEdge(v1, v2);
                graph.E0 += dE0;
                graph.E1 += dE1;

                //tabu.Add(tabuID);
                //if (tabu.Count > tabuSize) tabu.RemoveAt(0);
                
                //if (graph.E1 < 50)
                //{
                //    if (v1 > v2) { (v1, v2) = (v2, v1); }
                //    Console.WriteLine($"Flipped ({v1}, {v2})\n");
                //}

                return true;

            }
            return false;
        }


        public static void Simulate42(int[] x, int flips, double T)
        {
            string graph6 = ">>graph6<<izGOSHBG[`hBhBs`\\GRhAmcH\\G\\\\GYmcMjhBt\\G^Ts`}jhA]jhAnTs`jt\\G\\]jhBTymcMjt\\G]jt\\GVTimcLt]jhBmjt\\GQynTs`dt]jhBdt]jhBQynTs`cmjt\\GWdt]jhAAVTymcICmjt\\GO";
            Graph g = Graph6.FromGraph6(graph6, x);
            Graph g2 = Graph6.FromGraph6(graph6, x);
            Console.WriteLine($"Ground state has E0 = {g.E0}");

            Simulation sim = new Simulation(g);
            Console.WriteLine($"Perturbing by {flips} flips");
            sim.T = double.PositiveInfinity;
            for (int i = 0; i<flips; i++)
                sim.flip();           
            g.Overlap(g2, true);

            sim.Optimize(true, 10_000, T);
            g.Overlap(g2, true);
        }
    }
}
