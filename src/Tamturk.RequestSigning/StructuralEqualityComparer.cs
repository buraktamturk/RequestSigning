using System.Collections;
using System.Collections.Generic;

namespace Tamturk {
    public class StructuralEqualityComparer<T> : IEqualityComparer<T> {
        public bool Equals(T x, T y) {
            return StructuralComparisons.StructuralEqualityComparer.Equals(x,y);
        }

        public int GetHashCode(T obj) {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
        }

        private static StructuralEqualityComparer<T> defaultComparer;
        public static StructuralEqualityComparer<T> Default {
            get
            {
                StructuralEqualityComparer<T> comparer = defaultComparer;
                if (comparer == null)
                {
                    comparer = new StructuralEqualityComparer<T>();
                    defaultComparer = comparer;
                }
                return comparer;
            }
        }
    }
}