using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimuKit.DM.PatternDiscovery.MultiLevelPatterns
{
    public class MultiLevelItem<T> : IComparable<MultiLevelItem<T>>
        where T : IComparable<T>
    {
        protected T mItem = default(T);
        public T Item
        {
            get { return mItem; }
            set { mItem = value; }
        }

        protected double mMinSupport = 0.1;
        public double MinSupport
        {
            get { return mMinSupport; }
            set { mMinSupport = value; }
        }

        protected MultiLevelItem<T> mParent = null;
        public MultiLevelItem<T> Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        protected List<MultiLevelItem<T>> mChildren = new List<MultiLevelItem<T>>();

        public int ChildCount
        {
            get { return mChildren.Count; }
        }

        public MultiLevelItem<T> GetChild(int index)
        {
            return mChildren[index];
        }

        public void AddChild(T item)
        {
            AddChild(new MultiLevelItem<T>()
                {
                    mItem = item
                }
            );
        }

        public void AddChild(MultiLevelItem<T> item)
        {
            mChildren.Add(item);
        }

        public List<MultiLevelItem<T>> LeafLevelItems
        {
            get
            {
                List<MultiLevelItem<T>> selection = new List<MultiLevelItem<T>>();
                GetLeafLevelItems(this, selection);

                return selection;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return mChildren.Count == 0;
            }
        }

        protected void GetLeafLevelItems(MultiLevelItem<T> node, List<MultiLevelItem<T>> selection)
        {
            if (node == null) return;
            if (node.IsLeaf)
            {
                selection.Add(node);
                return;
            }
            for (int i = 0; i < node.ChildCount; ++i)
            {
                GetLeafLevelItems(node.GetChild(i), selection);
            }
        }

        public override int GetHashCode()
        {
            return mItem.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            MultiLevelItem<T> rhs = obj as MultiLevelItem<T>;
            return rhs.mItem.Equals(mItem);
        }

        public override string ToString()
        {
            return mItem.ToString();
        }

        public int CompareTo(MultiLevelItem<T> obj)
        {
            MultiLevelItem<T> rhs = obj as MultiLevelItem<T>;
            return mItem.CompareTo(rhs.mItem);
        }

        public IList<MultiLevelItem<T>> Flatten()
        {
            List<MultiLevelItem<T>> selection = new List<MultiLevelItem<T>>();
            Flatten(this, selection);
            return selection;
        }

        protected void Flatten(MultiLevelItem<T> node, List<MultiLevelItem<T>> selection)
        {
            if (node == null) return;
            if (node.IsLeaf)
            {
                selection.Add(node);
                return;
            }
            for (int i = 0; i < node.ChildCount; ++i)
            {
                Flatten(node.GetChild(i), selection);
            }
        }

        public bool IsDescendentOf(MultiLevelItem<T> node)
        {
            return IsDescendentOf(this, node);
        }

        protected bool IsDescendentOf(MultiLevelItem<T> node1, MultiLevelItem<T> node2)
        {
            if (node1.Equals(node2)) return true;
            if (node2.IsLeaf)
            {
                return false;
            }
            for (int i = 0; i < node2.ChildCount; ++i)
            {
                if(IsDescendentOf(node1, node2.GetChild(i)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
