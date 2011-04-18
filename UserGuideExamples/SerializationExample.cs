/*
 Copyright (c) 2003-2008 Niels Kokholm and Peter Sestoft
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
*/

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
