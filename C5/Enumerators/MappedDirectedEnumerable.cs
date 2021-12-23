// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

namespace C5;

internal abstract class MappedDirectedEnumerable<T, V> : EnumerableBase<V>, IDirectedEnumerable<V>
{
    private IDirectedEnumerable<T> directedenumerable;

    public abstract V Map(T item);

    public MappedDirectedEnumerable(IDirectedEnumerable<T> directedenumerable)
    {
        this.directedenumerable = directedenumerable;
    }

    public IDirectedEnumerable<V> Backwards()
    {
        MappedDirectedEnumerable<T, V> retval = (MappedDirectedEnumerable<T, V>)MemberwiseClone();
        retval.directedenumerable = directedenumerable.Backwards();
        return retval;
        //If we made this classs non-abstract we could do
        //return new MappedDirectedCollectionValue<T,V>(directedcollectionvalue.Backwards());;
    }


    public override SCG.IEnumerator<V> GetEnumerator()
    {
        foreach (T item in directedenumerable)
        {
            yield return Map(item);
        }
    }

    public Direction Direction => directedenumerable.Direction;
}
