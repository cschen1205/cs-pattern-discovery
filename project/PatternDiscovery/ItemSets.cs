using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternDiscovery
{
    public class ItemSets<T> : List<ItemSet<T>>
        where T : IComparable<T>
    {
    }
}
