// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: serialization and deserialization 

// Compile with 
//   csc /r:C5.dll SerializationExample.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

using System.IO;                                        // FileStream, Stream
using System.Runtime.Serialization.Formatters.Binary;   // BinaryFormatter

namespace SerializationExample
{
  class MyTest
  {
    private static readonly BinaryFormatter formatter = new BinaryFormatter();

    public static void ToFile<T>(IExtensible<T> coll, String filename) {
      Stream outstream = new FileStream(filename, FileMode.OpenOrCreate);
      formatter.Serialize(outstream, coll);
      outstream.Close();
    }

    public static T FromFile<T>(String filename) {
      Stream instream = new FileStream(filename, FileMode.Open);
      Object obj = formatter.Deserialize(instream);
      instream.Close();
      return (T)obj;
    }

    public static void Main() {
      IList<String> names = new LinkedList<String>();
      String reagan = "Reagan";
      names.AddAll(new String[] { reagan, reagan, "Bush", "Clinton", 
                                  "Clinton", "Bush", "Bush" });
      ToFile(names, "prez.bin");
      IList<String> namesFromFile = FromFile<IList<String>>("prez.bin");

      foreach (String s in namesFromFile) 
        Console.WriteLine(s);

      Console.Write("Deserialization preserves sequenced equality: ");
      Console.WriteLine(names.SequencedEquals(namesFromFile));
      Console.Write("Deserialization reuses the same strings: ");
      Console.WriteLine(Object.ReferenceEquals(reagan, namesFromFile[1]));
      Console.Write("Deserialization preserves sharing: ");
      Console.WriteLine(Object.ReferenceEquals(namesFromFile[0], namesFromFile[1]));
    }
  }
}
