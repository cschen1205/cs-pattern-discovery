using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternDiscovery.FrequentPatterns;

namespace PatternDiscovery.MultiLevelPatterns
{
    public class MultiLevelFPGrowth<T>
        where T : IComparable<T>
    {
        public delegate double GetMinSupportHandle(ItemSet<MultiLevelItem<T>> itemset);

        public ItemSets<MultiLevelItem<T>> MinePatterns(IEnumerable<TransactionWithMultiLevelItems<T>> database, List<MultiLevelItem<T>> domain3)
        {
            return MinePatterns(database, domain3, 
                (itemset) =>
                {
                    double minSupport = 1;
                    for (int i = 0; i < itemset.Count; ++i)
                    {
                        minSupport = System.Math.Min(minSupport, itemset[i].MinSupport);
                    }
                    return minSupport;
                });
        }

        public ItemSets<MultiLevelItem<T>> MinePatterns(IEnumerable<TransactionWithMultiLevelItems<T>> database, IList<MultiLevelItem<T>> domain, GetMinSupportHandle getMinItemSetSupport)
        {
            Dictionary<MultiLevelItem<T>, int> counts = new Dictionary<MultiLevelItem<T>, int>();
            for (int i = 0; i < domain.Count; ++i)
            {
                counts[domain[i]] = 0;
            }

            foreach (TransactionWithMultiLevelItems<T> transaction in database)
            {
                for (int i = 0; i < domain.Count; ++i)
                {
                    if (transaction.ContainsItem(domain[i]))
                    {
                        counts[domain[i]]++;
                    }
                }
            }

            List<MultiLevelItem<T>> freqItems = new List<MultiLevelItem<T>>();
            for (int i = 0; i < domain.Count; ++i)
            {
                if (counts[domain[i]] >= getMinItemSetSupport(new ItemSet<MultiLevelItem<T>>() { domain[i] }))
                {
                    freqItems.Add(domain[i]);
                }
            }

            MultiLevelItem<T>[] FList = freqItems.ToArray();
            Array.Sort(FList, (i1, i2) =>
            {
                int comp = counts[i1].CompareTo(counts[i2]);
                return -comp;
            });

            FPTree<MultiLevelItem<T>> fpTree = new FPTree<MultiLevelItem<T>>();

            int dbSize = 0;
            foreach (TransactionWithMultiLevelItems<T> transaction in database)
            {
                dbSize++;

                List<MultiLevelItem<T>> orderedFreqItems = new List<MultiLevelItem<T>>();
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

            ItemSets<MultiLevelItem<T>> allItemSets = new ItemSets<MultiLevelItem<T>>();
            for (int i = FList.Length - 1; i >= 0; i--)
            {
                MultiLevelItem<T> item = FList[i];
                List<ItemSet<MultiLevelItem<T>>> fis = fpTree.MinePatternsContaining(item, (candidate) => { return getMinItemSetSupport(candidate); });
                fpTree.RemoveFromLeaves(item);
                allItemSets.AddRange(fis);
            }

            return allItemSets;
        }
    }
}
