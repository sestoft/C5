using System;

namespace C5
{
    [Serializable]
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


        public override System.Collections.Generic.IEnumerator<V> GetEnumerator()
        {
            foreach (T item in directedenumerable)
            {
                yield return Map(item);
            }
        }

        public EnumerationDirection Direction => directedenumerable.Direction;
    }
}