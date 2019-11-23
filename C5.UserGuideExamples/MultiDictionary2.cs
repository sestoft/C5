// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: 2006-01-29

// Compile with 
//   csc /r:netstandard.dll /r:C5.dll MultiDictionary2.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    using Inner = HashSet<string>;

    using MultiDictionary2_MultiDictionary3;

    public class MultiDictionary2
    {
        public static void Main()
        {
            MultiHashDictionary<int, string, Inner> mdict = new MultiHashDictionary<int, string, Inner>
            {
                { 2, "to" },
                { 2, "deux" },
                { 2, "two" },
                { 20, "tyve" },
                { 20, "tyve" },
                { 20, "twenty" }
            };
            Console.WriteLine(mdict);
            Console.WriteLine("mdict.Count is {0}", mdict.Count);
            Console.WriteLine("mdict.Count (keys) is {0}",
                              ((IDictionary<int, Inner>)mdict).Count);
            Console.WriteLine("mdict[2].Count is {0}", mdict[2].Count);
            mdict.Remove(20, "tyve");
            mdict.Remove(20, "twenty");
            Console.WriteLine(mdict);
            Console.WriteLine("mdict.Count is {0}", mdict.Count);
            Inner zwei = new Inner
            {
                "zwei"
            };
            mdict[2] = zwei;
            mdict[-2] = zwei;
            Console.WriteLine(mdict);
            Console.WriteLine("mdict.Count is {0}", mdict.Count);
            zwei.Add("kaksi");
            Console.WriteLine(mdict);
            Console.WriteLine("mdict.Count is {0}", mdict.Count);
            Inner empty = new Inner();
            mdict[0] = empty;
            Console.WriteLine(mdict);
            Console.WriteLine("mdict.Count is {0}", mdict.Count);
            Console.WriteLine("mdict contains key 0: {0}", mdict.Contains(0));
            mdict.Remove(-2);
            Console.WriteLine(mdict);
            Console.WriteLine("mdict.Count is {0}", mdict.Count);
            zwei.Remove("kaksi");
            Console.WriteLine(mdict);
            Console.WriteLine("mdict.Count is {0}", mdict.Count);
            zwei.Clear();
            Console.WriteLine(mdict);
            Console.WriteLine("mdict.Count is {0}", mdict.Count);
        }
    }
}

namespace MultiDictionary2_MultiDictionary1
{
    // Here we implement a multivalued dictionary as a hash dictionary
    // from keys to value collections.  The value collections may have set
    // or bag semantics.

    // The value collections are externally modifiable (as in Peter
    // Golde's PowerCollections library), and therefore:
    //
    //  * A value collection associated with a key may be null or
    //  non-empty.  Hence for correct semantics, the Contains(k) method
    //  must check that the value collection associated with a key is
    //  non-null and non-empty.
    //  
    //  * A value collection may be shared between two or more keys.
    //

    // A C5 hash dictionary each of whose keys map to a collection of values.

    public class MultiHashDictionary<K, V> : HashDictionary<K, ICollection<V>>
    {

        // Return total count of values associated with keys.  This basic
        // implementation simply sums over all value collections, and so
        // is a linear-time operation in the total number of values.  

        public new virtual int Count
        {
            get
            {
                int count = 0;
                foreach (System.Collections.Generic.KeyValuePair<K, ICollection<V>> entry in this)
                    if (entry.Value != null)
                        count += entry.Value.Count;
                return count;
            }
        }

        public override Speed CountSpeed
        {
            get { return Speed.Linear; }
        }

        // Add a (key,value) pair

        public virtual void Add(K k, V v)
        {
            if (!base.Find(ref k, out ICollection<V> values) || values == null)
            {
                values = new HashSet<V>();
                Add(k, values);
            }
            values.Add(v);
        }

        // Remove a single (key,value) pair, if present; return true if
        // anything was removed, else false

        public virtual bool Remove(K k, V v)
        {
            if (base.Find(ref k, out ICollection<V> values) && values != null)
            {
                if (values.Remove(v))
                {
                    if (values.IsEmpty)
                        base.Remove(k);
                    return true;
                }
            }
            return false;
        }

        // Determine whether key k is associated with a value

        public override bool Contains(K k)
        {
            return Find(ref k, out ICollection<V> values) && values != null && !values.IsEmpty;
        }

        // Determine whether each key in ks is associated with a value

        public override bool ContainsAll<U>(SCG.IEnumerable<U> ks)
        {
            foreach (K k in ks)
                if (!Contains(k))
                    return false;
            return true;
        }

        // Get or set the value collection associated with key k

        public override ICollection<V> this[K k]
        {
            get
            {
                return base.Find(ref k, out ICollection<V> values) && values != null ? values : new HashSet<V>();
            }
            set
            {
                base[k] = value;
            }
        }

        // Inherited from base class HashDictionary<K,ICollection<V>>:

        // Add(K k, ICollection<V> values) 
        // AddAll(IEnumerable<System.Collections.Generic.KeyValuePair<K,ICollection<V>>> kvs) 
        // Clear
        // Clone
        // Find(ref k k, out ICollection<V> values)
        // Find(ref K k, out ICollection<V> values)
        // FindOrAdd(K k, ref ICollection<V> values) 
        // Remove(K k) 
        // Remove(K k, out ICollection<V> values) 
        // Update(K k, ICollection<V> values)
        // Update(K k, ICollection<V> values, out ICollection<V> oldValues)
        // UpdateOrAdd(K k, ICollection<V> values)
        // UpdateOrAdd(K k, ICollection<V> values, out ICollection<V> oldValues)
    }
}


namespace MultiDictionary2_MultiDictionary2
{
    // Here we implement a multivalued dictionary as a hash dictionary
    // from keys to value collections.  The value collections may have
    // set or bag semantics.  This version uses event listeners to make
    // the Count operation constant time.

    //  * To avoid recomputing the total number of values for the Count
    //  property, one may cache it and then use event listeners on the
    //  value collections to keep the cached count updated.  In turn, this
    //  requires such event listeners on the dictionary also.

    // A C5 hash dictionary each of whose keys map to a collection of values.

    public class MultiHashDictionary<K, V> : HashDictionary<K, ICollection<V>>
    {
        private int count = 0;      // Cached value count, updated by events only

        private void IncrementCount(object sender, ItemCountEventArgs<V> args)
        {
            count += args.Count;
        }

        private void DecrementCount(object sender, ItemCountEventArgs<V> args)
        {
            count -= args.Count;
        }

        private void ClearedCount(object sender, ClearedEventArgs args)
        {
            count -= args.Count;
        }

        public MultiHashDictionary()
        {
            ItemsAdded +=
              delegate (object sender, ItemCountEventArgs<System.Collections.Generic.KeyValuePair<K, ICollection<V>>> args)
              {
                  ICollection<V> values = args.Item.Value;
                  if (values != null)
                  {
                      count += values.Count;
                      values.ItemsAdded += IncrementCount;
                      values.ItemsRemoved += DecrementCount;
                      values.CollectionCleared += ClearedCount;
                  }
              };
            ItemsRemoved +=
              delegate (object sender, ItemCountEventArgs<System.Collections.Generic.KeyValuePair<K, ICollection<V>>> args)
              {
                  ICollection<V> values = args.Item.Value;
                  if (values != null)
                  {
                      count -= values.Count;
                      values.ItemsAdded -= IncrementCount;
                      values.ItemsRemoved -= DecrementCount;
                      values.CollectionCleared -= ClearedCount;
                  }
              };
        }

        // Return total count of values associated with keys.  

        public new virtual int Count
        {
            get
            {
                return count;
            }
        }

        public override Speed CountSpeed
        {
            get { return Speed.Constant; }
        }

        // Add a (key,value) pair

        public virtual void Add(K k, V v)
        {
            if (!base.Find(ref k, out ICollection<V> values) || values == null)
            {
                values = new HashSet<V>();
                Add(k, values);
            }
            values.Add(v);
        }

        // Remove a single (key,value) pair, if present; return true if
        // anything was removed, else false

        public virtual bool Remove(K k, V v)
        {
            if (base.Find(ref k, out ICollection<V> values) && values != null)
            {
                if (values.Remove(v))
                {
                    if (values.IsEmpty)
                        base.Remove(k);
                    return true;
                }
            }
            return false;
        }

        // Determine whether key k is associated with a value

        public override bool Contains(K k)
        {
            return Find(ref k, out ICollection<V> values) && values != null && !values.IsEmpty;
        }

        // Determine whether each key in ks is associated with a value

        public override bool ContainsAll<U>(SCG.IEnumerable<U> ks)
        {
            foreach (K k in ks)
                if (!Contains(k))
                    return false;
            return true;
        }

        // Get or set the value collection associated with key k

        public override ICollection<V> this[K k]
        {
            get
            {
                return base.Find(ref k, out ICollection<V> values) && values != null ? values : new HashSet<V>();
            }
            set
            {
                base[k] = value;
            }
        }
    }
}


namespace MultiDictionary2_MultiDictionary3
{
    // Here we implement a multivalued dictionary as a hash dictionary
    // from keys to value collections.  The value collections may have
    // set or bag semantics.  This version uses event listeners to make
    // the Count operation constant time.

    //  * To avoid recomputing the total number of values for the Count
    //  property, one may cache it and then use event listeners on the
    //  value collections to keep the cached count updated.  In turn, this
    //  requires such event listeners on the dictionary also.

    // A C5 hash dictionary each of whose keys map to a collection of values.

    public class MultiHashDictionary<K, V, W> : HashDictionary<K, W> where W : ICollection<V>, new()
    {
        private int count = 0;      // Cached value count, updated by events only

        private void IncrementCount(object sender, ItemCountEventArgs<V> args)
        {
            count += args.Count;
        }

        private void DecrementCount(object sender, ItemCountEventArgs<V> args)
        {
            count -= args.Count;
        }

        private void ClearedCount(object sender, ClearedEventArgs args)
        {
            count -= args.Count;
        }

        public MultiHashDictionary()
        {
            ItemsAdded +=
              (object sender, ItemCountEventArgs<SCG.KeyValuePair<K, W>> args) =>
              {
                  ICollection<V> values = args.Item.Value;
                  if (values != null)
                  {
                      count += values.Count;
                      values.ItemsAdded += IncrementCount;
                      values.ItemsRemoved += DecrementCount;
                      values.CollectionCleared += ClearedCount;
                  }
              };
            ItemsRemoved +=
              (object sender, ItemCountEventArgs<SCG.KeyValuePair<K, W>> args) =>
              {
                  ICollection<V> values = args.Item.Value;
                  if (values != null)
                  {
                      count -= values.Count;
                      values.ItemsAdded -= IncrementCount;
                      values.ItemsRemoved -= DecrementCount;
                      values.CollectionCleared -= ClearedCount;
                  }
              };
        }

        // Return total count of values associated with keys.  

        public new virtual int Count
        {
            get
            {
                return count;
            }
        }

        public override Speed CountSpeed
        {
            get { return Speed.Constant; }
        }

        // Add a (key,value) pair

        public virtual void Add(K k, V v)
        {
            if (!base.Find(ref k, out W values) || values == null)
            {
                values = new W();
                Add(k, values);
            }
            values.Add(v);
        }

        // Remove a single (key,value) pair, if present; return true if
        // anything was removed, else false

        public virtual bool Remove(K k, V v)
        {
            if (base.Find(ref k, out W values) && values != null)
            {
                if (values.Remove(v))
                {
                    if (values.IsEmpty)
                        base.Remove(k);
                    return true;
                }
            }
            return false;
        }

        // Determine whether key k is associated with a value

        public override bool Contains(K k)
        {
            return Find(ref k, out W values) && values != null && !values.IsEmpty;
        }

        // Determine whether each key in ks is associated with a value

        public override bool ContainsAll<U>(SCG.IEnumerable<U> ks)
        {
            foreach (K k in ks)
                if (!Contains(k))
                    return false;
            return true;
        }

        // Get or set the value collection associated with key k

        public override W this[K k]
        {
            get
            {
                return base.Find(ref k, out W values) && values != null ? values : new W();
            }
            set
            {
                base[k] = value;
            }
        }
    }
}
