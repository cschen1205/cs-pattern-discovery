using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimuKit.DM.PatternDiscovery.FrequentPatterns;

namespace SimuKit.DM.PatternDiscovery.MultiLevelPatterns
{
    public class MultiLevelAprioriWithDbPartitioning<T>
        where T : IComparable<T>
    {
        protected AprioriWithDbPartitioning<MultiLevelItem<T>> mMethod = new AprioriWithDbPartitioning<MultiLevelItem<T>>();

        public MultiLevelAprioriWithDbPartitioning()
        {
            mMethod.CanJoinConstraint += new AprioriWithDbPartitioning<MultiLevelItem<T>>.CanJoinHandle(CanJoin);
        }

        public virtual ItemSets<MultiLevelItem<T>> MinePatterns(IEnumerable<TransactionWithMultiLevelItems<T>> database, MultiLevelItem<T> hierarchy, int partitionCount)
        {
            IList<MultiLevelItem<T>> domain2 = hierarchy.Flatten();
            domain2.Remove(hierarchy);

            return mMethod.MinePatterns(database, 
                (itemset) =>
                {
                    double minSupport = 1;
                    for (int i = 0; i < itemset.Count; ++i)
                    {
                        minSupport = System.Math.Min(minSupport, itemset[i].MinSupport);
                    }
                    return minSupport;
                }, domain2, partitionCount);
        }

        protected virtual bool CanJoin(ItemSet<MultiLevelItem<T>> itemset1, MultiLevelItem<T> k2)
        {
            MultiLevelItem<T> k1 = itemset1[itemset1.Count - 1];
            if (k1.IsDescendentOf(k2) || k2.IsDescendentOf(k1))
            {
                return false;
            }
            return true;
        }
    }
}
