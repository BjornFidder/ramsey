using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Ramsey
{
    class Program
    {
        static void Main(string[] args)
        {
            int N = 37;
            int[] x = new int[] {5, 5};

            Console.WriteLine($"Considering R({x[0]}, {x[1]}) with N = {N}\n");

            //Simulation.Simulate42(x, 40, 0.5);
            //string graph6 = ">>graph6<<izGOSHBG[`hBhBs`\\GRhAmcH\\G\\\\GYmcMjhBt\\G^Ts`}jhA]jhAnTs`jt\\G\\]jhBTymcMjt\\G]jt\\GVTimcLt]jhBmjt\\GQynTs`dt]jhBdt]jhBQynTs`cmjt\\GWdt]jhAAVTymcICmjt\\GO";
            //string graph6 = ">>graph6<<ikp@?cG`aEcKcLQEs`McGyOds`Ds`iyOqmcLTs`lVQEymcLymcG|VQCnTs`dymcLViyOqnTs`inTs`lViyOfTimcKynTs`ji|VQCVTymcKVTymcLJi|VQEQynTs`aVTymcKH\\ViyO_QynTs`GQynTs`?";
            //string graph6 = ">>graph6<<i?UhCDACX_e]KbK~J]uaJwHUfxIXZl\\OT\\QPziBUwewZiUF~BCg~?qQa]pUPFqaYE]lelCyFvpNTWZ{UTB~qLFk`RebpEC~DcTL_Kb{Pdbh~Bw_VsuCU[Mj]]KGfn[Xod\\whG]xczXXC^pkzG";
            //Graph g = Graph6.FromGraph6(graph6, x);

            //Console.WriteLine(g.E0);

            //Console.WriteLine($"Variance: {g.DegVar()}");
            //Neighbourhoods.GroundStateFlips(N, x, 1, g);m 
            //g.FlipEdge(31, 32);
            //g.CalculateEnergies();
            //Console.WriteLine($"E0= {g.E0}");
            //Console.WriteLine($"Variance: {g.DegVar()}");

            Simulation.RunSimulation(N, x, null, 1, true, true, false, 0, 0, 100);

            //StartingState.StartingStateRounds(N, x, false, 50, 50, 0.7);
            //Simulation.GroundStateE2(N, x, 50);

            //Degeneracy.GroundStates(N, x);
            //Degeneracy.AllStates(N, x);
            //Neighbourhoods.GroundStateFlips(N, x, 1, null);
            //Neighbourhoods.GroundStateDeltas(N, x, 200, 0);
            //Neighbourhoods.StableFlips(x, 20);
            //Energy1.E1plot(N, x, 2500, true);
            //Energy1.N_E1(x, 20);

            //new Gibbs(N, x);

            //Network.GroundStateConnections(N, x, 1, g);
            //Network.GroundStateVariance(N, x, 500);
            //Network.KVariance(N, x, 20, 0);

            //GraphEnumeration.EnumerateGraphs(N, x, 18);

            //Console.ReadLine();
        }
    }
}
