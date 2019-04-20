// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: 2006-06-17

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.ListenableEvents
//  dotnet run

using System;

namespace C5.UserGuideExamples
{
    class ListenableEvents
    {
        public static void Main(string[] args)
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
}
