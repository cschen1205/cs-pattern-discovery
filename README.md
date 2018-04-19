# cs-pattern-discovery

Pattern Discovery implemented in C#

# Usage

### Apriori

The [sample codes](project/PatternDiscovery.FT/FTApriori.cs) shows how to use Apriori to find the frequent item sets from a transaction database:

```cs 
using System;
using System.Collections.Generic;
using PatternDiscovery.FrequentPatterns;

namespace PatternDiscovery.FT
{
    public class FTApriori
    {
        public static void Example()
        {
            List<Transaction<char>> database = new List<Transaction<char>>();
            database.Add(new Transaction<char>('a', 'c', 'd', 'e') { ID = 10 });
            database.Add(new Transaction<char>('a', 'b', 'e') { ID = 20 });
            database.Add(new Transaction<char>('b', 'c', 'e') { ID = 30 });
            database.Add(new Transaction<char>('b', 'c', 'e') { ID = 40 });

            Apriori<char> method = new Apriori<char>();
            ItemSets<char> itemsets = method.MinePatterns(database, 0.5, new List<char>() { 'a', 'b', 'c', 'd', 'e' });
            for (int i = 0; i < itemsets.Count; ++i)
            {
                ItemSet<char> itemset = itemsets[i];
               
                Console.WriteLine(itemset);
            }
        }
    }
}
```

### Apriori with DB Partitioning

The [sample codes](project/PatternDiscovery.FT/FTAprioriWithDbPartitioning.cs) shows how to use Apriori with DB Partitioning to find the frequent item sets from a transaction database:

```cs 
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
```

### FP-Growth

The [sample codes](project/PatternDiscovery.FT/FTFPGrowth.cs) shows how to use fp-growth to mine patterns and discover closed patterns:

```cs
using System;
using System.Collections.Generic;
using PatternDiscovery.FrequentPatterns;

namespace PatternDiscovery.FT
{
    public class FTFPGrowth
    {
        public static void Example()
        {
            List<Transaction<char>> database = new List<Transaction<char>>();
            database.Add(new Transaction<char>('f', 'a', 'c', 'd', 'g', 'i', 'm', 'p') { ID = 100 });
            database.Add(new Transaction<char>('a', 'b', 'c', 'f', 'l', 'm', 'o') { ID = 200 });
            database.Add(new Transaction<char>('b', 'f', 'h', 'j', 'o', 'w') { ID = 300 });
            database.Add(new Transaction<char>('b', 'c', 'k', 's', 'p') { ID = 400 });
            database.Add(new Transaction<char>('a', 'f', 'c', 'e', 'l', 'p', 'm', 'n') { ID = 500 });


            Console.WriteLine("Using FPGrowth");
            DateTime start_time = DateTime.UtcNow;
            FPGrowth<char> method = new FPGrowth<char>();
            ItemSets<char> fis = method.MinePatterns(database, Transaction<char>.ExtractDomain(database), 0.4);
            DateTime end_time = DateTime.UtcNow;
            Show(fis);
            Console.WriteLine("Time Span: {0} ms", (end_time - start_time).TotalMilliseconds);

            Console.WriteLine("Finding Closed Pattern");
            Show(method.FindMaxPatterns(database, Transaction<char>.ExtractDomain(database), 0.4));

            Console.WriteLine("Using baseline Apriori");
            start_time = DateTime.UtcNow;
            Apriori<char> baseline_method = new Apriori<char>();
            fis = method.MinePatterns(database, Transaction<char>.ExtractDomain(database), 0.4);
            end_time = DateTime.UtcNow;
            Show(fis);
            Console.WriteLine("Time Span: {0} ms", (end_time - start_time).TotalMilliseconds);
        }

        private static void Show(ItemSets<char> fis)
        {
            for (int i = 0; i < fis.Count; ++i)
            {
                Console.WriteLine("{0} (Support: {1})", fis[i], fis[i].Support);
            }
        }
    }
}
```

### 
