using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PatternDiscovery.FrequentPatterns;

namespace PatternDiscovery.MultiLevelPatterns
{
    public class MultiLevelApriori<T> 
        where T : IComparable<T>
    {
        protected Apriori<MultiLevelItem<T>> mMethod = new Apriori<MultiLevelItem<T>>();

        public MultiLevelApriori()
        {
            mMethod.CanJoinConstraint += new Apriori<MultiLevelItem<T>>.CanJoinHandle(CanJoin);
        }

        public virtual ItemSets<MultiLevelItem<T>> MinePatterns(IEnumerable<TransactionWithMultiLevelItems<T>> database, MultiLevelItem<T> hierarchy)
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
                }, domain2);
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
