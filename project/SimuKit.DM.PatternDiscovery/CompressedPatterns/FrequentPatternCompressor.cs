using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimuKit.DM.PatternDiscovery.CompressedPatterns
{
    public class FrequentPatternCompressor<T>
        where T : IComparable<T>
    {
        public delegate List<ItemSets<T>> ClusterHandle(double[][] distance_matrix, ItemSets<T> fis, int clusterCount);
        public event ClusterHandle ClusterMethod;

        public List<ItemSets<T>> Compress(ItemSets<T> fis, int clusterCount)
        {
            double[][] distance_matrix = new double[fis.Count][];
            for (int i = 0; i < fis.Count; ++i)
            {
                distance_matrix[i] = new double[fis.Count];
            }

            for (int i = 0; i < fis.Count-1; ++i)
            {
                for (int j = i + 1; j < fis.Count; ++j)
                {
                    distance_matrix[i][j] = GetPatternDistance(fis[i], fis[j], fis);
                    distance_matrix[i][j] = distance_matrix[j][i];
                }
            }

            return Cluster(distance_matrix, fis, clusterCount);


        }

        protected List<ItemSets<T>> Cluster(double[][] distance_matrix, ItemSets<T> fis, int clusterCount)
        {
            if (ClusterMethod != null)
            {
                return ClusterMethod(distance_matrix, fis, clusterCount);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private double GetPatternDistance(ItemSet<T> set1, ItemSet<T> set2, ItemSets<T> fis)
        {
            ItemSet<T> intersection_set = GetIntersection(set1, set2, fis);
            ItemSet<T> union_set = GetUnion(set1, set2, fis);
            int union_count = union_set != null ? union_set.Count : 0;
            int intersection_count = intersection_set != null ? intersection_set.Count : 0;
            if (intersection_count == 0) return 1;
            return 1 - (double)union_count / intersection_count;
        }

        private bool IsSubsetOf(ItemSet<T> set1, ItemSet<T> set2)
        {
            for (int i = 0; i < set1.Count; ++i)
            {
                if (!set2.Contains(set1[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private ItemSet<T> GetUnion(ItemSet<T> set1, ItemSet<T> set2, ItemSets<T> fis)
        {
            if (IsSubsetOf(set1, set2))
            {
                return set2;
            }
            else if(IsSubsetOf(set2, set1))
            {
                return set1;
            }

            HashSet<T> temp = new HashSet<T>();
            for (int i = 0; i < set1.Count; ++i)
            {
                temp.Add(set1[i]);
            }
            for (int i = 0; i < set2.Count; ++i)
            {
                temp.Add(set2[i]);
            }

            List<T> temp2 = temp.ToList();
            temp2.Sort();

            ItemSet<T> merged_set = new ItemSet<T>();
            for (int i = 0; i < temp2.Count; ++i)
            {
                merged_set.Add(temp2[i]);
            }

            for (int i = 0; i < fis.Count; ++i)
            {
                if (fis[i].Equals(merged_set))
                {
                    return fis[i];
                }
            }

            return null;
        }

        private ItemSet<T> GetIntersection(ItemSet<T> set1, ItemSet<T> set2, ItemSets<T> fis)
        {
            if (IsSubsetOf(set1, set2))
            {
                return set1;
            }
            else if (IsSubsetOf(set2, set1))
            {
                return set2;
            }

            HashSet<T> temp1 = new HashSet<T>();
            for (int i = 0; i < set1.Count; ++i)
            {
                temp1.Add(set1[i]);
            }
            HashSet<T> temp2 = new HashSet<T>();
            for (int i = 0; i < set2.Count; ++i)
            {
                if (temp1.Contains(set2[i]))
                {
                    temp2.Add(set2[i]);
                }
            }

            List<T> temp = temp2.ToList();
            temp.Sort();

            ItemSet<T> intersection_set = new ItemSet<T>();
            for (int i = 0; i < temp.Count; ++i)
            {
                intersection_set.Add(temp[i]);
            }

            for (int i = 0; i < fis.Count; ++i)
            {
                if (fis[i].Equals(intersection_set))
                {
                    return fis[i];
                }
            }

            return null;
        }


    }
}
