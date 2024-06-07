// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using SCG = System.Collections.Generic;

namespace C5;

internal abstract class MappedDirectedCollectionValue<T, V> : DirectedCollectionValueBase<V>, IDirectedCollectionValue<V>
{
    private IDirectedCollectionValue<T> _directedCollectionValue;

    public abstract V Map(T item);

    protected MappedDirectedCollectionValue(IDirectedCollectionValue<T> directedCollectionValue)
    {
        _directedCollectionValue = directedCollectionValue;
    }

    public override V Choose() => Map(_directedCollectionValue.Choose());

    public override bool IsEmpty => _directedCollectionValue.IsEmpty;

    public override int Count => _directedCollectionValue.Count;

    public override Speed CountSpeed => _directedCollectionValue.CountSpeed;

    public override IDirectedCollectionValue<V> Backwards()
    {
        var ret = (MappedDirectedCollectionValue<T, V>)MemberwiseClone();
        ret._directedCollectionValue = _directedCollectionValue.Backwards();
        return ret;
        //If we made this class non-abstract we could do
        //return new MappedDirectedCollectionValue<T,V>(directedCollectionValue.Backwards());;
    }

    public override SCG.IEnumerator<V> GetEnumerator()
    {
        foreach (var item in _directedCollectionValue)
        {
            yield return Map(item);
        }
    }

    public override Direction Direction => _directedCollectionValue.Direction;

    IDirectedEnumerable<V> IDirectedEnumerable<V>.Backwards()
    {
        return Backwards();
    }
}