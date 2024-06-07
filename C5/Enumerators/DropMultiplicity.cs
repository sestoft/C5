// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

internal class DropMultiplicity<K> : MappedCollectionValue<System.Collections.Generic.KeyValuePair<K, int>, K>
{
    public DropMultiplicity(ICollectionValue<System.Collections.Generic.KeyValuePair<K, int>> coll) : base(coll) { }
    public override K Map(System.Collections.Generic.KeyValuePair<K, int> kvp) { return kvp.Key; }
}