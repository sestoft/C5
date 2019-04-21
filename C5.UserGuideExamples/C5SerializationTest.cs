// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// Compile and run with 
//  dotnet clean
//  dotnet build ../C5/C5.csproj
//  dotnet build -p:StartupObject=C5.UserGuideExamples.C5SerializationTest
//  dotnet run

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;

namespace C5.UserGuideExamples
{
    static class C5SerializationTest
    {
        static void Main()
        {
            // The following appear to serialize/deserialize just fine
            var arrayList = new ArrayList<int> { 1, 2, 3, 4 };
            arrayList.ToFileDCS("C5ArrayList.xml");
            var testC5ArrayList = FromFileDCS<ArrayList<int>>("C5ArrayList.xml");
            // TODO: Assert content

            var arrayListOfArrayList = new ArrayList<ArrayList<int>> { testC5ArrayList, testC5ArrayList };
            arrayListOfArrayList.ToFileDCS("C5ArrayListList.xml");

            var testC5ArrayListOrArrayList = FromFileDCS<ArrayList<int>>("C5ArrayList.xml");
            Console.WriteLine($"Identity preserved: {ReferenceEquals(arrayListOfArrayList[0], testC5ArrayListOrArrayList[1])}");

            var linkedList = new LinkedList<int> { 1, 2, 3, 4 };
            linkedList.ToFileXML("C5LinkedList.xml");
            _ = FromFileXML<LinkedList<int>>("C5LinkedList.xml");
            // TODO: Assert content

            // The following only work using the DataContractSerializer SerializationExceptions: 
            new SortedArray<int> { 1, 2, 3, 4 }.ToFileDCS("C5SortedArray.xml");
            _ = FromFileDCS<SortedArray<int>>("C5SortedArray.xml");
            new HashBag<int> { 1, 2, 3, 4 }.ToFileDCS("C5HashBag.xml");
            _ = FromFileDCS<HashBag<int>>("C5HashBag.xml");
            new HashSet<int> { 1, 2, 3, 4 }.ToFileDCS("C5HashSet.xml");
            _ = FromFileDCS<HashSet<int>>("C5HashSet.xml");

            // The following System.Collections.Generic.Dictionary serializes...
            new Dictionary<int, float> { { 1, 1f }, { 2, 2f }, { 3, 3f }, { 4, 4f } }.ToFileDCS("Dictionary.xml");
            _ = FromFileDCS<Dictionary<int, float>>("Dictionary.xml");

            // ...but the C5.HashDictionary or C5.TreeDictionary do not.
            new HashDictionary<int, float> { { 1, 1f }, { 2, 2f }, { 3, 3f }, { 4, 4f } }.ToFileDCS("C5HashDictionary.xml");
            _ = FromFileDCS<HashDictionary<int, float>>("C5HashDictionary.xml");
            new TreeDictionary<int, float> { { 1, 1f }, { 2, 2f }, { 3, 3f }, { 4, 4f } }.ToFileDCS("C5TreeDictionary.xml");
            _ = FromFileDCS<TreeDictionary<int, float>>("C5TreeDictionary.xml");
        }

        public static T FromFileXML<T>(string filename)
        {
            using (Stream stream = File.OpenRead(Path.GetFullPath(filename)))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
            }
        }

        public static void ToFileXML<T>(this T myObject, string filename)
        {
            using (Stream stream = File.Create(Path.GetFullPath(filename)))
            {
                new XmlSerializer(typeof(T)).Serialize(stream, myObject);
            }
        }

        public static T FromFileDCS<T>(string filename)
        {
            using (Stream stream = File.OpenRead(Path.GetFullPath(filename)))
            {
                return (T)new DataContractSerializer(typeof(T)).ReadObject(stream);
            }
        }

        public static void ToFileDCS<T>(this T myObject, string filename)
        {
            using (Stream stream = File.Create(Path.GetFullPath(filename)))
            {
                new DataContractSerializer(typeof(T)).WriteObject(stream, myObject);
            }
        }
    }
}
