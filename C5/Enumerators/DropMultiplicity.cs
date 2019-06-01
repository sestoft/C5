using System;

namespace C5
{
    [Serializable]
    internal class DropMultiplicity<K> : MappedCollectionValue<KeyValuePair<K, int>, K>
    {
        public DropMultiplicity(ICollectionValue<KeyValuePair<K, int>> coll) : base(coll) { }
        public override K Map(KeyValuePair<K, int> kvp) { return kvp.Key; }
    }
}