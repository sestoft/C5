using System;

namespace C5
{
    [Serializable]
    internal class MultiplicityOne<K> : MappedCollectionValue<K, KeyValuePair<K, int>>
    {
        public MultiplicityOne(ICollectionValue<K> coll) : base(coll) { }
        public override KeyValuePair<K, int> Map(K k) { return new KeyValuePair<K, int>(k, 1); }
    }
}