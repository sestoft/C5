// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

internal class MultiplicityOne<K> : MappedCollectionValue<K, SCG.KeyValuePair<K, int>>
{
    public MultiplicityOne(ICollectionValue<K> coll) : base(coll) { }
    public override SCG.KeyValuePair<K, int> Map(K k) { return new SCG.KeyValuePair<K, int>(k, 1); }
}
