using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternDiscovery.FrequentPatterns
{
    /// <summary>
    /// This implements the partitioning technique proposed by (Savasere, et al., 1995)
    /// In this version, only two passes over the database is required
    /// </summary>
    public class AprioriWithDbPartitioning<T>
        where T : IComparable<T>
    {
        public delegate double GetMinSupportHandle(ItemSet<T> itemset);
        public delegate bool CanJoinHandle(ItemSet<T> itemset1, T k2);
        public event CanJoinHandle CanJoinConstraint;

        protected ItemSets<T> GenerateLargeItemSets(List<Transaction<T>> partition, double minSupport, IList<T> domain)
        {
            return GenerateLargeItemSets(partition, (itemset) => { return minSupport; }, domain);
        }

        protected ItemSets<T> GenerateLargeItemSets(List<Transaction<T>> partition, GetMinSupportHandle getMinItemSetSupport, IList<T> domain)
        {
            ItemSets<T> Fk = new ItemSets<T>();
            for (int i = 0; i < domain.Count; ++i)
            {
                T item = domain[i];
                ItemSet<T> itemset = new ItemSet<T>() { item };

                for(int j = 0; j < partition.Count; ++j)
                {
                    if (partition[j].ContainsItemSet(itemset))
                    {
                        long tid = partition[j].ID;
                        itemset.TransactionIDList.Add(tid);
                    }
                }

                if (itemset.TransactionIDList.Count >= getMinItemSetSupport(itemset) * partition.Count)
                {
                    Fk.Add(itemset);
                }
            }

            int k = 1;
            ItemSets<T> allFrequentItemSets = new ItemSets<T>();
            allFrequentItemSets.AddRange(Fk);
            while (Fk.Count > 0)
            {
                ItemSets<T> Fkp1 = new ItemSets<T>();

                //do self-join
                for (int i = 0; i < Fk.Count; ++i)
                {
                    for (int j = 0; j < Fk.Count; ++j)
                    {
                        if (i == j) continue;
                        bool canJoin = true;
                        for (int l = 0; l < k - 1; ++l)
                        {
                            if (Fk[i][l].CompareTo(Fk[j][l]) != 0)
                            {
                                canJoin = false;
                                break;
                            }
                        }
                        if (canJoin)
                        {
                            if (CanJoin(Fk[i], Fk[j][k - 1]))
                            {
                                ItemSet<T> c = Union(Fk[i], Fk[j]);
                                if (c.TransactionIDList.Count >= getMinItemSetSupport(c) * partition.Count)
                                {
                                    Fkp1.Add(c);
                                }
                            }
                        }
                    }
                }

                allFrequentItemSets.AddRange(Fkp1);
                Fk = Fkp1;
                k++;
            }

            return allFrequentItemSets;
        }

        protected virtual bool CanJoin(ItemSet<T> item1, T k2)
        {
            T k1 = item1[item1.Count - 1];
            if (k1.CompareTo(k2) < 0)
            {
                if (CanJoinConstraint != null)
                {
                    return CanJoinConstraint(item1, k2);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        protected ItemSet<T> Union(ItemSet<T> is1, ItemSet<T> is2)
        {
            ItemSet<T> c = is1.Clone();
            c.Add(is2[is2.Count - 1]);

            foreach (long id2 in is2.TransactionIDList)
            {
                bool isFound = false;
                foreach (long id1 in is1.TransactionIDList)
                {
                    if (id1 == id2)
                    {
                        isFound = true;
                        break;
                    }
                }

                if (isFound)
                {
                    c.TransactionIDList.Add(id2);
                }
            }
            return c;
        }

        protected virtual List<Transaction<T>> ReadInPartition(int i, IEnumerable<Transaction<T>> database)
        {
            int k = 1;
            List<Transaction<T>> partition = new List<Transaction<T>>();
            foreach(Transaction<T> transaction in database)
            {
                if (k % (i+1) == 0)
                {
                    partition.Add(transaction);
                }
                k++;
            }

            return partition;
        }

        protected int GetCount(List<Transaction<T>> database, ItemSet<T> itemset)
        {
            int support = 0;
            for (int i = 0; i < database.Count; ++i)
            {
                if (database[i].ContainsItemSet(itemset))
                {
                    support++;
                }
            }

            return support;
        }

        public virtual ItemSets<T> MinePatterns(IEnumerable<Transaction<T>> database, double minSupport, IList<T> domain, int partitionCount)
        {
            return MinePatterns(database, (itemset) => { return minSupport; }, domain, partitionCount);
        }

        public virtual ItemSets<T> MinePatterns(IEnumerable<Transaction<T>> database, GetMinSupportHandle getMinItemSetSupport, IList<T> domain, int partitionCount)
        {
            HashSet<ItemSet<T>> candidates = new HashSet<ItemSet<T>>();
            for (int i = 0; i < partitionCount; ++i)
            {
                List<Transaction<T>> partition = ReadInPartition(i, database);
                ItemSets<T> fis = GenerateLargeItemSets(partition, getMinItemSetSupport, domain);
                foreach (ItemSet<T> itemset in fis)
                {
                    candidates.Add(itemset);
                }
            }

            
            int dbSize = 0;
            for (int i = 0; i < partitionCount; ++i)
            {
                List<Transaction<T>> partition = ReadInPartition(i, database);
                dbSize+=partition.Count;

                foreach(ItemSet<T> itemset in candidates)
                {
                    itemset.TransactionCount += GetCount(partition, itemset);
                }
            }

            foreach(ItemSet<T> itemset in candidates)
            {
                itemset.DbSize = dbSize;
            }

            ItemSets<T> C = new ItemSets<T>();
            foreach(ItemSet<T> itemset in candidates)
            {
                if (itemset.Support >= getMinItemSetSupport(itemset))
                {
                    C.Add(itemset);
                }
            }

            return C;
        }

        

        
    }
}
