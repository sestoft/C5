using System;

namespace C5
{
    /// <summary>
    /// Base class (abstract) for ICollection implementations.
    /// </summary>
    public abstract class CollectionBase<T> : CollectionValueBase<T>
    {
        #region Fields

        /// <summary>
        /// The underlying field of the ReadOnly property
        /// </summary>
        protected bool isReadOnlyBase = false;

        /// <summary>
        /// The current stamp value
        /// </summary>
        protected int stamp;

        /// <summary>
        /// The number of items in the collection
        /// </summary>
        protected int size;

        /// <summary>
        /// The item equalityComparer of the collection
        /// </summary>
        protected readonly System.Collections.Generic.IEqualityComparer<T> itemequalityComparer;
        private int iUnSequencedHashCode, iUnSequencedHashCodeStamp = -1;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemequalityComparer"></param>
        protected CollectionBase(System.Collections.Generic.IEqualityComparer<T> itemequalityComparer)
        {
            this.itemequalityComparer = itemequalityComparer ?? throw new NullReferenceException("Item EqualityComparer cannot be null.");
        }

        #region Util

        /// <summary>
        /// Utility method for range checking.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if the start or count is negative or
        ///  if the range does not fit within collection size.</exception>
        /// <param name="start">start of range</param>
        /// <param name="count">size of range</param>
        protected void CheckRange(int start, int count)
        {
            if (start < 0 || count < 0 || start + count > size)
            {
                throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        /// Compute the unsequenced hash code of a collection
        /// </summary>
        /// <param name="items">The collection to compute hash code for</param>
        /// <param name="itemequalityComparer">The item equalitySCG.Comparer</param>
        /// <returns>The hash code</returns>
        public static int ComputeHashCode(ICollectionValue<T> items, System.Collections.Generic.IEqualityComparer<T> itemequalityComparer)
        {
            int h = 0;

            //But still heuristic: 
            //Note: the three odd factors should really be random, 
            //but there will be a problem with serialization/deserialization!
            //Two products is too few
            foreach (T item in items)
            {
                uint h1 = (uint)itemequalityComparer.GetHashCode(item);

                h += (int)((h1 * 1529784657 + 1) ^ (h1 * 2912831877) ^ (h1 * 1118771817 + 2));
            }

            return h;
            /*
                  The pairs (-1657792980, -1570288808) and (1862883298, -272461342) gives the same
                  unsequenced hashcode with this hashfunction. The pair was found with code like

                  HashDictionary<int, int[]> set = new HashDictionary<int, int[]>();
                  Random rnd = new C5Random(12345);
                  while (true)
                  {
                      int[] a = new int[2];
                      a[0] = rnd.Next(); a[1] = rnd.Next();
                      int h = unsequencedhashcode(a);
                      int[] b = a;
                      if (set.FindOrAdd(h, ref b))
                      {
                          Logger.Log(string.Format("Code {5}, Pair ({1},{2}) number {0} matched other pair ({3},{4})", set.Count, a[0], a[1], b[0], b[1], h));
                      }
                  }
                  */

        }

        // static Type isortedtype = typeof(ISorted<T>);

        /// <summary>
        /// Examine if collection1 and collection2 are equal as unsequenced collections
        /// using the specified item equalityComparer (assumed compatible with the two collections).
        /// </summary>
        /// <param name="collection1">The first collection</param>
        /// <param name="collection2">The second collection</param>
        /// <param name="itemequalityComparer">The item equalityComparer to use for comparison</param>
        /// <returns>True if equal</returns>
        public static bool StaticEquals(ICollection<T> collection1, ICollection<T> collection2, System.Collections.Generic.IEqualityComparer<T> itemequalityComparer)
        {
            if (object.ReferenceEquals(collection1, collection2))
            {
                return true;
            }

            // bug20070227:
            if (collection1 == null || collection2 == null)
            {
                return false;
            }

            if (collection1.Count != collection2.Count)
            {
                return false;
            }

            //This way we might run through both enumerations twice, but
            //probably not (if the hash codes are good)
            //TODO: check equal equalityComparers, at least here!
            if (collection1.GetUnsequencedHashCode() != collection2.GetUnsequencedHashCode())
            {
                return false;
            }

            //TODO: move this to the sorted implementation classes? 
            //Really depends on speed of InstanceOfType: we could save a cast
            {
                if (collection1 is ISorted<T> stit && collection2 is ISorted<T> stat && stit.Comparer == stat.Comparer)
                {
                    using System.Collections.Generic.IEnumerator<T> dat = collection2.GetEnumerator(), dit = collection1.GetEnumerator();
                    while (dit.MoveNext())
                    {
                        dat.MoveNext();
                        if (!itemequalityComparer.Equals(dit.Current, dat.Current))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            if (!collection1.AllowsDuplicates && (collection2.AllowsDuplicates || collection2.ContainsSpeed >= collection1.ContainsSpeed))
            {
                foreach (T x in collection1)
                {
                    if (!collection2.Contains(x))
                    {
                        return false;
                    }
                }
            }
            else if (!collection2.AllowsDuplicates)
            {
                foreach (T x in collection2)
                {
                    if (!collection1.Contains(x))
                    {
                        return false;
                    }
                }
            }
            // Now tit.AllowsDuplicates && tat.AllowsDuplicates
            else if (collection1.DuplicatesByCounting && collection2.DuplicatesByCounting)
            {
                foreach (T item in collection2)
                {
                    if (collection1.ContainsCount(item) != collection2.ContainsCount(item))
                    {
                        return false;
                    }
                }
            }
            else
            {
                // To avoid an O(n^2) algorithm, we make an aux hashtable to hold the count of items
                // bug20101103: HashDictionary<T, int> dict = new HashDictionary<T, int>();
                HashDictionary<T, int> dict = new HashDictionary<T, int>(itemequalityComparer);
                foreach (T item in collection2)
                {
                    int count = 1;
                    if (dict.FindOrAdd(item, ref count))
                    {
                        dict[item] = count + 1;
                    }
                }
                foreach (T item in collection1)
                {
                    var i = item;
                    if (dict.Find(ref i, out int count) && count > 0)
                    {
                        dict[item] = count - 1;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }

            return true;
        }


        /// <summary>
        /// Get the unsequenced collection hash code of this collection: from the cached 
        /// value if present and up to date, else (re)compute.
        /// </summary>
        /// <returns>The hash code</returns>
        public virtual int GetUnsequencedHashCode()
        {
            if (iUnSequencedHashCodeStamp == stamp)
            {
                return iUnSequencedHashCode;
            }

            iUnSequencedHashCode = ComputeHashCode(this, itemequalityComparer);
            iUnSequencedHashCodeStamp = stamp;
            return iUnSequencedHashCode;
        }


        /// <summary>
        /// Check if the contents of otherCollection is equal to the contents of this
        /// in the unsequenced sense.  Uses the item equality comparer of this collection
        /// </summary>
        /// <param name="otherCollection">The collection to compare to.</param>
        /// <returns>True if  equal</returns>
        public virtual bool UnsequencedEquals(ICollection<T> otherCollection)
        {
            return otherCollection != null && StaticEquals((ICollection<T>)this, otherCollection, itemequalityComparer);
        }


        /// <summary>
        /// Check if the collection has been modified since a specified time, expressed as a stamp value.
        /// </summary>
        /// <exception cref="CollectionModifiedException"> if this collection has been updated 
        /// since a target time</exception>
        /// <param name="thestamp">The stamp identifying the target time</param>
        protected virtual void ModifyCheck(int thestamp)
        {
            if (stamp != thestamp)
            {
                throw new CollectionModifiedException();
            }
        }


        /// <summary>
        /// Check if it is valid to perform update operations, and if so increment stamp.
        /// </summary>
        /// <exception cref="ReadOnlyCollectionException">If collection is read-only</exception>
        protected virtual void UpdateCheck()
        {
            if (isReadOnlyBase)
            {
                throw new ReadOnlyCollectionException();
            }

            stamp++;
        }

        #endregion

        #region ICollection<T> members

        /// <summary>
        /// 
        /// </summary>
        /// <value>True if this collection is read only</value>
        public virtual bool IsReadOnly => isReadOnlyBase;

        #endregion

        #region ICollectionValue<T> members
        /// <summary>
        /// 
        /// </summary>
        /// <value>The size of this collection</value>
        public override int Count => size;

        /// <summary>
        /// The value is symbolic indicating the type of asymptotic complexity
        /// in terms of the size of this collection (worst-case or amortized as
        /// relevant).
        /// </summary>
        /// <value>A characterization of the speed of the 
        /// <code>Count</code> property in this collection.</value>
        public override Speed CountSpeed => Speed.Constant;


        #endregion

        #region IExtensible<T> members

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual System.Collections.Generic.IEqualityComparer<T> EqualityComparer => itemequalityComparer;

        /// <summary>
        /// 
        /// </summary>
        /// <value>True if this collection is empty</value>
        public override bool IsEmpty => size == 0;

        #endregion

        #region IEnumerable<T> Members
        /// <summary>
        /// Create an enumerator for this collection.
        /// </summary>
        /// <returns>The enumerator</returns>
        public abstract override System.Collections.Generic.IEnumerator<T> GetEnumerator();
        #endregion
    }
}