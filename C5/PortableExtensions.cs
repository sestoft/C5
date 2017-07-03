using System;

#if !NETSTANDARD1_6 && !NET45

namespace C5
{
    internal static class PortableExtensions
    {
        internal static Type GetTypeInfo(this Type type) => type;
    }
}

#endif
