using C5.Linq;
using NUnit.Framework;

namespace C5.Tests.arrays
{
    [TestFixture]
    public class CollectionCacheTest
    {
        class DummyClass
        {
            public string astring;
            public int anint;
            enum AnEnum { A, B, C };
            AnEnum anEnum;

            public DummyClass(int i)
            {
                anint = i;
            }

        }

        [SetUp]
        public void Init()
        {
            dummy = new ArrayList<DummyClass>();
            for (int i = 0; i < 1000; i++)
            {
                dummy.Add(new DummyClass(i));
            }
        }
        ArrayList<DummyClass> dummy;
        // Use this for initialization

        [Test]
        public void CacheTest()
        {
            int count = 0;
            //            for (int i = 0; i < 10; i++)
            //            {
            var res = (from num in dummy
                       where num.anint < 502 && num.anint > 498
                       //  orderby num.anint descending
                       select num);
            //}

            foreach (var dummyClass in res)
            {
                count++;
            }

            Assert.IsTrue(count == 3);
        }
    }
}
