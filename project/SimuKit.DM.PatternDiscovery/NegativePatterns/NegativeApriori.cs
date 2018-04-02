using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimuKit.DM.PatternDiscovery.NegativePatterns
{
    public class NegativeApriori<T>
        where T : IComparable<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <param name="domain"></param>
        /// <param name="epsilon">e.g., epsilon = 0.01</param>
        /// <returns></returns>
        public ItemSets<T> FindNegativePatterns(IEnumerable<Transaction<T>> database, IList<T> domain, double epsilon)
        {
            ItemSet<T>[] itemsets = new ItemSet<T>[domain.Count];
            for (int i = 0; i < domain.Count; ++i)
            {
                itemsets[i]=new ItemSet<T>() { domain[i]};
            }
            int dbSize = 0;
            foreach (Transaction<T> transaction in database)
            {
                for(int i=0; i < domain.Count; ++i)
                {
                    if (transaction.ContainsItem(domain[i]))
                    {
                        itemsets[i].TransactionCount++;
                    }
                }
                dbSize++;
            }

            for (int i = 0; i < domain.Count; ++i)
            {
                itemsets[i].DbSize = dbSize;
            }

            List<ItemSet<T>> patterns = new List<ItemSet<T>>();
            for (int i = 0; i < domain.Count; ++i)
            {
                for (int j = 0; j < domain.Count; ++j)
                {
                    if (i == j) continue;

                    if (itemsets[i][0].CompareTo(itemsets[j][0]) < 0)
                    {
                        patterns.Add(new ItemSet<T>() { itemsets[i][0], itemsets[j][0] });
                    }
                }
            }

            foreach (Transaction<T> transaction in database)
            {
                for (int i = 0; i < patterns.Count; ++i)
                {
                    if (transaction.ContainsItemSet(patterns[i]))
                    {
                        patterns[i].TransactionCount++;
                    }
                }
            }

            for (int i = 0; i < patterns.Count; ++i)
            {
                patterns[i].DbSize = dbSize;
            }

            ItemSets<T> nis = new ItemSets<T>();
            for (int i = 0; i < patterns.Count; ++i)
            {
                T itemA = patterns[i][0];
                T itemB = patterns[i][1];
                int indexA = domain.IndexOf(itemA);
                int indexB = domain.IndexOf(itemB);
                ItemSet<T> A = itemsets[indexA];
                ItemSet<T> B = itemsets[indexB];
                double score = Evaluation.Kulczynski(A, B, patterns[i]);
                if (score < epsilon)
                {
                    nis.Add(patterns[i]);
                }
            }

            return nis;
        }
    }
}
