using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternDiscovery
{
    public class ItemSet<T> : List<T>
        where T : IComparable<T>
    {
        protected List<long> mTransactionIDList = new List<long>();
        protected int mDbSize = 0;

        public List<long> TransactionIDList
        {
            get { return mTransactionIDList; }
        }

        public int DbSize
        {
            get { return mDbSize; }
            set { mDbSize = value; }
        }

        public double Support
        {
            get
            {
                return mDbSize == 0 ? 0 : (double)mTransactionCount / mDbSize;
            }
        }

        protected int mTransactionCount = 0;
        public int TransactionCount
        {
            get
            {
                return mTransactionCount;
            }
            set
            {
                mTransactionCount = value;
            }
        }

        public virtual ItemSet<T> Clone()
        {
            ItemSet<T> clone = new ItemSet<T>();
            clone.mTransactionCount = mTransactionCount;
            for (int i = 0; i < Count; ++i)
            {
                clone.Add(this[i]);
            }
            for (int i = 0; i < mTransactionIDList.Count; ++i)
            {
                long id = mTransactionIDList[i];
                clone.TransactionIDList.Add(id);
            }
            return clone;
        }

        public override bool Equals(object obj)
        {
            ItemSet<T> rhs = obj as ItemSet<T>;
            if (Count != rhs.Count) return false;

            for (int i = 0; i < rhs.Count; ++i)
            {
                if (!this[i].Equals(rhs[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < Count; ++i)
            {
                hash = hash * 31 + this[i].GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");
            for (int j = 0; j < this.Count; ++j)
            {
                if (j == 0)
                {
                    sb.AppendFormat("{0}", this[j]);
                }
                else
                {
                    sb.AppendFormat(", {0}", this[j]);
                }
            }
            sb.Append(" }");
            return sb.ToString();
        }

    }
}
