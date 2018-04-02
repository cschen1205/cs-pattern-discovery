using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimuKit.DM.PatternDiscovery.FrequentPatterns
{
    public class FPTree<T> 
        where T : IComparable<T>
    {
        public delegate double GetMinSupportHandle(ItemSet<T> itemset);
        
        protected FPTreeNode<T> mRoot;
        public FPTree()
        {
            mRoot = new FPTreeNode<T>();
        }

        public void AddOrderedFreqItems(List<T> orderedFreqItems)
        {
            Append(mRoot, orderedFreqItems, 0);
        }

        protected int mDbSize = 0;
        public int DbSize
        {
            get { return mDbSize; }
            set { mDbSize = value; }
        }
        protected void Append(FPTreeNode<T> node, List<T> orderedFreqItems, int d)
        {
            int selectedIndex = -1;
            for (int i = 0; i < node.ChildCount; ++i)
            {
                if (node.GetChild(i).Item.Equals(orderedFreqItems[d]))
                {
                    selectedIndex = i;
                    break;
                }
            }

            if (selectedIndex != -1)
            {
                node.GetChild(selectedIndex).Count++;
                if (d < orderedFreqItems.Count - 1)
                {
                    Append(node.GetChild(selectedIndex), orderedFreqItems, d + 1);
                }
            }
            else
            {
                FPTreeNode<T> child = new FPTreeNode<T>();
                child.Item = orderedFreqItems[d];
                child.Count = 1;
                node.AddChild(child);
                if (d < orderedFreqItems.Count - 1)
                {
                    Append(child, orderedFreqItems, d + 1);
                }
            }
        }

        public List<ItemSet<T>> MinePatternsContaining(T item, double minSupport)
        {
            List<ItemSet<T>> selection = new List<ItemSet<T>>();
            MinePatternsContaining(mRoot, selection, item, minSupport);

            return selection;
        }

        public List<ItemSet<T>> MinePatternsContaining(T item, GetMinSupportHandle getMinItemSetSupport)
        {
            List<ItemSet<T>> selection = new List<ItemSet<T>>();
            MinePatternsContaining(mRoot, selection, item, getMinItemSetSupport);

            return selection;
        }

        public void RemoveFromLeaves(T item)
        {
            RemoveFromLeaves(mRoot, item);
        }

        protected void RemoveFromLeaves(FPTreeNode<T> node, T item)
        {
            if (node.IsLeaf)
            {
                if (node.Item.Equals(item))
                {
                    node.Parent.RemoveChild(node);
                }
                return;
            }

            for (int i = 0; i < node.ChildCount; ++i)
            {
                RemoveFromLeaves(node.GetChild(i), item);
            }
        }

        protected void MinePatternsContaining(FPTreeNode<T> node, List<ItemSet<T>> selection, T targetItem, double minSupport)
        {
            if (node.IsLeaf)
            {
                if (node.Item.Equals(targetItem) && node.Count >= minSupport * mDbSize)
                {
                    ItemSet<T> fis = new ItemSet<T>();
                    fis = node.GetPath();
                    fis.TransactionCount = node.Count;
                    fis.DbSize = mDbSize;
                    selection.Add(fis);
                }
                return;
            }
            for (int i = 0; i < node.ChildCount; ++i)
            {
                MinePatternsContaining(node.GetChild(i), selection, targetItem, minSupport);
            }
        }

        protected void MinePatternsContaining(FPTreeNode<T> node, List<ItemSet<T>> selection, T targetItem, GetMinSupportHandle getMinItemSetSupport)
        {
            if (node.IsLeaf)
            {
                if (node.Item.Equals(targetItem))
                {
                    ItemSet<T> fis = new ItemSet<T>();
                    fis = node.GetPath();
                    fis.TransactionCount = node.Count;
                    fis.DbSize = mDbSize;
                    if (node.Count >= getMinItemSetSupport(fis) * mDbSize)
                    {
                        selection.Add(fis);
                    }
                }
                return;
            }
            for (int i = 0; i < node.ChildCount; ++i)
            {
                MinePatternsContaining(node.GetChild(i), selection, targetItem, getMinItemSetSupport);
            }
        }
    }
}
