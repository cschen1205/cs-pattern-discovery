using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatternDiscovery.FrequentPatterns;

namespace PatternDiscovery.UT
{
    [TestClass]
    public class UTApriori
    {
        [TestMethod]
        public void Example()
        {
            List<Transaction<char>> database = new List<Transaction<char>>();
            database.Add(new Transaction<char>('a', 'c', 'd', 'e') { ID = 10 });
            database.Add(new Transaction<char>('a', 'b', 'e') { ID = 20 });
            database.Add(new Transaction<char>('b', 'c', 'e') { ID = 30 });
            database.Add(new Transaction<char>('b', 'c', 'e') { ID = 40 });

            Apriori<char> method = new Apriori<char>();
            ItemSets<char> itemsets = method.MinePatterns(database, 2, new List<char>() { 'a', 'b', 'c', 'd', 'e' });
            for (int i = 0; i < itemsets.Count; ++i)
            {
                ItemSet<char> itemset = itemsets[i];
               
                Console.WriteLine(itemset);
            }
        }
    }
}
