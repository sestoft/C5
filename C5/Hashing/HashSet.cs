// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// A set collection class based on linear hashing
    /// </summary>
    [Serializable]
    public class HashSet<T> : CollectionBase<T>, ICollection<T>
    {
        #region Feature
        /// <summary>
        /// Enum class to assist printing of compilation alternatives.
        /// </summary>
        [Flags]
        public enum Feature : short
        {
            /// <summary>
            /// Nothing
            /// </summary>
            Dummy = 0,
            /// <summary>
            /// Buckets are of reference type
            /// </summary>
            RefTypeBucket = 1,
            /// <summary>
            /// Primary buckets are of value type
            /// </summary>
            ValueTypeBucket = 2,
            /// <summary>
            /// Using linear probing to resolve index clashes
            /// </summary>
            LinearProbing = 4,
            /// <summary>
            /// Shrink table when very sparsely filled
            /// </summary>
            ShrinkTable = 8,
            /// <summary>
            /// Use chaining to resolve index clashes
            /// </summary>
            Chaining = 16,
            /// <summary>
            /// Use hash function on item hash code
            /// </summary>
            InterHashing = 32,
            /// <summary>
            /// Use a universal family of hash functions on item hash code
            /// </summary>
            RandomInterHashing = 64
        }


        private static readonly Feature features = Feature.Dummy
                                          | Feature.RefTypeBucket
                                          | Feature.Chaining
                                          | Feature.RandomInterHashing;


        /// <summary>
        /// Show which implementation features was chosen at compilation time
        /// </summary>
        public static Feature Features => features;

        #endregion
        #region Fields
        private int indexmask;
        private int bits;
        private int bitsc;
        private readonly int origbits;
        private int lastchosen;
        private Bucket?[] table;
        private readonly double fillfactor = 0.66;
        private int resizethreshhold;

        private static readonly Random Random = new Random();
        private uint _randomhashfactor;

        #endregion

        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public override EventType ListenableEvents => EventType.Basic;

        #endregion

        #region Bucket nested class(es)
        [Serializable]
        private class Bucket
        {
            internal T item;

            internal int hashval; //Cache!

            internal Bucket? overflow;

            internal Bucket(T item, int hashval, Bucket? overflow)
            {
                this.item = item;
                this.hashval = hashval;
                this.overflow = overflow;
            }
        }

        #endregion

        #region Basic Util

        private bool Equals(T i1, T i2) { return itemequalityComparer.Equals(i1, i2); }

        private int GetHashCode(T item) { return itemequalityComparer.GetHashCode(item); }

        private int Hv2i(int hashval)
        {
            return (int)(((uint)hashval * _randomhashfactor) >> bitsc);
        }

        private void Expand()
        {
            Logger.Log(string.Format(string.Format("Expand to {0} bits", bits + 1)));
            Resize(bits + 1);
        }

        /*
        void shrink()
        {
            if (bits > 3)
            {
                Logger.Log(string.Format(string.Format("Shrink to {0} bits", bits - 1)));
                resize(bits - 1);
            }
        } */


        private void Resize(int bits)
        {
            Logger.Log(string.Format(string.Format("Resize to {0} bits", bits)));
            this.bits = bits;
            bitsc = 32 - bits;
            indexmask = (1 << bits) - 1;

            Bucket[] newtable = new Bucket[indexmask + 1];

            for (int i = 0, s = table.Length; i < s; i++)
            {
                Bucket? b = table[i];

                while (b != null)
                {
                    int j = Hv2i(b.hashval);

                    newtable[j] = new Bucket(b.item, b.hashval, newtable[j]);
                    b = b.overflow;
                }

            }

            table = newtable;
            resizethreshhold = (int)(table.Length * fillfactor);
            Logger.Log(string.Format(string.Format("Resize to {0} bits done", bits)));
        }

        /// <summary>
        /// Search for an item equal (according to itemequalityComparer) to the supplied item.  
        /// </summary>
        /// <param name="item"></param>
        /// <param name="add">If true, add item to table if not found.</param>
        /// <param name="update">If true, update table entry if item found.</param>
        /// <param name="raise">If true raise events</param>
        /// <returns>True if found</returns>
        private bool SearchOrAdd(ref T item, bool add, bool update, bool raise)
        {

            int hashval = GetHashCode(item);
            int i = Hv2i(hashval);
            Bucket? b = table[i];
            Bucket? bold = null;

            if (b != null)
            {
                while (b != null)
                {
                    T olditem = b.item;
                    if (Equals(olditem, item))
                    {
                        if (update)
                        {
                            b.item = item;
                        }
                        if (raise && update)
                        {
                            RaiseForUpdate(item, olditem);
                        }
                        // bug20071112:
                        item = olditem;
                        return true;
                    }

                    bold = b;
                    b = b.overflow;
                }

                if (!add)
                {
                    goto notfound;
                }

                bold!.overflow = new Bucket(item, hashval, null);
            }
            else
            {
                if (!add)
                {
                    goto notfound;
                }

                table[i] = new Bucket(item, hashval, null);
            }
            size++;
            if (size > resizethreshhold)
            {
                Expand();
            }

        notfound:
            if (raise && add)
            {
                RaiseForAdd(item);
            }

            if (update)
            {
                item = default;
            }

            return false;
        }


        private bool Remove(ref T item)
        {

            if (size == 0)
            {
                return false;
            }

            int hashval = GetHashCode(item);
            int index = Hv2i(hashval);
            Bucket? b = table[index], bold;

            if (b == null)
            {
                return false;
            }

            if (Equals(item, b.item))
            {
                //ref
                item = b.item;
                table[index] = b.overflow;
            }
            else
            {
                bold = b;
                b = b.overflow;
                while (b != null && !Equals(item, b.item))
                {
                    bold = b;
                    b = b.overflow;
                }

                if (b == null)
                {
                    return false;
                }

                //ref
                item = b.item;
                bold.overflow = b.overflow;
            }
            size--;

            return true;
        }


        private void ClearInner()
        {
            bits = origbits;
            bitsc = 32 - bits;
            indexmask = (1 << bits) - 1;
            size = 0;
            table = new Bucket[indexmask + 1];
            resizethreshhold = (int)(table.Length * fillfactor);
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Create a hash set with natural item equalityComparer and default fill threshold (66%)
        /// and initial table size (16).
        /// </summary>
        public HashSet()
            : this(EqualityComparer<T>.Default) { }


        /// <summary>
        /// Create a hash set with external item equalityComparer and default fill threshold (66%)
        /// and initial table size (16).
        /// </summary>
        /// <param name="itemequalityComparer">The external item equalitySCG.Comparer</param>
        public HashSet(SCG.IEqualityComparer<T> itemequalityComparer)
            : this(16, itemequalityComparer) { }


        /// <summary>
        /// Create a hash set with external item equalityComparer and default fill threshold (66%)
        /// </summary>
        /// <param name="capacity">Initial table size (rounded to power of 2, at least 16)</param>
        /// <param name="itemequalityComparer">The external item equalitySCG.Comparer</param>
        public HashSet(int capacity, SCG.IEqualityComparer<T> itemequalityComparer)
            : this(capacity, 0.66, itemequalityComparer) { }


        /// <summary>
        /// Create a hash set with external item equalityComparer.
        /// </summary>
        /// <param name="capacity">Initial table size (rounded to power of 2, at least 16)</param>
        /// <param name="fill">Fill threshold (in range 10% to 90%)</param>
        /// <param name="itemequalityComparer">The external item equalitySCG.Comparer</param>
        public HashSet(int capacity, double fill, SCG.IEqualityComparer<T> itemequalityComparer)
            : base(itemequalityComparer)
        {
            _randomhashfactor = (Debug.UseDeterministicHashing) ? 1529784659 : (2 * (uint)Random.Next() + 1) * 1529784659;

            if (fill < 0.1 || fill > 0.9)
            {
                throw new ArgumentException("Fill outside valid range [0.1, 0.9]");
            }

            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be non-negative");
            }
            //this.itemequalityComparer = itemequalityComparer;
            origbits = 4;
            while (capacity - 1 >> origbits > 0)
            {
                origbits++;
            }

            ClearInner();
        }



        #endregion

        #region IEditableCollection<T> Members

        /// <summary>
        /// The complexity of the Contains operation
        /// </summary>
        /// <value>Always returns Speed.Constant</value>
        public virtual Speed ContainsSpeed => Speed.Constant;

        /// <summary>
        /// Check if an item is in the set 
        /// </summary>
        /// <param name="item">The item to look for</param>
        /// <returns>True if set contains item</returns>
        public virtual bool Contains(T item) { return SearchOrAdd(ref item, false, false, false); }


        /// <summary>
        /// Check if an item (collection equal to a given one) is in the set and
        /// if so report the actual item object found.
        /// </summary>
        /// <param name="item">On entry, the item to look for.
        /// On exit the item found, if any</param>
        /// <returns>True if set contains item</returns>
        public virtual bool Find(ref T item) { return SearchOrAdd(ref item, false, false, false); }


        /// <summary>
        /// Check if an item (collection equal to a given one) is in the set and
        /// if so replace the item object in the set with the supplied one.
        /// </summary>
        /// <param name="item">The item object to update with</param>
        /// <returns>True if item was found (and updated)</returns>
        public virtual bool Update(T item)
        { UpdateCheck(); return SearchOrAdd(ref item, false, true, true); }

        /// <summary>
        /// Check if an item (collection equal to a given one) is in the set and
        /// if so replace the item object in the set with the supplied one.
        /// </summary>
        /// <param name="item">The item object to update with</param>
        /// <param name="olditem"></param>
        /// <returns>True if item was found (and updated)</returns>
        public virtual bool Update(T item, out T olditem)
        { UpdateCheck(); olditem = item; return SearchOrAdd(ref olditem, false, true, true); }


        /// <summary>
        /// Check if an item (collection equal to a given one) is in the set.
        /// If found, report the actual item object in the set,
        /// else add the supplied one.
        /// </summary>
        /// <param name="item">On entry, the item to look for or add.
        /// On exit the actual object found, if any.</param>
        /// <returns>True if item was found</returns>
        public virtual bool FindOrAdd(ref T item)
        { UpdateCheck(); return SearchOrAdd(ref item, true, false, true); }


        /// <summary>
        /// Check if an item (collection equal to a supplied one) is in the set and
        /// if so replace the item object in the set with the supplied one; else
        /// add the supplied one.
        /// </summary>
        /// <param name="item">The item to look for and update or add</param>
        /// <returns>True if item was updated</returns>
        public virtual bool UpdateOrAdd(T item)
        { UpdateCheck(); return SearchOrAdd(ref item, true, true, true); }


        /// <summary>
        /// Check if an item (collection equal to a supplied one) is in the set and
        /// if so replace the item object in the set with the supplied one; else
        /// add the supplied one.
        /// </summary>
        /// <param name="item">The item to look for and update or add</param>
        /// <param name="olditem"></param>
        /// <returns>True if item was updated</returns>
        public virtual bool UpdateOrAdd(T item, out T olditem)
        { UpdateCheck(); olditem = item; return SearchOrAdd(ref olditem, true, true, true); }


        /// <summary>
        /// Remove an item from the set
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if item was (found and) removed </returns>
        public override bool Remove(T item)
        {
            UpdateCheck();
            if (Remove(ref item))
            {
                RaiseForRemove(item);
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Remove an item from the set, reporting the actual matching item object.
        /// </summary>
        /// <param name="item">The value to remove.</param>
        /// <param name="removeditem">The removed value.</param>
        /// <returns>True if item was found.</returns>
        public virtual bool Remove(T item, out T removeditem)
        {
            UpdateCheck();
            removeditem = item;
            if (Remove(ref removeditem))
            {
                RaiseForRemove(removeditem);
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Remove all items in a supplied collection from this set.
        /// </summary>
        /// <param name="items">The items to remove.</param>
        public virtual void RemoveAll(SCG.IEnumerable<T> items)
        {
            UpdateCheck();
            RaiseForRemoveAllHandler raiseHandler = new RaiseForRemoveAllHandler(this);
            bool raise = raiseHandler.MustFire;
            T jtem;
            foreach (var item in items)
            {
                jtem = item; if (Remove(ref jtem) && raise)
                {
                    raiseHandler.Remove(jtem);
                }
            }

            if (raise)
            {
                raiseHandler.Raise();
            }
        }

        /// <summary>
        /// Remove all items from the set, resetting internal table to initial size.
        /// </summary>
        public override void Clear()
        {
            UpdateCheck();
            int oldsize = size;
            ClearInner();
            if (ActiveEvents != 0 && oldsize > 0)
            {
                RaiseCollectionCleared(true, oldsize);
                RaiseCollectionChanged();
            }
        }


        /// <summary>
        /// Remove all items *not* in a supplied collection from this set.
        /// </summary>
        /// <param name="items">The items to retain</param>
        public virtual void RetainAll(SCG.IEnumerable<T> items)
        {
            UpdateCheck();

            HashSet<T> aux = new HashSet<T>(EqualityComparer);

            //This only works for sets:
            foreach (var item in items)
            {
                if (Contains(item))
                {
                    T jtem = item;

                    aux.SearchOrAdd(ref jtem, true, false, false);
                }
            }

            if (size == aux.size)
            {
                return;
            }

            CircularQueue<T>? wasRemoved = null;
            if ((ActiveEvents & EventType.Removed) != 0)
            {
                wasRemoved = new CircularQueue<T>();
                foreach (T item in this)
                {
                    if (!aux.Contains(item))
                    {
                        wasRemoved.Enqueue(item);
                    }
                }
            }

            table = aux.table;
            size = aux.size;

            indexmask = aux.indexmask;
            resizethreshhold = aux.resizethreshhold;
            bits = aux.bits;
            bitsc = aux.bitsc;

            _randomhashfactor = aux._randomhashfactor;

            if ((ActiveEvents & EventType.Removed) != 0)
            {
                RaiseForRemoveAll(wasRemoved);
            }
            else if ((ActiveEvents & EventType.Changed) != 0)
            {
                RaiseCollectionChanged();
            }
        }

        /// <summary>
        /// Check if all items in a supplied collection is in this set
        /// (ignoring multiplicities). 
        /// </summary>
        /// <param name="items">The items to look for.</param>
        /// <returns>True if all items are found.</returns>
        public virtual bool ContainsAll(SCG.IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (!Contains(item))
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Create an array containing all items in this set (in enumeration order).
        /// </summary>
        /// <returns>The array</returns>
        public override T[] ToArray()
        {
            T[] res = new T[size];
            int index = 0;

            for (int i = 0; i < table.Length; i++)
            {
                Bucket? b = table[i];
                while (b != null)
                {
                    res[index++] = b.item;
                    b = b.overflow;
                }
            }

            System.Diagnostics.Debug.Assert(size == index);
            return res;
        }


        /// <summary>
        /// Count the number of times an item is in this set (either 0 or 1).
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>1 if item is in set, 0 else</returns>
        public virtual int ContainsCount(T item) { return Contains(item) ? 1 : 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionValue<T> UniqueItems() { return this; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ICollectionValue<System.Collections.Generic.KeyValuePair<T, int>> ItemMultiplicities()
        {
            return new MultiplicityOne<T>(this);
        }

        /// <summary>
        /// Remove all (at most 1) copies of item from this set.
        /// </summary>
        /// <param name="item">The item to remove</param>
        public virtual void RemoveAllCopies(T item) { Remove(item); }

        #endregion

        #region IEnumerable<T> Members


        /// <summary>
        /// Choose some item of this collection. 
        /// </summary>
        /// <exception cref="NoSuchItemException">if collection is empty.</exception>
        /// <returns></returns>
        public override T Choose()
        {
            int len = table.Length;
            if (size == 0)
            {
                throw new NoSuchItemException();
            }

            do { if (++lastchosen >= len) { lastchosen = 0; } } while (table[lastchosen] == null);

            return (table[lastchosen])!.item;
        }

        /// <summary>
        /// Create an enumerator for this set.
        /// </summary>
        /// <returns>The enumerator</returns>
        public override SCG.IEnumerator<T> GetEnumerator()
        {
            int index = -1;
            int mystamp = stamp;
            int len = table.Length;

            Bucket? b = null;

            while (true)
            {
                if (mystamp != stamp)
                {
                    throw new CollectionModifiedException();
                }

                if (b == null || b.overflow == null)
                {
                    do
                    {
                        if (++index >= len)
                        {
                            yield break;
                        }
                    } while (table[index] == null);

                    b = table[index];
                    yield return b!.item;
                }
                else
                {
                    b = b.overflow;
                    yield return b.item;
                }
            }
        }

        #endregion

        #region ISink<T> Members
        /// <summary>
        /// Report if this is a set collection.
        /// </summary>
        /// <value>Always false</value>
        public virtual bool AllowsDuplicates => false;

        /// <summary>
        /// By convention this is true for any collection with set semantics.
        /// </summary>
        /// <value>True if only one representative of a group of equal items 
        /// is kept in the collection together with the total count.</value>
        public virtual bool DuplicatesByCounting => true;

        /// <summary>
        /// Add an item to this set.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if item was added (i.e. not found)</returns>
        public override bool Add(T item)
        {
            UpdateCheck();
            return !SearchOrAdd(ref item, true, false, true);
        }

        /// <summary>
        /// Add an item to this set.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void SCG.ICollection<T>.Add(T item) => Add(item);

        /// <summary>
        /// Add the elements from another collection with a more specialized item type 
        /// to this collection. Since this
        /// collection has set semantics, only items not already in the collection
        /// will be added.
        /// </summary>
        /// <param name="items">The items to add</param>
        public virtual void AddAll(SCG.IEnumerable<T> items)
        {
            UpdateCheck();
            bool wasChanged = false;
            bool raiseAdded = (ActiveEvents & EventType.Added) != 0;
            CircularQueue<T>? wasAdded = raiseAdded ? new CircularQueue<T>() : null;
            foreach (T item in items)
            {
                T jtem = item;

                if (!SearchOrAdd(ref jtem, true, false, false))
                {
                    wasChanged = true;
                    if (raiseAdded)
                    {
                        wasAdded?.Enqueue(item);
                    }
                }
            }
            //TODO: implement a RaiseForAddAll() method
            if (raiseAdded & wasChanged)
            {
                if (wasAdded != null)
                {
                    foreach (T item in wasAdded)
                    {
                        RaiseItemsAdded(item, 1);
                    }
                }
            }

            if (((ActiveEvents & EventType.Changed) != 0 && wasChanged))
            {
                RaiseCollectionChanged();
            }
        }


        #endregion

        #region Diagnostics

        /// <summary>
        /// Test internal structure of data (invariants)
        /// </summary>
        /// <returns>True if pass</returns>
        public virtual bool Check()
        {
            int count = 0;
            bool retval = true;

            if (bitsc != 32 - bits)
            {
                Logger.Log(string.Format("bitsc != 32 - bits ({0}, {1})", bitsc, bits));
                retval = false;
            }
            if (indexmask != (1 << bits) - 1)
            {
                Logger.Log(string.Format("indexmask != (1 << bits) - 1 ({0}, {1})", indexmask, bits));
                retval = false;
            }
            if (table.Length != indexmask + 1)
            {
                Logger.Log(string.Format("table.Length != indexmask + 1 ({0}, {1})", table.Length, indexmask));
                retval = false;
            }
            if (bitsc != 32 - bits)
            {
                Logger.Log(string.Format("resizethreshhold != (int)(table.Length * fillfactor) ({0}, {1}, {2})", resizethreshhold, table.Length, fillfactor));
                retval = false;
            }

            for (int i = 0, s = table.Length; i < s; i++)
            {
                int level = 0;
                Bucket? b = table[i];
                while (b != null)
                {
                    if (i != Hv2i(b.hashval))
                    {
                        Logger.Log(string.Format("Bad cell item={0}, hashval={1}, index={2}, level={3}", b.item, b.hashval, i, level));
                        retval = false;
                    }

                    count++;
                    level++;
                    b = b.overflow;
                }
            }

            if (count != size)
            {
                Logger.Log(string.Format("size({0}) != count({1})", size, count));
                retval = false;
            }

            return retval;
        }


        /// <summary>
        /// Produce statistics on distribution of bucket sizes. Current implementation is incomplete.
        /// </summary>
        /// <returns>Histogram data.</returns>
        public ISortedDictionary<int, int> BucketCostDistribution()
        {
            TreeDictionary<int, int> res = new TreeDictionary<int, int>();
            for (int i = 0, s = table.Length; i < s; i++)
            {
                int count = 0;
                Bucket? b = table[i];

                while (b != null)
                {
                    count++;
                    b = b.overflow;
                }
                if (res.Contains(count))
                {
                    res[count]++;
                }
                else
                {
                    res[count] = 1;
                }
            }

            return res;
        }

        #endregion
    }
}
