using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternDiscovery.FrequentPatterns
{
    public class FPGrowth<T>
        where T : IComparable<T>
    {
        public delegate double GetMinSupportHandle(ItemSet<T> itemset);

        public ItemSets<T> MinePatterns(IEnumerable<Transaction<T>> database, IList<T> domain, double minSupport)
        {
            return MinePatterns(database, domain, (itemset) => { return minSupport; });
        }

        public ItemSets<T> MinePatterns(IEnumerable<Transaction<T>> database, IList<T> domain, GetMinSupportHandle getMinItemSetSupport)
        {
            Dictionary<T, int> counts = new Dictionary<T, int>();
            for (int i = 0; i < domain.Count; ++i)
            {
                counts[domain[i]] = 0;
            }

            foreach (Transaction<T> transaction in database)
            {
                for (int i = 0; i < domain.Count; ++i)
                {
                    if (transaction.ContainsItem(domain[i]))
                    {
                        counts[domain[i]]++;
                    }
                }
            }

            List<T> freqItems = new List<T>();
            for (int i = 0; i < domain.Count; ++i)
            {
                if(counts[domain[i]] >= getMinItemSetSupport(new ItemSet<T>() { domain[i]}))
                {
                    freqItems.Add(domain[i]);
                }
            }

            T[] FList = freqItems.ToArray();
            Array.Sort(FList, (i1, i2) =>
                {
                    int comp = counts[i1].CompareTo(counts[i2]);
                    return -comp;
                });

            FPTree<T> fpTree = new FPTree<T>();

            int dbSize = 0;
            foreach (Transaction<T> transaction in database)
            {
                dbSize++;

                List<T> orderedFreqItems = new List<T>();
                for (int i = 0; i < FList.Length; ++i)
                {
                    if (transaction.ContainsItem(FList[i]))
                    {
                        orderedFreqItems.Add(FList[i]);
                    }
                }

                fpTree.AddOrderedFreqItems(orderedFreqItems);
            }

            fpTree.DbSize = dbSize;

            ItemSets<T> allItemSets = new ItemSets<T>();
            for (int i = FList.Length - 1; i >= 0; i--)
            {
                T item = FList[i];
                List<ItemSet<T>> fis = fpTree.MinePatternsContaining(item, getMinItemSetSupport(new ItemSet<T>() { item }));
                fpTree.RemoveFromLeaves(item);
                allItemSets.AddRange(fis);
            }

            return allItemSets;
        }

        public ItemSets<T> FindMaxPatterns(IEnumerable<Transaction<T>> database, IList<T> domain, double minSupport)
        {
            return FindMaxPatterns(MinePatterns(database, domain, minSupport));
        }

        protected ItemSets<T> FindMaxPatterns(ItemSets<T> fis)
        {
            for (int i = fis.Count - 1; i >= 0; i--)
            {
                ItemSet<T> itemset = fis[i];
                bool isSubSet = false;
                for (int j = 0; j < fis.Count; j++)
                {
                    if (i == j) continue;
                    isSubSet = true;

                    ItemSet<T> itemset2 = fis[j];
                    if (itemset.Count > itemset2.Count)
                    {
                        isSubSet = false;
                        break;
                    }
                    
                    for (int k = 0; k < itemset.Count; ++k)
                    {
                        if (!itemset2.Contains(itemset[k]))
                        {
                            isSubSet = false;
                            break;
                        }
                    }

                    if (!isSubSet) break;
                }

                if (isSubSet)
                {
                    fis.RemoveAt(i);
                }
            }
            return fis;
        }

        
    }
}
