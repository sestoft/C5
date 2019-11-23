using System;

namespace C5
{
    [Serializable]
    internal class DropMultiplicity<K> : MappedCollectionValue<System.Collections.Generic.KeyValuePair<K, int>, K>
    {
        public DropMultiplicity(ICollectionValue<System.Collections.Generic.KeyValuePair<K, int>> coll) : base(coll) { }
        public override K Map(System.Collections.Generic.KeyValuePair<K, int> kvp) { return kvp.Key; }
    }
}