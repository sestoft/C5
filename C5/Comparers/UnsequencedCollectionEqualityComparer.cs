using SCG = System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// Prototype for an unsequenced equalityComparer for something (T) that implements ICollection[W]
    /// This will use ICollection[W] specific implementations of the equalityComparer operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="W"></typeparam>
    public class UnsequencedCollectionEqualityComparer<T, W> : SCG.IEqualityComparer<T>
        where T : ICollection<W>
    {
        private static UnsequencedCollectionEqualityComparer<T, W>? _cached;

        private UnsequencedCollectionEqualityComparer() { }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public static UnsequencedCollectionEqualityComparer<T, W> Default => _cached ??= new UnsequencedCollectionEqualityComparer<T, W>();
        /// <summary>
        /// Get the hash code with respect to this unsequenced equalityComparer
        /// </summary>
        /// <param name="collection">The collection</param>
        /// <returns>The hash code</returns>
        public int GetHashCode(T collection) { return collection.GetUnsequencedHashCode(); }


        /// <summary>
        /// Check if two collections are equal with respect to this unsequenced equalityComparer
        /// </summary>
        /// <param name="collection1">first collection</param>
        /// <param name="collection2">second collection</param>
        /// <returns>True if equal</returns>
        public bool Equals(T? collection1, T? collection2)
        {
            if (collection1 == null && collection2 == null) return true;
            if (collection1 == null) return false;
            if (collection2 == null) return false;

            return collection1.UnsequencedEquals(collection2);
        }
    }
}