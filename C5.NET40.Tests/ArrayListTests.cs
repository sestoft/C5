using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NUnit.Framework;

namespace C5.Tests
{
    [TestFixture]
    public class ArrayListTests
    {
        [Test]
        public void Serialize()
        {
            var source = new ArrayList<int>();
            source.AddAll(new[] { 1, 2, 3, 4, 5 });

            ArrayList<int> target;
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                stream.Position = 0;

                target = (ArrayList<int>) formatter.Deserialize(stream);
            }

            CollectionAssert.AreEqual(source, target);
        }
    }
}
