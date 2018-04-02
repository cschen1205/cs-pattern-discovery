using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimuKit.DM.PatternDiscovery
{
    public class Evaluation
    {
        /// <summary>
        /// Confidence measure [0, infinity]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="A"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public static double GetConfidence<T>(ItemSet<T> A, ItemSets<T> all)
            where T : IComparable<T>
        {
            double sum_supp = 0;
            foreach (ItemSet<T> itemset in all)
            {
                sum_supp += itemset.Support;
            }
            return A.Support / sum_supp;
        }

        /// <summary>
        /// Interesting measure [0, infinity]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="A_Join_B"></param>
        /// <returns></returns>
        public static double GetLift<T>(ItemSet<T> A, ItemSet<T> B, ItemSet<T> A_Join_B)
            where T : IComparable<T>
        {
            return A_Join_B.Support / (A.Support * B.Support);
        }

        public static bool IsIndependent<T>(ItemSet<T> A, ItemSet<T> B, ItemSet<T> A_Join_B)
            where T : IComparable<T>
        {
            double lift = GetLift(A, B, A_Join_B);
            if (lift == 1) return true;
            return false;
        }

        public static bool IsPositivelyCorrelated<T>(ItemSet<T> A, ItemSet<T> B, ItemSet<T> A_Join_B)
            where T : IComparable<T>
        {
            double lift = GetLift(A, B, A_Join_B);
            if (lift > 1) return true;
            return false;
        }

        public static bool IsNegativelyCorrelated<T>(ItemSet<T> A, ItemSet<T> B, ItemSet<T> A_Join_B)
            where T : IComparable<T>
        {
            double lift = GetLift(A, B, A_Join_B);
            if (lift < 1) return true;
            return false;
        }

        public static double GetChiSquare<T>(ItemSet<T> AB, ItemSet<T> notAB, ItemSet<T> AnotB, ItemSet<T> notAnotB)
            where T : IComparable<T>
        {
            int totalBCount = AB.TransactionCount + notAB.TransactionCount;
            int totalNotBCount = AnotB.TransactionCount + notAnotB.TransactionCount;
            int totalACount = AB.TransactionCount + AnotB.TransactionCount;
            int totalNotACount = notAB.TransactionCount + notAnotB.TransactionCount;

            double expectedABCount = (double)totalACount * totalBCount / (totalBCount + totalNotBCount);
            double expectedNotABCount = (double)totalNotACount * totalBCount / (totalBCount + totalNotBCount);
            double expectedANotBCount = (double)totalACount * totalNotBCount / (totalBCount + totalNotBCount);
            double expectedNotANotBCount = (double)totalNotACount * totalNotBCount / (totalBCount + totalNotBCount);

            double chiSqr = System.Math.Pow(AB.TransactionCount - expectedABCount, 2) / expectedABCount;
            chiSqr += System.Math.Pow(AnotB.TransactionCount - expectedANotBCount, 2) / expectedANotBCount;
            chiSqr += System.Math.Pow(notAnotB.TransactionCount - expectedNotANotBCount, 2) / expectedNotANotBCount;
            chiSqr += System.Math.Pow(notAB.TransactionCount - expectedNotABCount, 2) / expectedNotABCount;

            return chiSqr;
        }

        /// <summary>
        /// Null-invariant measure [0, 1]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="A_Join_B"></param>
        /// <returns></returns>
        public static double AllConf<T>(ItemSet<T> A, ItemSet<T> B, ItemSet<T> A_Join_B)
            where T : IComparable<T>
        {
            double maxSup = System.Math.Max(A.Support, B.Support);
            return A_Join_B.Support / maxSup;
        }

        /// <summary>
        /// Null-invariant measure [0, 1]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="A_Join_B"></param>
        /// <returns></returns>
        public static double Jaccard<T>(ItemSet<T> A, ItemSet<T> B, ItemSet<T> A_Join_B)
             where T : IComparable<T>
        {
            return A_Join_B.Support / (A.Support + B.Support - A_Join_B.Support);
        }

        /// <summary>
        /// Null-invariant measure between [0, 1]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="A_Join_B"></param>
        /// <returns></returns>
        public static double Cosine<T>(ItemSet<T> A, ItemSet<T> B, ItemSet<T> A_Join_B)
             where T : IComparable<T>
        {
            return A_Join_B.Support / System.Math.Sqrt(A.Support * B.Support);
        }

        /// <summary>
        /// Null-invariant measure between [0, 1]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="A_Join_B"></param>
        /// <returns></returns>
        public static double Kulczynski<T>(ItemSet<T> A, ItemSet<T> B, ItemSet<T> A_Join_B)
             where T : IComparable<T>
        {
            return 0.5 * (A_Join_B.Support / A.Support + A_Join_B.Support / B.Support);
        }
    }
}
