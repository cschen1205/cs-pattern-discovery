using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimuKit.DM.PatternDiscovery.FrequentPatterns
{
    public class Apriori<T>
        where T : IComparable<T>
    {
        public delegate void GetCountHandle(IEnumerable<ItemSet<T>> itemsetSup);
        public delegate double GetMinSupportHandle(ItemSet<T> itemset);
        public delegate bool CanJoinHandle(ItemSet<T> itemset, T k2);
        public event CanJoinHandle CanJoinConstraint;

        public virtual ItemSets<T> MinePatterns(IEnumerable<Transaction<T>> database, double minSup, IList<T> domain)
        {
            return MinePatterns(database, (itemset) => { return minSup; }, domain);
        }

        public virtual ItemSets<T> MinePatterns(IEnumerable<Transaction<T>> database, GetMinSupportHandle getMinItemSetSupport, IList<T> domain)
        {
            return MinePatterns((itemsets) =>
                {
                    foreach (ItemSet<T> itemset in itemsets)
                    {
                        itemset.TransactionCount = 0;
                    }
                    int dbSize = 0;
                    
                    foreach (Transaction<T> transaction in database)
                    {
                        foreach (ItemSet<T> itemset in itemsets)
                        {
                            if (transaction.ContainsItemSet(itemset))
                            {
                                itemset.TransactionCount++;
                            }
                        }
                        dbSize++;
                    }

                    foreach (ItemSet<T> itemset in itemsets)
                    {
                        itemset.DbSize = dbSize;
                    }

                }, getMinItemSetSupport, domain);
        }

        public virtual ItemSets<T> MinePatterns(GetCountHandle UpdateItemSetSupport, double minSup, IList<T> domain)
        {
            return MinePatterns(UpdateItemSetSupport, (itemset) => { return minSup; }, domain);
        }

        public virtual ItemSets<T> MinePatterns(GetCountHandle updateItemSetSupport, GetMinSupportHandle getMinItemSetSupport, IList<T> domain)
        {
            ItemSets<T> Fk = new ItemSets<T>();
            List<ItemSet<T>> itemsetSup = new List<ItemSet<T>>();
            for (int i = 0; i < domain.Count; ++i)
            {
                T item = domain[i];
                ItemSet<T> itemset = new ItemSet<T>() { item };
                itemset.TransactionCount = 0;
                itemsetSup.Add(itemset);
            }

            updateItemSetSupport(itemsetSup);
            foreach(ItemSet<T> itemset in itemsetSup)
            {
                if (itemset.Support >= getMinItemSetSupport(itemset))
                {
                    Fk.Add(itemset);
                }
            }

            int k=1;
            ItemSets<T> allFrequentItemSets = new ItemSets<T>();
            allFrequentItemSets.AddRange(Fk);
            while(Fk.Count > 0)
            {
                ItemSets<T> Fkp1 = new ItemSets<T>();

                //do self-join
                for(int i=0; i < Fk.Count; ++i)
                {
                    for(int j=0; j < Fk.Count; ++j)
                    {
                        if(i == j) continue;
                        bool canJoin = true;

                        for(int l=0; l < k-1; ++l)
                        {
                            if(Fk[i][l].CompareTo(Fk[j][l]) != 0)
                            {
                                canJoin = false;
                                break;
                            }
                        }
                        if(canJoin)
                        {
                            if(CanJoin(Fk[i], Fk[j][k-1]))
                            {
                                ItemSet<T> c = Fk[i].Clone();
                                c.Add(Fk[j][k - 1]);
                                Fkp1.Add(c);
                            }
                        }
                    }
                }

                
                updateItemSetSupport(Fkp1);

                List<ItemSet<T>> fis = new List<ItemSet<T>>();
                foreach (ItemSet<T> itemset in Fkp1)
                {
                    if(itemset.Support >= getMinItemSetSupport(itemset))
                    {
                        fis.Add(itemset);
                    }
                }
               
                allFrequentItemSets.AddRange(fis);

                Fk.Clear();
                Fk.AddRange(fis);
                k++;
            }

            return allFrequentItemSets;
        }

        protected virtual bool CanJoin(ItemSet<T> itemset1, T k2)
        {
            T k1 = itemset1[itemset1.Count - 1];
            if (k1.CompareTo(k2) < 0)
            {
                if (CanJoinConstraint != null)
                {
                    return CanJoinConstraint(itemset1, k2);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}
