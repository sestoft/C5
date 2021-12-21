// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using SCG = System.Collections.Generic;

namespace C5
{
    [Serializable]
    internal abstract class MappedDirectedCollectionValue<T, V> : DirectedCollectionValueBase<V>, IDirectedCollectionValue<V>
    {
        private IDirectedCollectionValue<T> directedCollectionValue;

        public abstract V Map(T item);

        protected MappedDirectedCollectionValue(IDirectedCollectionValue<T> directedCollectionValue)
        {
            this.directedCollectionValue = directedCollectionValue;
        }

        public override V Choose() { return Map(directedCollectionValue.Choose()); }

        public override bool IsEmpty => directedCollectionValue.IsEmpty;

        public override int Count => directedCollectionValue.Count;

        public override Speed CountSpeed => directedCollectionValue.CountSpeed;

        public override IDirectedCollectionValue<V> Backwards()
        {
            var ret = (MappedDirectedCollectionValue<T, V>)MemberwiseClone();
            ret.directedCollectionValue = directedCollectionValue.Backwards();
            return ret;
            //If we made this classs non-abstract we could do
            //return new MappedDirectedCollectionValue<T,V>(directedCollectionValue.Backwards());;
        }

        public override SCG.IEnumerator<V> GetEnumerator()
        {
            foreach (var item in directedCollectionValue)
            {
                yield return Map(item);
            }
        }

        public override EnumerationDirection Direction => directedCollectionValue.Direction;

        IDirectedEnumerable<V> IDirectedEnumerable<V>.Backwards()
        {
            return Backwards();
        }
    }
}