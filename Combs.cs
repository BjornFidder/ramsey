using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ramsey
{
    // Source : https://stackoverflow.com/questions/33336540/how-to-use-linq-to-find-all-combinations-of-n-items-from-a-set-of-numbers

    public static class PermsAndCombs
    {
        public static int nCr(int n, int r)
        {
            // naive: return Factorial(n) / (Factorial(r) * Factorial(n - r));
            return nPr(n, r) / Factorial(r);
        }

        public static int nPr(int n, int r)
        {
            // naive: return Factorial(n) / Factorial(n - r);
            return FactorialDivision(n, n - r);
        }

        private static int FactorialDivision(int topFactorial, int divisorFactorial)
        {
            int result = 1;
            for (int i = topFactorial; i > divisorFactorial; i--)
                result *= i;
            return result;
        }

        private static int Factorial(int i)
        {
            if (i <= 1)
                return 1;
            return i * Factorial(i - 1);
        }
    }

    static class Combs
    {
         
        private static void InitIndexes(int[] indexes)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i] = i;
            }
        }

        private static void SetIndexes(int[] indexes, int lastIndex, int count)
        {
            indexes[lastIndex]++;
            if (lastIndex > 0 && indexes[lastIndex] == count)
            {
                SetIndexes(indexes, lastIndex - 1, count - 1);
                indexes[lastIndex] = indexes[lastIndex - 1] + 1;
            }
        }

        private static List<T> TakeAt<T>(int[] indexes, IEnumerable<T> list)
        {
            List<T> selected = new List<T>();
            for (int i = 0; i < indexes.Length; i++)
            {
                selected.Add(list.ElementAt(indexes[i]));
            }
            return selected;
        }

        private static bool AllPlacesChecked(int[] indexes, int places)
        {
            for (int i = indexes.Length - 1; i >= 0; i--)
            {
                if (indexes[i] != places)
                    return false;
                places--;
            }
            return true;
        }

        public static IEnumerable<List<T>> GetCombs<T>(this IEnumerable<T> collection, int count)
        {
            int[] indexes = new int[count];
            int listCount = collection.Count();
            if (count > listCount)
                throw new InvalidOperationException($"{nameof(count)} is greater than the collection elements.");
            InitIndexes(indexes);
            do
            {
                var selected = TakeAt(indexes, collection);
                yield return selected;
                SetIndexes(indexes, indexes.Length - 1, listCount);
            }
            while (!AllPlacesChecked(indexes, listCount));

        }
    }
}
