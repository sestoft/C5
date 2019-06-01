// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// Compile with
//   csc /r:netstandard.dll /r:C5.dll MultiCollection.cs

using System;
using SCG = System.Collections.Generic;

namespace C5.UserGuideExamples
{
    internal class BasicCollectionValue<T> : CollectionValueBase<T>, ICollectionValue<T>
    {
        private readonly SCG.IEnumerable<T> _enumerable;
        private readonly Func<T> _chooser;
        //TODO: add delegate for checking validity!

        public BasicCollectionValue(SCG.IEnumerable<T> e, Func<T> chooser, int c)
        {
            _enumerable = e;
            _chooser = chooser;
            Count = c;
        }

        public override int Count { get; }

        public override Speed CountSpeed => Speed.Constant;

        public override bool IsEmpty => Count == 0;

        public override T Choose()
        {
            return _chooser();
        }

        public override SCG.IEnumerator<T> GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }
    }

    internal interface IMultiCollection<K, V>
    {
        SCG.IEqualityComparer<K> KeyEqualityComparer { get; }
        SCG.IEqualityComparer<V> ValueEqualityComparer { get; }
        bool Add(K k, V v);
        bool Remove(K k, V v);
        ICollectionValue<V> this[K k] { get; }
        ICollectionValue<K> Keys { get; }
        SCG.IEnumerable<V> Values { get; }

    }

    internal class BasicMultiCollection<K, V, W, U> : IMultiCollection<K, V>//: IDictionary<K, W>
        where W : ICollection<V>, new()
        where U : IDictionary<K, W>, new()
    {
        private readonly U _dictionary = new U();

        public SCG.IEqualityComparer<K> KeyEqualityComparer => EqualityComparer<K>.Default;

        public SCG.IEqualityComparer<V> ValueEqualityComparer => EqualityComparer<V>.Default; //TODO: depends on W!

        public bool Add(K k, V v)
        {
            if (!_dictionary.Find(ref k, out W w))
            {
                _dictionary.Add(k, w = new W());
            }

            return w.Add(v);
        }

        public bool Remove(K k, V v)
        {
            if (_dictionary.Find(ref k, out W w) && w.Remove(v))
            {
                if (w.Count == 0)
                {
                    _dictionary.Remove(k);
                }

                return true;
            }
            return false;
        }

        public ICollectionValue<V> this[K k] => _dictionary[k];

        public ICollectionValue<K> Keys => _dictionary.Keys;

        public SCG.IEnumerable<V> Values
        {
            get
            {
                foreach (var w in _dictionary.Values)
                {
                    foreach (var v in w)
                    {
                        yield return v;
                    }
                }
            }
        }
    }

    internal class WordIndex : BasicMultiCollection<string, int, HashSet<int>, HashDictionary<string, HashSet<int>>>
    {
    }

    internal class MultiCollectionProgram
    {
        public static void Main()
        {
            var wi = new WordIndex();
            wi.Add("ja", 2);
            wi.Add("nej", 4);
            wi.Add("ja", 7);

            foreach (string s in wi.Keys)
            {
                Console.WriteLine(s + " -->");

                foreach (int line in wi[s])
                {
                    Console.WriteLine(" " + line);
                }
            }
        }
    }
}