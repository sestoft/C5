// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: anagrams 2004-12-08

namespace C5.UserGuideExamples;

internal class CollectionSanity
{
    public static void Main()
    {
        var col1 = new LinkedList<int>();
        var col2 = new LinkedList<int>();
        var col3 = new LinkedList<int>();
        col1.AddAll([7, 9, 13]);
        col2.AddAll([7, 9, 13]);
        col3.AddAll([9, 7, 13]);

        HashSet<IList<int>> hs1 =
        [
                col1,
                col2,
                col3
            ];
        Console.WriteLine("hs1 is sane: {0}", EqualityComparerSanity<int, IList<int>>(hs1));
    }

    // When colls is a collection of collections, this method checks
    // that all `inner' collections use the exact same equalityComparer.  Note
    // that two equalityComparer objects may be functionally (extensionally)
    // identical, yet be distinct objects.  However, if the equalityComparers
    // were obtained from EqualityComparer<T>.Default, there will be at most one
    // equalityComparer for each type T.
    public static bool EqualityComparerSanity<T, U>(ICollectionValue<U> colls) where U : IExtensible<T>
    {
        SCG.IEqualityComparer<T> equalityComparer = null;
        foreach (IExtensible<T> coll in colls)
        {
            if (equalityComparer == null)
            {
                equalityComparer = coll.EqualityComparer;
            }
            if (equalityComparer != coll.EqualityComparer)
            {
                return false;
            }
        }
        return true;
    }
}
