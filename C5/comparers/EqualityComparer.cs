// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using System.Linq;
using System.Reflection;
using SCG = System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// Utility class for building default generic equality comparers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public static class EqualityComparer<T>
    {
        private static SCG.IEqualityComparer<T> _default;

        readonly static Type SequencedCollectionEqualityComparer = typeof(SequencedCollectionEqualityComparer<,>);

        readonly static Type UnsequencedCollectionEqualityComparer = typeof(UnsequencedCollectionEqualityComparer<,>);

        /// <summary>
        /// A default generic equality comparer for type T. The procedure is as follows:
        /// <list>
        /// <item>If the actual generic argument T implements the generic interface
        /// <see cref="T:C5.ISequenced`1"/> for some value W of its generic parameter T,
        /// the equalityComparer will be <see cref="T:C5.SequencedCollectionEqualityComparer`2"/></item>
        /// <item>If the actual generic argument T implements
        /// <see cref="T:C5.ICollection`1"/> for some value W of its generic parameter T,
        /// the equalityComparer will be <see cref="T:C5.UnsequencedCollectionEqualityComparer`2"/></item>
        /// <item>Otherwise the SCG.EqualityComparer&lt;T&gt;.Default is returned</item>
        /// </list>
        /// </summary>
        /// <value>The comparer</value>
        public static SCG.IEqualityComparer<T> Default
        {
            get
            {
                if (_default != null)
                {
                    return _default;
                }

                var type = typeof(T);
                var interfaces = type.GetTypeInfo().GetInterfaces();

                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(ISequenced<>)))
                {
                    return CreateAndCache(SequencedCollectionEqualityComparer.MakeGenericType(new[] { type, type.GetTypeInfo().GetGenericArguments()[0] }));
                }

                var isequenced = interfaces.FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(ISequenced<>)));
                if (isequenced != null)
                {
                    return CreateAndCache(SequencedCollectionEqualityComparer.MakeGenericType(new[] { type, isequenced.GetTypeInfo().GetGenericArguments()[0] }));
                }

                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(ICollection<>)))
                {
                    return CreateAndCache(UnsequencedCollectionEqualityComparer.MakeGenericType(new[] { type, type.GetTypeInfo().GetGenericArguments()[0] }));
                }

                var icollection = interfaces.FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(ICollection<>)));
                if (icollection != null)
                {
                    return CreateAndCache(UnsequencedCollectionEqualityComparer.MakeGenericType(new[] { type, icollection.GetTypeInfo().GetGenericArguments()[0] }));
                }

                return _default = SCG.EqualityComparer<T>.Default;
            }
        }

        private static SCG.IEqualityComparer<T> CreateAndCache(Type equalityComparertype)
        {
            return _default = (SCG.IEqualityComparer<T>)(equalityComparertype.GetTypeInfo().GetProperty("Default", BindingFlags.Static | BindingFlags.Public).GetValue(null, null));
        }
    }
}
