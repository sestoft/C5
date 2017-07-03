using System;
using System.Collections.Generic;
using System.Reflection;

namespace C5
{
    internal static class PortableExtensions
    {
#if NETSTANDARD1_0
        internal static Type[] GetGenericArguments(this TypeInfo typeInfo) => typeInfo.GenericTypeArguments;
        internal static IEnumerable<Type> GetInterfaces(this TypeInfo typeInfo) => typeInfo.ImplementedInterfaces;
#elif !NET45
        internal static Type GetTypeInfo(this Type type) => type;
#endif
    }
}
