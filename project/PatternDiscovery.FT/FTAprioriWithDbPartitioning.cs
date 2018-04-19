using System;
using System.Collections.Generic;
using PatternDiscovery.FrequentPatterns;

namespace PatternDiscovery.FT
{
    public class FTAprioriWithDbPartitioning
    {
        public static void Example()
        {
            List<Transaction<char>> database = new List<Transaction<char>>();
            database.Add(new Transaction<char>('a', 'c', 'd', 'e') { ID = 10 });
            database.Add(new Transaction<char>('a', 'b', 'e') { ID = 20 });
            database.Add(new Transaction<char>('b', 'c', 'e') { ID = 30 });
            database.Add(new Transaction<char>('b', 'c', 'e') { ID = 40 });

            AprioriWithDbPartitioning<char> method = new AprioriWithDbPartitioning<char>();

            
            ItemSets<char> itemsets = method.MinePatterns(database, 0.5, new List<char>() { 'a', 'b', 'c', 'd', 'e' }, 3);
            for (int i = 0; i < itemsets.Count; ++i)
            {
                ItemSet<char> itemset = itemsets[i];
               
                Console.WriteLine(itemset);
            }
        }
    }
}
