using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternDiscovery
{
    public class Transaction<T>
        where T : IComparable<T>
    {
        private long mID;
        private static long GlobalID = 0;
        protected List<T> mItems = new List<T>();

        public virtual bool ContainsItem(T item)
        {
            return mItems.Contains(item);
        }

        public int ItemCount
        {
            get
            {
                return mItems.Count;
            }
        }

        public void AddItem(T item)
        {
            mItems.Add(item);
        }

        public T this[int index]
        {
            get
            {
                return mItems[index];
            }
            set
            {
                mItems[index] = value;
            }
        }

        public bool ContainsItemSet(ItemSet<T> itemset)
        {
            for (int i = 0; i < itemset.Count; ++i)
            {
                T item = itemset[i];
                if (!this.ContainsItem(item))
                {
                    return false;
                }
            }
            return true;
        }

        public Transaction()
        {
            mID = GlobalID++;
        }

        public Transaction(params T[] items)
        {
            foreach (T item in items)
            {
                this.AddItem(item);
            }
            mID = GlobalID++;
        }

        public static List<T> ExtractDomain(IEnumerable<Transaction<T>> transactions)
        {
            HashSet<T> uniqueItems = new HashSet<T>();
            foreach (Transaction<T> transaction in transactions)
            {
                for (int i = 0; i < transaction.ItemCount; ++i)
                {
                    uniqueItems.Add(transaction[i]);
                }
            }

            return uniqueItems.ToList();
        }

        public long ID
        {
            get
            {
                return mID;
            }
            set
            {
                mID = value;
            }
        }

        public override bool Equals(object obj)
        {
            Transaction<T> rhs = obj as Transaction<T>;
            if (rhs.ID != ID) return false;
            if (rhs.ItemCount != ItemCount) return false;

            for (int i = 0; i < ItemCount; ++i)
            {
                if (!rhs[i].Equals(this[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = mID.GetHashCode();
            for (int i = 0; i < ItemCount; ++i)
            {
                hash = hash * 31 + this[i].GetHashCode();
            }
            return hash;
        }
    }
}
