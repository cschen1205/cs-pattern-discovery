using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimuKit.DM.PatternDiscovery.FrequentPatterns
{
    public class FPTreeNode<T>
        where T : IComparable<T>
    {
        protected T mItem;
        public T Item
        {
            get { return mItem; }
            set { mItem = value; }
        }

        public int mCount = 0;
        public int Count
        {
            get { return mCount; }
            set { mCount = value; }
        }

        protected FPTreeNode<T> mParent = null;
        public FPTreeNode<T> Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        protected List<FPTreeNode<T>> mChildren = new List<FPTreeNode<T>>();

        public void AddChild(FPTreeNode<T> node)
        {
            node.Parent = this;
            mChildren.Add(node);
        }

        public FPTreeNode<T> GetChild(int i)
        {
            return mChildren[i];
        }

        public int ChildCount
        {
            get
            {
                return mChildren.Count;
            }
        }

        public bool IsLeaf
        {
            get { return mChildren.Count == 0; }
        }

        public bool IsRoot
        {
            get { return mParent == null; }
        }

        public ItemSet<T> GetPath()
        {
            ItemSet<T> path = new ItemSet<T>();
            FPTreeNode<T> x = this;
            while (x != null)
            {
                if (!x.IsRoot)
                {
                    path.Add(x.Item);
                }
                x = x.Parent;
            }

            return path;
        }

        public bool RemoveChild(FPTreeNode<T> node)
        {
            return mChildren.Remove(node);
        }
    }
}
