// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: 2006-06-17

namespace C5.UserGuideExamples;

internal class ListenableEvents
{
    public static void Main()
    {
        PrintEvents("CircularQueue", new CircularQueue<int>());
        PrintEvents("ArrayList", new ArrayList<int>());
        PrintEvents("LinkedList", new LinkedList<int>());
        PrintEvents("HashedArrayList", new HashedArrayList<int>());
        PrintEvents("HashedLinkedList", new HashedLinkedList<int>());
        PrintEvents("SortedArray", new SortedArray<int>());
        PrintEvents("WrappedArray", new WrappedArray<int>(new int[0]));
        PrintEvents("TreeSet", new TreeSet<int>());
        PrintEvents("TreeBag", new TreeBag<int>());
        PrintEvents("HashSet", new HashSet<int>());
        PrintEvents("HashBag", new HashBag<int>());
        PrintEvents("IntervalHeap", new IntervalHeap<int>());
        PrintEvents("HashDictionary", new HashDictionary<int, int>());
        PrintEvents("TreeDictionary", new TreeDictionary<int, int>());
    }

    public static void PrintEvents<T>(string kind, ICollectionValue<T> coll)
    {
        Console.WriteLine("{0,25} {1}", kind, coll.ListenableEvents);
    }
}
