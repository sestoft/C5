using System;

namespace C5
{
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

        public override System.Collections.Generic.IEnumerator<V> GetEnumerator()
        {
            foreach (T item in collectionvalue)
            {
                yield return Map(item);
            }
        }
    }
}