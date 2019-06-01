// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using SCG = System.Collections.Generic;
namespace C5
{
    [Serializable]
    internal abstract class MappedDirectedCollectionValue<T, V> : DirectedCollectionValueBase<V>, IDirectedCollectionValue<V>
    {
        private IDirectedCollectionValue<T> directedcollectionvalue;

        public abstract V Map(T item);

        public MappedDirectedCollectionValue(IDirectedCollectionValue<T> directedcollectionvalue)
        {
            this.directedcollectionvalue = directedcollectionvalue;
        }

        public override V Choose() { return Map(directedcollectionvalue.Choose()); }

        public override bool IsEmpty => directedcollectionvalue.IsEmpty;

        public override int Count => directedcollectionvalue.Count;

        public override Speed CountSpeed => directedcollectionvalue.CountSpeed;

        public override IDirectedCollectionValue<V> Backwards()
        {
            MappedDirectedCollectionValue<T, V> retval = (MappedDirectedCollectionValue<T, V>)MemberwiseClone();
            retval.directedcollectionvalue = directedcollectionvalue.Backwards();
            return retval;
            //If we made this classs non-abstract we could do
            //return new MappedDirectedCollectionValue<T,V>(directedcollectionvalue.Backwards());;
        }


        public override SCG.IEnumerator<V> GetEnumerator()
        {
            foreach (T item in directedcollectionvalue)
            {
                yield return Map(item);
            }
        }

        public override EnumerationDirection Direction => directedcollectionvalue.Direction;

        IDirectedEnumerable<V> IDirectedEnumerable<V>.Backwards()
        {
            return Backwards();
        }


    }
}