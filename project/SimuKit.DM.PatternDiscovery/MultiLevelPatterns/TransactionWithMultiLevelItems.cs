using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimuKit.DM.PatternDiscovery.MultiLevelPatterns
{
    public class TransactionWithMultiLevelItems<T> : Transaction<MultiLevelItem<T>>
        where T : IComparable<T>
    {
        public override bool ContainsItem(MultiLevelItem<T> item)
        {
            foreach (MultiLevelItem<T> leaf_item in item.LeafLevelItems)
            {
                if (mItems.Contains(leaf_item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
