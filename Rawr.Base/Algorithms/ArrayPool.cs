using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rawr.Base.Algorithms
{
    public static class ArrayPool<TArraySet> where TArraySet : new()
    {
        private static List<TArraySet> pool = new List<TArraySet>();

        private static int createdArraySets = 0;
        private static int maximumPoolSize = 2;

        public static int MaximumPoolSize
        {
            get
            {
                lock (pool)
                {
                    return maximumPoolSize;
                }
            }
            set
            {
                lock (pool)
                {
                    maximumPoolSize = value;
                }
            }
        }

        public static TArraySet RequestArraySet()
        {
            lock (pool)
            {
                if (pool.Count == 0 && createdArraySets < maximumPoolSize)
                {
                    pool.Add(new TArraySet());
                    createdArraySets++;
                }
                else
                {
                    while (pool.Count == 0) Monitor.Wait(pool);
                }
                int bestIndex = pool.Count - 1;
                TArraySet result = pool[bestIndex];
                pool.RemoveAt(bestIndex);
                return result;
            }
        }

        public static void ReleaseArraySet(TArraySet arraySet)
        {
            lock (pool)
            {
                if (pool.Count >= maximumPoolSize)
                {
                    createdArraySets--;
                }
                else
                {
                    pool.Add(arraySet);
                    Monitor.Pulse(pool);
                }
            }
        }
    }
}
