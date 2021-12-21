// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: ReadOnlyPatterns.cs for pattern chapter

#pragma warning disable IDE0060
namespace C5.UserGuideExamples;

internal class ReadOnlyPatterns
{
    public static void Main(string[] args)
    {
        GuardHashSet<int>();
        GuardTreeSet<int>();
        GuardList<int>();
        GuardHashDictionary<int, int>();
        GuardSortedDictionary<int, int>();
    }

    // Read-only access to a hash-based collection
    private static void GuardHashSet<T>()
    {
        ICollection<T> coll = new HashSet<T>();
        DoWork(new GuardedCollection<T>(coll));
    }

    private static void DoWork<T>(ICollection<T> gcoll)
    {
        // Use gcoll ...
    }

    // Read-only access to an indexed sorted collection
    private static void GuardTreeSet<T>()
    {
        IIndexedSorted<T> coll = new TreeSet<T>();
        DoWork(new GuardedIndexedSorted<T>(coll));
    }

    private static void DoWork<T>(IIndexedSorted<T> gcoll)
    {
        // Use gcoll ...
    }

    // Read-only access to a list
    private static void GuardList<T>()
    {
        IList<T> coll = new ArrayList<T>();
        DoWork(new GuardedList<T>(coll));
    }

    private static void DoWork<T>(IList<T> gcoll)
    {
        // Use gcoll ...
    }

    // Read-only access to a dictionary
    private static void GuardHashDictionary<K, V>()
    {
        IDictionary<K, V> dict = new HashDictionary<K, V>();
        DoWork(new GuardedDictionary<K, V>(dict));
    }

    private static void DoWork<K, V>(IDictionary<K, V> gdict)
    {
        // Use gdict ...
    }

    // Read-only access to a sorted dictionary
    private static void GuardSortedDictionary<K, V>()
    {
        ISortedDictionary<K, V> dict = new TreeDictionary<K, V>();
        DoWork(new GuardedSortedDictionary<K, V>(dict));
    }

    private static void DoWork<K, V>(ISortedDictionary<K, V> gdict)
    {
        // Use gdict ...
    }
}
#pragma warning restore IDE0060
