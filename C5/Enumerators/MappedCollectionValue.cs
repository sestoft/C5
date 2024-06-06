// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

internal abstract class MappedCollectionValue<T, V> : CollectionValueBase<V>
{
    private readonly ICollectionValue<T> _collectionValue;

    public abstract V Map(T item);

    protected MappedCollectionValue(ICollectionValue<T> collectionValue)
    {
        _collectionValue = collectionValue;
    }

    public override V Choose() => Map(_collectionValue.Choose());

    public override bool IsEmpty => _collectionValue.IsEmpty;

    public override int Count => _collectionValue.Count;

    public override Speed CountSpeed => _collectionValue.CountSpeed;

    public override System.Collections.Generic.IEnumerator<V> GetEnumerator()
    {
        foreach (var item in _collectionValue)
        {
            yield return Map(item);
        }
    }
}