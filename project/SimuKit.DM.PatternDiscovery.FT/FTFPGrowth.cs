using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimuKit.DM.PatternDiscovery.FrequentPatterns;

namespace SimuKit.DM.PatternDiscovery.FT
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
