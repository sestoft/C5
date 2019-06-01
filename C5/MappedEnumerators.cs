// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using SCG = System.Collections.Generic;
namespace C5
{
    [Serializable]
    internal abstract class MappedDirectedCollectionValue<T, V> : DirectedCollectionValueBase<V>, IDirectedCollectionValue<V>
    {
        private IDirectedCollectionValue<T> directedcollectionvalue;

        public abstract V Map(T item);

        public MappedDirectedCollectionValue(IDirectedCollectionValue<T> directedcollectionvalue)
        {
            this.directedcollectionvalue = directedcollectionvalue;
        }

        public override V Choose() { return Map(directedcollectionvalue.Choose()); }

        public override bool IsEmpty => directedcollectionvalue.IsEmpty;

        public override int Count => directedcollectionvalue.Count;

        public override Speed CountSpeed => directedcollectionvalue.CountSpeed;

        public override IDirectedCollectionValue<V> Backwards()
        {
            MappedDirectedCollectionValue<T, V> retval = (MappedDirectedCollectionValue<T, V>)MemberwiseClone();
            retval.directedcollectionvalue = directedcollectionvalue.Backwards();
            return retval;
            //If we made this classs non-abstract we could do
            //return new MappedDirectedCollectionValue<T,V>(directedcollectionvalue.Backwards());;
        }


        public override SCG.IEnumerator<V> GetEnumerator()
        {
            foreach (T item in directedcollectionvalue)
            {
                yield return Map(item);
            }
        }

        public override EnumerationDirection Direction => directedcollectionvalue.Direction;

        IDirectedEnumerable<V> IDirectedEnumerable<V>.Backwards()
        {
            return Backwards();
        }


    }

    [Serializable]
    internal abstract class MappedCollectionValue<T, V> : CollectionValueBase<V>, ICollectionValue<V>
    {
        private readonly ICollectionValue<T> collectionvalue;

        public abstract V Map(T item);

        public MappedCollectionValue(ICollectionValue<T> collectionvalue)
        {
            this.collectionvalue = collectionvalue;
        }

        public override V Choose() { return Map(collectionvalue.Choose()); }

        public override bool IsEmpty => collectionvalue.IsEmpty;

        public override int Count => collectionvalue.Count;

        public override Speed CountSpeed => collectionvalue.CountSpeed;

        public override SCG.IEnumerator<V> GetEnumerator()
        {
            foreach (T item in collectionvalue)
            {
                yield return Map(item);
            }
        }
    }

    [Serializable]
    internal class MultiplicityOne<K> : MappedCollectionValue<K, KeyValuePair<K, int>>
    {
        public MultiplicityOne(ICollectionValue<K> coll) : base(coll) { }
        public override KeyValuePair<K, int> Map(K k) { return new KeyValuePair<K, int>(k, 1); }
    }

    [Serializable]
    internal class DropMultiplicity<K> : MappedCollectionValue<KeyValuePair<K, int>, K>
    {
        public DropMultiplicity(ICollectionValue<KeyValuePair<K, int>> coll) : base(coll) { }
        public override K Map(KeyValuePair<K, int> kvp) { return kvp.Key; }
    }

    [Serializable]
    internal abstract class MappedDirectedEnumerable<T, V> : EnumerableBase<V>, IDirectedEnumerable<V>
    {
        private IDirectedEnumerable<T> directedenumerable;

        public abstract V Map(T item);

        public MappedDirectedEnumerable(IDirectedEnumerable<T> directedenumerable)
        {
            this.directedenumerable = directedenumerable;
        }

        public IDirectedEnumerable<V> Backwards()
        {
            MappedDirectedEnumerable<T, V> retval = (MappedDirectedEnumerable<T, V>)MemberwiseClone();
            retval.directedenumerable = directedenumerable.Backwards();
            return retval;
            //If we made this classs non-abstract we could do
            //return new MappedDirectedCollectionValue<T,V>(directedcollectionvalue.Backwards());;
        }


        public override SCG.IEnumerator<V> GetEnumerator()
        {
            foreach (T item in directedenumerable)
            {
                yield return Map(item);
            }
        }

        public EnumerationDirection Direction => directedenumerable.Direction;
    }
}