namespace C5;

internal abstract class MappedDirectedEnumerable<T, V> : EnumerableBase<V>, IDirectedEnumerable<V>
{
    private IDirectedEnumerable<T> _directedEnumerable;

    public abstract V Map(T item);

    public MappedDirectedEnumerable(IDirectedEnumerable<T> directedEnumerable)
    {
        _directedEnumerable = directedEnumerable;
    }

    public IDirectedEnumerable<V> Backwards()
    {
        MappedDirectedEnumerable<T, V> retval = (MappedDirectedEnumerable<T, V>)MemberwiseClone();
        retval._directedEnumerable = _directedEnumerable.Backwards();
        return retval;
        //If we made this classs non-abstract we could do
        //return new MappedDirectedCollectionValue<T,V>(directedcollectionvalue.Backwards());;
    }


    public override System.Collections.Generic.IEnumerator<V> GetEnumerator()
    {
        foreach (T item in _directedEnumerable)
        {
            yield return Map(item);
        }
    }

    public Direction Direction => _directedEnumerable.Direction;
}