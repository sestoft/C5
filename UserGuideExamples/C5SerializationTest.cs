using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;

namespace TestCollectionSerialization
{
    static class C5SerializationTest
    {
        static void Main(string[] args)
        {
            // The following appear to serialize/deserialize just fine
            new C5.ArrayList<int> { 1, 2, 3, 4 }.ToFileDCS("C5ArrayList.xml");
            var testC5ArrayList = FromFileDCS<C5.ArrayList<int>>("C5ArrayList.xml");

            new C5.ArrayList<C5.ArrayList<int>> { testC5ArrayList, testC5ArrayList }.ToFileDCS("C5ArrayListList.xml");
            var testC5ArrayListList = FromFileDCS<C5.ArrayList<int>>("C5ArrayList.xml");
            Console.WriteLine("Identity preserved: {0}", Object.ReferenceEquals(testC5ArrayListList[0], testC5ArrayListList[1]));

            new C5.LinkedList<int> { 1, 2, 3, 4 }.ToFileXML("C5LinkedList.xml");
            var testC5LinkedList = FromFileXML<C5.LinkedList<int>>("C5LinkedList.xml");

            // The following only work using the DataContractSerializer SerializationExceptions: 
            new C5.SortedArray<int> { 1, 2, 3, 4 }.ToFileDCS("C5SortedArray.xml");
            var testC5SortedArray = FromFileDCS<C5.SortedArray<int>>("C5SortedArray.xml");
            new C5.HashBag<int> { 1, 2, 3, 4 }.ToFileDCS("C5HashBag.xml");
            var testC5HashBag = FromFileDCS<C5.HashBag<int>>("C5HashBag.xml");
            new C5.HashSet<int> { 1, 2, 3, 4 }.ToFileDCS("C5HashSet.xml");
            var testC5LHashSett = FromFileDCS<C5.HashSet<int>>("C5HashSet.xml");

            // The following System.Collections.Generic.Dictionary serializes...
            new Dictionary<int, float> { { 1, 1f }, { 2, 2f }, { 3, 3f }, { 4, 4f } }.ToFileDCS("Dictionary.xml");
            var testDictionary = FromFileDCS<Dictionary<int, float>>("Dictionary.xml");
            
            // ...but the C5.HashDictionary or C5.TreeDictionary do not.
            //new C5.HashDictionary<int, float> { { 1, 1f }, { 2, 2f }, { 3, 3f }, { 4, 4f } }.ToFileDCS("C5HashDictionary.xml");
            //var testC5HashDictionary = FromFileDCS<C5.HashDictionary<int, float>>("C5HashDictionary.xml");
            //new C5.TreeDictionary<int, float> { { 1, 1f }, { 2, 2f }, { 3, 3f }, { 4, 4f } }.ToFileDCS("C5TreeDictionary.xml");
            //var testC5TreeDictionary = FromFileDCS<C5.TreeDictionary<int, float>>("C5TreeDictionary.xml");
        }

        public static T FromFileXML<T>(String filename)
        {
            using (System.IO.Stream stream = System.IO.File.OpenRead(Path.GetFullPath(filename)))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
            }
        }

        public static void ToFileXML<T>(this T myObject, string filename)
        {
            using (System.IO.Stream stream = System.IO.File.Create(Path.GetFullPath(filename)))
            {new XmlSerializer(typeof(T)).Serialize(stream, myObject);
            }
        }

        public static T FromFileDCS<T>(String filename)
        {
            using (System.IO.Stream stream = System.IO.File.OpenRead(Path.GetFullPath(filename)))
            {
                return (T)new DataContractSerializer(typeof(T)).ReadObject(stream);
            }
        }

        public static void ToFileDCS<T>(this T myObject, string filename)
        {
            using (System.IO.Stream stream = System.IO.File.Create(Path.GetFullPath(filename)))
            {
                new DataContractSerializer(typeof(T)).WriteObject(stream, myObject);
            }
        }
    }
}
