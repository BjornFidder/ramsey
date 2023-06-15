using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace Ramsey
{
    public class Histogram<TVal> : SortedDictionary<TVal, int>
    {
        public void IncrementCount(TVal binToIncrement)
        {
            if (ContainsKey(binToIncrement))
            {
                this[binToIncrement]++;
            }
            else
            {
                Add(binToIncrement, 1);
            }
        }

        public void Print()
        {
            int count = this.AsEnumerable().Count();
            Console.WriteLine("number of bins: {0}", count);
            foreach (KeyValuePair<TVal, int> histEntry in this.AsEnumerable())
            {
                Console.WriteLine("{0} occurred {1} times", histEntry.Key, histEntry.Value);
            }
        }

        public void Save(string name)
        {
            string path = "C:\\Users\\bjorn\\OneDrive - Universiteit Utrecht\\UNI\\Bachelor\\Jaar 4\\Blok 3\\Scriptie\\Resultaten\\Python\\Histograms\\";
            using (StreamWriter writer = new StreamWriter(path + name))
            {
                foreach (KeyValuePair<TVal, int> histEntry in this.AsEnumerable())
                {
                    writer.WriteLine("{0}; {1}", histEntry.Key, histEntry.Value);
                }
            }
        }
        
        public Dictionary<TVal, int> ToDictionary()
        {
            return this.AsEnumerable().ToDictionary(p => p.Key, p => p.Value);
        }


    }

    public static class ShowHistogram
    {

        public static double Variance(List<double> values)
        {
            double avg = values.Average();
            return values.Average(x => Math.Pow(x - avg, 2));
        }
        public static Histogram<int> FromDictionary(Dictionary<int, int> dict)
        {
            Histogram<int> hist = new Histogram<int>();
            foreach (int key in dict.Keys)
                for (int i = 0; i < dict[key]; i++)
                    hist.IncrementCount(key);
            return hist;
        }
        public static void PrintStats(Histogram<int> hist)
        {
            List<int> values = hist.Keys.Aggregate(new List<int>(), (vals, key) => vals.Concat(Enumerable.Repeat(key, hist[key])).ToList());

            double avg = values.Average();
            Console.WriteLine($"Average: {avg}");
            Console.WriteLine($"Variance: {Variance(values.ConvertAll(x => (double)x))}");
        }
        public static void Show(Histogram<int> hist)
        {
            Bitmap bm = ToBitmap(hist);
            Application.Run(new BitmapForm(bm));
        }

        public static Bitmap ToBitmap(Histogram<int> histogram, int xLabelOffset = 0, int yLabelOffset = 0)
        {
            var marginTop = 20;
            var marginBottom = 20;
            var marginLeft = 50;
            var marginAxis = 10;
            var width = 200;
            var height = 150;

            var yRound = 10;

            var max = histogram.Max(kvp => kvp.Key);
            var maxValue = (int)Math.Round((double)(histogram.Max(kvp => kvp.Value) / yRound + 1)) * yRound;
            float heightScale = maxValue / (float)height;
            var bitmap = new Bitmap(width + marginLeft + marginAxis + 50, height + marginBottom + marginTop);
            var gfx = Graphics.FromImage(bitmap);
            gfx.Clear(Color.White);
            var blue = new Pen(Color.Blue, 2 * width / histogram.Keys.Count / 3);
            foreach (var kvp in histogram)
            {
                var x = kvp.Key * width / max + marginLeft + marginAxis;
                gfx.DrawLine(blue, new PointF(x, (height - kvp.Value / heightScale) + marginTop), new PointF(x, height + marginTop));
            }

            var font = new Font("Arial", 7);
            StringFormat strF = new StringFormat();
            strF.Alignment = StringAlignment.Far;

            for (var i = (int)((maxValue - yLabelOffset) / heightScale); i >= 0; i -= 20)
            {
                gfx.DrawString((Math.Round(i*heightScale)).ToString(), font, Brushes.Black, marginLeft - 5, height - i + marginTop, strF);
            }

            foreach (int key in histogram.Keys)
            {
                var g = Graphics.FromImage(bitmap);
                g.TranslateTransform(key * width / max - 6 + marginLeft + marginAxis, height + 5 + marginTop);
                g.RotateTransform(270);
                g.DrawString(key.ToString(), font, Brushes.Black, 0, 0, strF);
            }
            return bitmap;
        }
    }
}
