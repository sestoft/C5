// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: 2006-01-29, 2006-06-26

namespace C5.UserGuideExamples
{
    class MultiDictionary
    {
        static void Main()
        {
            MultiDictionary_MultiDictionary1.TestIt.Run();
            MultiDictionary_MultiDictionary2.TestIt.Run();
        }
    }

    namespace MultiDictionary_MultiDictionary1
    {
        class TestIt
        {
            public static void Run()
            {
                var mdict = new MultiHashDictionary<int, string> {
                    { 2, "to" },
                    { 2, "deux" },
                    { 2, "two" },
                    { 20, "tyve" },
                    { 20, "twenty" }
                };
                Console.WriteLine(mdict);
                Console.WriteLine("mdict.Count is {0}", mdict.Count);
                Console.WriteLine("mdict.Count (keys) is {0}",
                          ((IDictionary<int, ICollection<string>>)mdict).Count);
                Console.WriteLine("mdict[2].Count is {0}", mdict[2].Count);
                mdict.Remove(20, "tyve");
                mdict.Remove(20, "twenty");
                Console.WriteLine(mdict);
                Console.WriteLine("mdict.Count is {0}", mdict.Count);
                ICollection<string> zwei = new HashSet<string> { "zwei" };
                mdict[2] = zwei;
                mdict[-2] = zwei;
                Console.WriteLine(mdict);
                Console.WriteLine("mdict.Count is {0}", mdict.Count);
                zwei.Add("kaksi");
                Console.WriteLine(mdict);
                Console.WriteLine("mdict.Count is {0}", mdict.Count);
                ICollection<string> empty = new HashSet<string>();
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
                Console.WriteLine("------------------------------");

            }
        }

        // Here we implement a multivalued dictionary as a hash dictionary
        // from keys to value collections.  The value collections may have
        // set or bag semantics.

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
                    {
                        if (entry.Value != null)
                        {
                            count += entry.Value.Count;
                        }
                    }

                    return count;
                }
            }

            public override Speed CountSpeed => Speed.Linear;

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
                return base.Find(ref k, out ICollection<V> values) && values != null && !values.IsEmpty;
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
            // Find(K k, out ICollection<V> values)
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

    namespace MultiDictionary_MultiDictionary2
    {
        class TestIt
        {
            public static void Run()
            {
                {
                    var mdict = new MultiHashDictionary<int, string> {
                        { 2, "to" },
                        { 2, "deux" },
                        { 2, "two" },
                        { 20, "tyve" },
                        { 20, "twenty" }
                    };
                    Console.WriteLine(mdict);
                    Console.WriteLine("mdict.Count is {0}", mdict.Count);
                    Console.WriteLine("mdict.Count (keys) is {0}",
                              ((IDictionary<int, ICollection<string>>)mdict).Count);
                    Console.WriteLine("mdict[2].Count is {0}", mdict[2].Count);
                    mdict.Remove(20, "tyve");
                    mdict.Remove(20, "twenty");
                    Console.WriteLine(mdict);
                    Console.WriteLine("mdict.Count is {0}", mdict.Count);
                    ICollection<string> zwei = new HashSet<string> { "zwei" };
                    mdict[2] = zwei;
                    mdict[-2] = zwei;
                    Console.WriteLine(mdict);
                    Console.WriteLine("mdict.Count is {0}", mdict.Count);
                    zwei.Add("kaksi");
                    Console.WriteLine(mdict);
                    Console.WriteLine("mdict.Count is {0}", mdict.Count);
                    ICollection<string> empty = new HashSet<string>();
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
                    Console.WriteLine("------------------------------");
                }
                {
                    MultiHashDictionary<int, string, HashSet<string>> mdict
                      = new()
                      {
                      { 2, "to" },
                      { 2, "deux" },
                      { 2, "two" },
                      { 20, "tyve" },
                      { 20, "twenty" }
                      };
                    Console.WriteLine(mdict);
                    Console.WriteLine("mdict.Count is {0}", mdict.Count);
                    Console.WriteLine("mdict.Count (keys) is {0}",
                              ((IDictionary<int, HashSet<string>>)mdict).Count);
                    Console.WriteLine("mdict[2].Count is {0}", mdict[2].Count);
                    mdict.Remove(20, "tyve");
                    mdict.Remove(20, "twenty");
                    Console.WriteLine(mdict);
                    Console.WriteLine("mdict.Count is {0}", mdict.Count);
                    HashSet<string> zwei = new()
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
                    HashSet<string> empty = new();
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
                    Console.WriteLine("------------------------------");
                }
            }
        }

        // This version of the multidictionary uses event listeners to make
        // the Count operation constant time.

        // The total value count for the multidictionary is cached, and
        // event listeners on the value collections keep this cached count
        // updated.  Event listeners on the dictionary make sure that event
        // listeners are added to and removed from value collections.

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

            // Clearing the multidictionary should remove event listeners

            public override void Clear()
            {
                foreach (ICollection<V> values in Values)
                    if (values != null)
                    {
                        count -= values.Count;
                        values.ItemsAdded -= IncrementCount;
                        values.ItemsRemoved -= DecrementCount;
                        values.CollectionCleared -= ClearedCount;
                    }
                base.Clear();
            }
        }

        // --------------------------------------------------

        // This version of the multidictionary also uses event listeners to
        // make the Count operation constant time.

        // The difference relative to the preceding version is that each
        // value collection must be an instance of some type VS that has an
        // argumentless constructor and that implements ICollection<V>.
        // This provides additional flexibility: The creator of a
        // multidictionary instance can determine the collection class VC
        // used for value collections, instead of having to put up with the
        // choice made by the multidictionary implementation.

        public class MultiHashDictionary<K, V, VC> : HashDictionary<K, VC>
          where VC : ICollection<V>, new()
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
                  (object sender, ItemCountEventArgs<SCG.KeyValuePair<K, VC>> args) =>
                  {
                      VC values = args.Item.Value;
                      if (values != null)
                      {
                          count += values.Count;
                          values.ItemsAdded += IncrementCount;
                          values.ItemsRemoved += DecrementCount;
                          values.CollectionCleared += ClearedCount;
                      }
                  };
                ItemsRemoved +=
                  (object sender, ItemCountEventArgs<SCG.KeyValuePair<K, VC>> args) =>
                  {
                      VC values = args.Item.Value;
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

            public new virtual int Count => count;

            public override Speed CountSpeed => Speed.Constant;

            // Add a (key,value) pair

            public virtual void Add(K k, V v)
            {
                if (!base.Find(ref k, out VC values) || values == null)
                {
                    values = new VC();
                    Add(k, values);
                }
                values.Add(v);
            }

            // Remove a single (key,value) pair, if present; return true if
            // anything was removed, else false

            public virtual bool Remove(K k, V v)
            {
                if (base.Find(ref k, out VC values) && values != null)
                {
                    if (values.Remove(v))
                    {
                        if (values.IsEmpty)
                        {
                            base.Remove(k);
                        }
                        return true;
                    }
                }
                return false;
            }

            // Determine whether key k is associated with a value

            public override bool Contains(K k)
            {
                return Find(ref k, out VC values) && values != null && !values.IsEmpty;
            }

            // Determine whether each key in ks is associated with a value

            public override bool ContainsAll<U>(SCG.IEnumerable<U> ks)
            {
                foreach (K k in ks)
                {
                    if (!Contains(k))
                    {
                        return false;
                    }
                }

                return true;
            }

            // Get or set the value collection associated with key k

            public override VC this[K k]
            {
                get => base.Find(ref k, out VC values) && values != null ? values : new VC();
                set => base[k] = value;
            }

            public override void Clear()
            {
                foreach (VC values in Values)
                {
                    if (values != null)
                    {
                        count -= values.Count;
                        values.ItemsAdded -= IncrementCount;
                        values.ItemsRemoved -= DecrementCount;
                        values.CollectionCleared -= ClearedCount;
                    }
                }

                base.Clear();
            }
        }
    }
}