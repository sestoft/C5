// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using C5;
using SCG = System.Collections.Generic;

namespace MultiCollection
{
  class BasicCollectionValue<T> : CollectionValueBase<T>, ICollectionValue<T>
  {
    SCG.IEnumerable<T> enumerable;
    Func<T> chooser;
    int count;
    //TODO: add delegate for checking validity!

    public BasicCollectionValue(SCG.IEnumerable<T> e, Func<T> chooser, int c) { enumerable = e; count = c; this.chooser = chooser; }

    public override int Count { get { return count; } }

    public override Speed CountSpeed { get { return Speed.Constant; } }

    public override bool IsEmpty { get { return count == 0; } }

    public override T Choose() { return chooser(); }

    public override System.Collections.Generic.IEnumerator<T> GetEnumerator()
    {
      return enumerable.GetEnumerator();
    }
  }

  interface IMultiCollection<K, V>
  {
    SCG.IEqualityComparer<K> KeyEqualityComparer { get;}
    SCG.IEqualityComparer<V> ValueEqualityComparer { get;}
    bool Add(K k, V v);
    bool Remove(K k, V v);
    ICollectionValue<V> this[K k] { get;}
    ICollectionValue<K> Keys { get;}
    SCG.IEnumerable<V> Values { get;}

  }

  class BasicMultiCollection<K, V, W, U> : IMultiCollection<K, V>//: IDictionary<K, W>
  where W : ICollection<V>, new()
      where U : IDictionary<K, W>, new()
  {
    U dict = new U();

    public SCG.IEqualityComparer<K> KeyEqualityComparer { get { return EqualityComparer<K>.Default; } }

    public SCG.IEqualityComparer<V> ValueEqualityComparer { get { return EqualityComparer<V>.Default; } } //TODO: depends on W!

    public bool Add(K k, V v)
    {
            if (!dict.Find(ref k, out W w))
                dict.Add(k, w = new W());
            return w.Add(v);
    }

    public bool Remove(K k, V v)
    {
            if (dict.Find(ref k, out W w) && w.Remove(v))
            {
                if (w.Count == 0)
                    dict.Remove(k);
                return true;
            }
            return false;
    }

    public ICollectionValue<V> this[K k] { get { return dict[k]; } }

    public ICollectionValue<K> Keys { get { return dict.Keys; } }

    public SCG.IEnumerable<V> Values
    {
      get
      {
        foreach (W w in dict.Values)
          foreach (V v in w)
            yield return v;
      }
    }
  }

  class WordIndex : BasicMultiCollection<string,int,HashSet<int>,HashDictionary<string,HashSet<int>>>
  {
  }

  class MyTest
  {
    public static void Main(String[] args)
    {
      WordIndex wi = new WordIndex();
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