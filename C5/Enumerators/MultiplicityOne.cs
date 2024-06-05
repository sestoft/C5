using System;

namespace C5
{
    internal class MultiplicityOne<K> : MappedCollectionValue<K, System.Collections.Generic.KeyValuePair<K, int>>
    {
        public MultiplicityOne(ICollectionValue<K> coll) : base(coll) { }
        public override System.Collections.Generic.KeyValuePair<K, int> Map(K k) { return new System.Collections.Generic.KeyValuePair<K, int>(k, 1); }
    }
}