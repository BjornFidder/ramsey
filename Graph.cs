using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Numerics;
using System.Xml.Schema;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.Providers.LinearAlgebra;

namespace Ramsey
{
    public enum Colour
    {
        Red,
        Green,
        Blue
    }
    public class Graph
    {
        public int N; // size of the graph
        public int[] X; // ramsey parameters
        public double[] K0; // parameter weights
        public double[] K1; // parameter weights
        public Colour[,] colours; // adjacency matrix with colours
        public double E0; // energy
        public double E1;
        public int[] degrees;
        public int greens;
        readonly Random rand = new Random();
        bool Calc_E1;

        public Graph(int n, int[] x, bool calc_E1 = false)
        {
            N = n;
            X = x;
            Calc_E1 = calc_E1;
            if (x.Min() == x.Max())
                K0 = x.ToList().Select(xi => 1.0).ToArray();
            else
                K0 = x.ToList().Select(xi => 1 / (double)xi).ToArray();
            K1 = K0;
            //Math.Exp(-Math.Pow(PermsAndCombs.nCr(xi, 2), 2)/e2)

            colours = new Colour[N, N];
            degrees = new int[N];
            CalculateE0();
            if (Calc_E1) CalculateE1();
        }

        public static Graph RandomGraph(int N, int[] x, int greens)
        {
            Random rand = new Random();
            Graph g = new Graph(N, x, false);
            List<List<int>> edges = Combs.GetCombs(Enumerable.Range(0, N), 2).ToList();
            List<int> edge;
            for (int j = 0; j < greens; j++)
            {
                edge = edges[rand.Next(edges.Count())];
                edges.Remove(edge);
                g.FlipEdge(edge[0], edge[1]);
            }
            return g;
        }

        public void Randomize()
        {
            for (int j = 0; j < N; j++)
                for (int k = j + 1; k < N; k++)
                    if (rand.Next(2) == 1) { FlipEdge(j, k); }
            CalculateE0();
            if (Calc_E1) CalculateE1();
        }

        public void FlipEdge(int v1, int v2)
        {
            colours[v1, v2] ^= Colour.Green;
            colours[v2, v1] = colours[v1, v2]; //also flip reverse edge
            if (colours[v1, v2] == Colour.Green) { degrees[v1]++; degrees[v2]++; greens++; }
            else { degrees[v1]--; degrees[v2]--; greens--; }
        }

        public (double, double, double) FlipImpact(int v1, int v2)
        {
            double dE0 = 0; double dE1 = 0; double Var = 0;
            List<int> clique;
            List<int> verticesRest = Enumerable.Range(0, N).Where(v => v != v1 && v != v2).ToList();
            for (int i = 0; i < X.Length; i++)
            {
                if (colours[v1, v2] == (Colour)i)
                    dE0 -= K0[i] * E0_edge(new List<int> { v1, v2 }, i);
                else
                    dE0 += K0[i] * E0_edge(new List<int> { v1, v2 }, i);

                if (Calc_E1)
                {
                    foreach (List<int> rest in Combs.GetCombs<int>(verticesRest, X[i] - 2))
                    {
                        clique = new List<int> { v1, v2 };
                        clique.AddRange(rest);

                        //dE0 -= K0[i] * this.E0_clique(clique, (Colour)i);
                        dE1 -= K1[i] * this.E1_clique(clique, (Colour)i);
                        FlipEdge(v1, v2);
                        //dE0 += K0[i] * this.E0_clique(clique, (Colour)i);
                        dE1 += K1[i] * this.E1_clique(clique, (Colour)i);
                        FlipEdge(v1, v2);
                    }
                }
            }
            FlipEdge(v1, v2);
            Var = DegVar();
            FlipEdge(v1, v2);

            return (dE0, dE1, Var);
        }

        public double E0_edge(List<int> startClique, int i)
        {
            
            List<List<int>> cliques = new List<List<int>> { startClique };
            for (int j = startClique.Count; j < X[i]; j++)
            {
                List<List<int>> cliquesNext = new List<List<int>>();
                foreach (List<int> clique in cliques)
                {
                    List<int> verticesRest = Enumerable.Range(0, N).Where(v => !clique.Contains(v) && (j == startClique.Count || v > clique.Last())).ToList();
                    foreach (int v in verticesRest)
                    {
                        bool monochromatic = clique.All(v0 => colours[v0, v] == (Colour)i);
                        if (monochromatic) cliquesNext.Add(clique.Concat(new[] { v }).ToList());
                    }
                }
                cliques = cliquesNext;
                if (cliques.Count == 0) break;
            }
            return cliques.Count;
        }
        public double E0_clique(List<int> clique, Colour colour)
        {
            foreach (List<int> edge in Combs.GetCombs<int>(clique, 2))
            {
                if (colours[edge[0], edge[1]] != colour) return 0;
            }
            return 1;
        }

        //public double EpsilonGauss(List<int> clique, Colour colour)
        //{
        //    int count = 0;
        //    foreach (List<int> edge in Combs.GetCombs<int>(clique, 2))
        //    {
        //        if (colours[edge[0], edge[1]] == colour) count++;
        //    }
        //    return Math.Exp(-count * count / e2);
        //}

        public double E1_clique(List<int> clique, Colour colour)
        {
            int count = 0;
            foreach (List<int> edge in Combs.GetCombs<int>(clique, 2))
            {
                if (colours[edge[0], edge[1]] != colour) count++;
            }
            if (count == 1) return 1;
            else return 0;
        }
        public double DegVar()
        {
            return ShowHistogram.Variance(degrees.ToList().ConvertAll(x => (double)x));
        }

        public double EigVar()
        {
            int[,] coloursInt = new int[N, N];
            double[,] coloursD = new double[N, N];
            Array.Copy(colours, coloursInt, colours.Length);
            Array.Copy(coloursInt, coloursD, colours.Length);
            Matrix<double> A = DenseMatrix.OfArray(coloursD);

            Evd<double> eig = A.Evd();
            Vector<Complex> val = eig.EigenValues;
            Matrix<double> vec = eig.EigenVectors;

            vec *= N - 1;

            return ShowHistogram.Variance(vec.Column(N-1).ToList());
        }

        public double rho()
        {
            int greens = 0;
            for (int j = 0; j < N; j++)
                for (int k = j + 1; k < N; k++)
                    if (colours[j, k] == Colour.Green) greens++;
            return (double)greens / PermsAndCombs.nCr(N, 2);

        }
        public void CalculateE0()
        {
            E0 = 0; 
            E0 += E0_edge(new List<int>(), 0);
            E0 += E0_edge(new List<int>(), 1);
        }

        public void CalculateE1()
        {
            E1 = 0;
            for (int i = 0; i < X.Length; i++)
                foreach (List<int> clique in Combs.GetCombs<int>(Enumerable.Range(0, N), X[i]))
                {
                    E1 += K1[i] * E1_clique(clique, (Colour)i);
                }
        }

        public double ID()
        {
            return DegVar() + EigVar() + CorrelationCoefficient();
        }
        public double Overlap(Graph g, bool print)
        {
            if (N != g.N) { throw new ArgumentException("Attempted calculating overlap of graphs of nonequal size"); }
            int overlap = 0;
            for (int j = 0; j < N; j++)
                for (int k = j + 1; k < N; k++)
                {
                    if (colours[j, k] == g.colours[j, k]) overlap++;
                    else overlap--;
                }
            double q = (double)overlap / PermsAndCombs.nCr(N, 2);
            if (print)
            {
                Console.WriteLine($"Overlap with ground state: {Math.Round(q, 4)}");
                Console.WriteLine($"{(PermsAndCombs.nCr(N, 2) - overlap) / 2} of {PermsAndCombs.nCr(N, 2)} different\n");
            }
            return q;
        }

        public double CorrelationCoefficient()
        {
            int Se = 0;
            
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if (colours[i, j] == Colour.Green) Se += degrees[i] * degrees[j];
            int S1 = 0; int S2 = 0; int S3 = 0; int k;
            for (int i = 0; i < N; i++)
            {
                k = degrees[i];
                S1 += k;
                S2 += k * k;
                S3 += k * k * k;
            }
            return (double)(S1 * Se - S2 * S2) / (S1 * S3 - S2 * S2);
        }
    }

    public static class Graph6
    {
        public static Graph FromGraph6(string bytesIn, int[] x)
        {
            if (bytesIn.StartsWith(">>graph6<<")) bytesIn = bytesIn.Substring(10);
            string data = new string(bytesIn.ToList().ConvertAll(c => (char)(c - 63)).ToArray());
            if (data.Any(c => c > 63)) throw new
                ArgumentException("Each input character must be in the range 63 - 127");
            int N;
            (N, data) = dataToN(data);
            int Nd = (N * (N - 1) / 2 + 5) / 6;
            if (data.Length != Nd) throw new
                ArgumentException($"Expected {N * (N - 1) / 2} bits, but got {data.Length * 6} in graph6");

            List<Colour> colourList = getColours(data);
            Graph g = new Graph(N, x);
            int i = 0;
            for (int j = 0; j < N; j++)
                for (int k = 0; k < j; k++)
                { if (colourList[i] == Colour.Green) g.FlipEdge(j, k);
                  i++; }

            g.CalculateE0();
            return g;
        }

        static List<Colour> getColours(string data)
        {
            List<Colour> colourList = new List<Colour>();
            foreach (char c in data)
                for (int i = 5; i >= 0; i--)
                    colourList.Add((Colour)(c >> i & 1));
            return colourList;
        }

        static (int, string) dataToN(string data)
        {
            if (data[0] <= 62) return (data[0], data.Substring(1));
            if (data[1] <= 62) return ((data[1] << 12) + (data[2] << 6) + data[3], data.Substring(4));
            else return ((data[2] << 30) + (data[3] << 24) + (data[4] << 18) +
                         (data[5] << 12) + (data[6] << 6) + data[7], data.Substring(8));
        }

        public static string ToGraph6(Graph g)
        {
            int row_elems = 0;
            string bin_list = "";
            for (int i = 0; i < g.N; i++)
            {
                row_elems++;
                if (row_elems == 1) continue;
                for (int j = 0; j < row_elems-1; j++)
                    bin_list += ((int)g.colours[i, j]).ToString();
            }

            if ((bin_list.Length) % 6 != 0)
                    bin_list += new string('0', 6 - bin_list.Length % 6);

            List<string> chunks = new List<string>();
            for (int i = 0; i < bin_list.Length  / 6; i++)
            {
                chunks.Add("");
                for (int j = 0; j < 6; j++)
                    chunks[i] += bin_list[6 * i + j];
            }

            string graph6 = "";
            graph6 += Convert.ToChar(g.N + 63);

            foreach (string chunk in chunks)
            {
                graph6 += Convert.ToChar(Convert.ToInt32(chunk, 2) + 63);
            }

            return graph6;

        }
    }
}


