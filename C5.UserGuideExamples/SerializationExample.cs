// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: serialization and deserialization 

// Compile with 
//   csc /r:netstandard.dll /r:C5.dll SerializationExample.cs 

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace C5.UserGuideExamples
{
    class SerializationExample
    {
        private static readonly BinaryFormatter _formatter = new BinaryFormatter();

        public static void ToFile<T>(IExtensible<T> coll, string filename)
        {
            using (var stream = new FileStream(filename, FileMode.OpenOrCreate))
            {
                _formatter.Serialize(stream, coll);
            }
        }

        public static T FromFile<T>(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                return (T)_formatter.Deserialize(stream);
            }
        }

        public static void Main()
        {
            var names = new LinkedList<string>();
            var reagan = "Reagan";
            names.AddAll(new string[] { reagan, reagan, "Bush", "Clinton", "Clinton", "Bush", "Bush" });
            ToFile(names, "prez.bin");
            var namesFromFile = FromFile<IList<string>>("prez.bin");

            foreach (var s in namesFromFile)
            {
                Console.WriteLine(s);
            }

            Console.Write("Deserialization preserves sequenced equality: ");
            Console.WriteLine(names.SequencedEquals(namesFromFile));
            Console.Write("Deserialization reuses the same strings: ");
            Console.WriteLine(ReferenceEquals(reagan, namesFromFile[1]));
            Console.Write("Deserialization preserves sharing: ");
            Console.WriteLine(ReferenceEquals(namesFromFile[0], namesFromFile[1]));
        }
    }
}
