// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;


namespace C5.Tests.trees.RBDictionary
{
    internal static class Factory
    {
        public static IDictionary<K, V> New<K, V>() { return new TreeDictionary<K, V>(); }
    }

    [TestFixture]
    public class Formatting
    {
        private IDictionary<int, int> coll;
        private IFormatProvider rad16;
        
        [SetUp]
        public void Init() { coll = Factory.New<int, int>(); rad16 = new RadixFormatProvider(16); }
        
        [TearDown]
        public void Dispose() { coll = null; rad16 = null; }
        
        [Test]
        public void Format()
        {
            Assert.AreEqual("[  ]", coll.ToString());
            coll.Add(23, 67); coll.Add(45, 89);
            Assert.AreEqual("[ [23, 67], [45, 89] ]", coll.ToString());
            Assert.AreEqual("[ [17, 43], [2D, 59] ]", coll.ToString(null, rad16));
            Assert.AreEqual("[ [23, 67], ... ]", coll.ToString("L14", null));
            Assert.AreEqual("[ [17, 43], ... ]", coll.ToString("L14", rad16));
        }
    }

    [TestFixture]
    public class RBDict
    {
        private TreeDictionary<string, string> dict;


        [SetUp]
        public void Init() { dict = new TreeDictionary<string, string>(new SC()); }


        [TearDown]
        public void Dispose() { dict = null; }

        [Test]
        public void NullEqualityComparerinConstructor1()
        {
            Assert.Throws<NullReferenceException>(() => new TreeDictionary<int, int>(null));
        }

        [Test]
        public void Choose()
        {
            dict.Add("YES", "NO");
            Assert.AreEqual(new System.Collections.Generic.KeyValuePair<string, string>("YES", "NO"), dict.Choose());
        }

        [Test]
        public void BadChoose()
        {
            Assert.Throws<NoSuchItemException>(() => dict.Choose());
        }

        [Test]
        public void Pred1()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");
            Assert.AreEqual("1", dict.Predecessor("B").Value);
            Assert.AreEqual("1", dict.Predecessor("C").Value);
            Assert.AreEqual("1", dict.WeakPredecessor("B").Value);
            Assert.AreEqual("2", dict.WeakPredecessor("C").Value);
            Assert.AreEqual("2", dict.Successor("B").Value);
            Assert.AreEqual("3", dict.Successor("C").Value);
            Assert.AreEqual("2", dict.WeakSuccessor("B").Value);
            Assert.AreEqual("2", dict.WeakSuccessor("C").Value);
        }

        [Test]
        public void Pred2()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");
            Assert.IsTrue(dict.TryPredecessor("B", out System.Collections.Generic.KeyValuePair<string, string> res));
            Assert.AreEqual("1", res.Value);
            Assert.IsTrue(dict.TryPredecessor("C", out res));
            Assert.AreEqual("1", res.Value);
            Assert.IsTrue(dict.TryWeakPredecessor("B", out res));
            Assert.AreEqual("1", res.Value);
            Assert.IsTrue(dict.TryWeakPredecessor("C", out res));
            Assert.AreEqual("2", res.Value);
            Assert.IsTrue(dict.TrySuccessor("B", out res));
            Assert.AreEqual("2", res.Value);
            Assert.IsTrue(dict.TrySuccessor("C", out res));
            Assert.AreEqual("3", res.Value);
            Assert.IsTrue(dict.TryWeakSuccessor("B", out res));
            Assert.AreEqual("2", res.Value);
            Assert.IsTrue(dict.TryWeakSuccessor("C", out res));
            Assert.AreEqual("2", res.Value);

            Assert.IsFalse(dict.TryPredecessor("A", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);

            Assert.IsFalse(dict.TryWeakPredecessor("@", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);

            Assert.IsFalse(dict.TrySuccessor("E", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);

            Assert.IsFalse(dict.TryWeakSuccessor("F", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);
        }

        [Test]
        public void Initial()
        {
            bool res;
            Assert.IsFalse(dict.IsReadOnly);

            Assert.AreEqual(dict.Count, 0, "new dict should be empty");
            dict.Add("A", "B");
            Assert.AreEqual(dict.Count, 1, "bad count");
            Assert.AreEqual(dict["A"], "B", "Wrong value for dict[A]");
            dict.Add("C", "D");
            Assert.AreEqual(dict.Count, 2, "bad count");
            Assert.AreEqual(dict["A"], "B", "Wrong value");
            Assert.AreEqual(dict["C"], "D", "Wrong value");
            res = dict.Remove("A");
            Assert.IsTrue(res, "bad return value from Remove(A)");
            Assert.IsTrue(dict.Check());
            Assert.AreEqual(dict.Count, 1, "bad count");
            Assert.AreEqual(dict["C"], "D", "Wrong value of dict[C]");
            res = dict.Remove("Z");
            Assert.IsFalse(res, "bad return value from Remove(Z)");
            Assert.AreEqual(dict.Count, 1, "bad count");
            Assert.AreEqual(dict["C"], "D", "Wrong value of dict[C] (2)");
            dict.Clear();
            Assert.AreEqual(dict.Count, 0, "dict should be empty");
        }
        [Test]
        public void Contains()
        {
            dict.Add("C", "D");
            Assert.IsTrue(dict.Contains("C"));
            Assert.IsFalse(dict.Contains("D"));
        }


        [Test]
        public void IllegalAdd()
        {
            dict.Add("A", "B");

            var exception = Assert.Throws<DuplicateNotAllowedException>(() => dict.Add("A", "B"));
            Assert.AreEqual("Key being added: 'A'", exception.Message);
        }


        [Test]
        public void GettingNonExisting()
        {
            Assert.Throws<NoSuchItemException>(() => Console.WriteLine(dict["R"]));
        }


        [Test]
        public void Setter()
        {
            dict["R"] = "UYGUY";
            Assert.AreEqual(dict["R"], "UYGUY");
            dict["R"] = "UIII";
            Assert.AreEqual(dict["R"], "UIII");
            dict["S"] = "VVV";
            Assert.AreEqual(dict["R"], "UIII");
            Assert.AreEqual(dict["S"], "VVV");
            //dict.dump();
        }
    }

    [TestFixture]
    public class GuardedSortedDictionaryTest
    {
        private GuardedSortedDictionary<string, string> dict;

        [SetUp]
        public void Init()
        {
            ISortedDictionary<string, string> dict = new TreeDictionary<string, string>(new SC())
            {
                { "A", "1" },
                { "C", "2" },
                { "E", "3" }
            };
            this.dict = new GuardedSortedDictionary<string, string>(dict);
        }

        [TearDown]
        public void Dispose() { dict = null; }

        [Test]
        public void Pred1()
        {
            Assert.AreEqual("1", dict.Predecessor("B").Value);
            Assert.AreEqual("1", dict.Predecessor("C").Value);
            Assert.AreEqual("1", dict.WeakPredecessor("B").Value);
            Assert.AreEqual("2", dict.WeakPredecessor("C").Value);
            Assert.AreEqual("2", dict.Successor("B").Value);
            Assert.AreEqual("3", dict.Successor("C").Value);
            Assert.AreEqual("2", dict.WeakSuccessor("B").Value);
            Assert.AreEqual("2", dict.WeakSuccessor("C").Value);
        }

        [Test]
        public void Pred2()
        {
            Assert.IsTrue(dict.TryPredecessor("B", out System.Collections.Generic.KeyValuePair<string, string> res));
            Assert.AreEqual("1", res.Value);
            Assert.IsTrue(dict.TryPredecessor("C", out res));
            Assert.AreEqual("1", res.Value);
            Assert.IsTrue(dict.TryWeakPredecessor("B", out res));
            Assert.AreEqual("1", res.Value);
            Assert.IsTrue(dict.TryWeakPredecessor("C", out res));
            Assert.AreEqual("2", res.Value);
            Assert.IsTrue(dict.TrySuccessor("B", out res));
            Assert.AreEqual("2", res.Value);
            Assert.IsTrue(dict.TrySuccessor("C", out res));
            Assert.AreEqual("3", res.Value);
            Assert.IsTrue(dict.TryWeakSuccessor("B", out res));
            Assert.AreEqual("2", res.Value);
            Assert.IsTrue(dict.TryWeakSuccessor("C", out res));
            Assert.AreEqual("2", res.Value);

            Assert.IsFalse(dict.TryPredecessor("A", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);

            Assert.IsFalse(dict.TryWeakPredecessor("@", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);

            Assert.IsFalse(dict.TrySuccessor("E", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);

            Assert.IsFalse(dict.TryWeakSuccessor("F", out res));
            Assert.AreEqual(null, res.Key);
            Assert.AreEqual(null, res.Value);
        }

        [Test]
        public void Initial()
        {
            Assert.IsTrue(dict.IsReadOnly);

            Assert.AreEqual(3, dict.Count);
            Assert.AreEqual("1", dict["A"]);
        }

        [Test]
        public void Contains()
        {
            Assert.IsTrue(dict.Contains("A"));
            Assert.IsFalse(dict.Contains("1"));
        }

        [Test]
        public void IllegalAdd()
        {
            Assert.Throws<ReadOnlyCollectionException>(() => dict.Add("Q", "7"));
        }

        [Test]
        public void IllegalClear()
        {
            Assert.Throws<ReadOnlyCollectionException>(() => dict.Clear());
        }
        [Test]

        public void IllegalSet()
        {
            Assert.Throws<ReadOnlyCollectionException>(() => dict["A"] = "8");
        }

        public void IllegalRemove()
        {
            Assert.Throws<ReadOnlyCollectionException>(() => dict.Remove("A"));
        }

        [Test]
        public void GettingNonExisting()
        {
            Assert.Throws<NoSuchItemException>(() => Console.WriteLine(dict["R"]));
        }
    }

    [TestFixture]
    public class Enumerators
    {
        private TreeDictionary<string, string> dict;

        private SCG.IEnumerator<System.Collections.Generic.KeyValuePair<string, string>> dictenum;


        [SetUp]
        public void Init()
        {
            dict = new TreeDictionary<string, string>(new SC())
            {
                ["S"] = "A",
                ["T"] = "B",
                ["R"] = "C"
            };
            dictenum = dict.GetEnumerator();
        }


        [TearDown]
        public void Dispose()
        {
            dictenum = null;
            dict = null;
        }

        [Test]
        public void KeysEnumerator()
        {
            SCG.IEnumerator<string> keys = dict.Keys.GetEnumerator();
            Assert.AreEqual(3, dict.Keys.Count);
            Assert.IsTrue(keys.MoveNext());
            Assert.AreEqual("R", keys.Current);
            Assert.IsTrue(keys.MoveNext());
            Assert.AreEqual("S", keys.Current);
            Assert.IsTrue(keys.MoveNext());
            Assert.AreEqual("T", keys.Current);
            Assert.IsFalse(keys.MoveNext());
        }

        [Test]
        public void KeysISorted()
        {
            ISorted<string> keys = dict.Keys;
            Assert.IsTrue(keys.IsReadOnly);
            Assert.AreEqual("R", keys.FindMin());
            Assert.AreEqual("T", keys.FindMax());
            Assert.IsTrue(keys.Contains("S"));
            Assert.AreEqual(3, keys.Count);
            // This doesn't hold, maybe because the dict uses a special key comparer?
            // Assert.IsTrue(keys.SequencedEquals(new WrappedArray<string>(new string[] { "R", "S", "T" })));
            Assert.IsTrue(keys.UniqueItems().All(delegate (String s) { return s == "R" || s == "S" || s == "T"; }));
            Assert.IsTrue(keys.All(delegate (String s) { return s == "R" || s == "S" || s == "T"; }));
            Assert.IsFalse(keys.Exists(delegate (String s) { return s != "R" && s != "S" && s != "T"; }));
            Assert.IsTrue(keys.Find(delegate (String s) { return s == "R"; }, out string res));
            Assert.AreEqual("R", res);
            Assert.IsFalse(keys.Find(delegate (String s) { return s == "Q"; }, out res));
            Assert.AreEqual(null, res);
        }

        [Test]
        public void KeysISortedPred()
        {
            ISorted<string> keys = dict.Keys;
            Assert.IsTrue(keys.TryPredecessor("S", out string res));
            Assert.AreEqual("R", res);
            Assert.IsTrue(keys.TryWeakPredecessor("R", out res));
            Assert.AreEqual("R", res);
            Assert.IsTrue(keys.TrySuccessor("S", out res));
            Assert.AreEqual("T", res);
            Assert.IsTrue(keys.TryWeakSuccessor("T", out res));
            Assert.AreEqual("T", res);
            Assert.IsFalse(keys.TryPredecessor("R", out res));
            Assert.AreEqual(null, res);
            Assert.IsFalse(keys.TryWeakPredecessor("P", out res));
            Assert.AreEqual(null, res);
            Assert.IsFalse(keys.TrySuccessor("T", out res));
            Assert.AreEqual(null, res);
            Assert.IsFalse(keys.TryWeakSuccessor("U", out res));
            Assert.AreEqual(null, res);

            Assert.AreEqual("R", keys.Predecessor("S"));
            Assert.AreEqual("R", keys.WeakPredecessor("R"));
            Assert.AreEqual("T", keys.Successor("S"));
            Assert.AreEqual("T", keys.WeakSuccessor("T"));
        }

        [Test]
        public void ValuesEnumerator()
        {
            SCG.IEnumerator<string> values = dict.Values.GetEnumerator();
            Assert.AreEqual(3, dict.Values.Count);
            Assert.IsTrue(values.MoveNext());
            Assert.AreEqual("C", values.Current);
            Assert.IsTrue(values.MoveNext());
            Assert.AreEqual("A", values.Current);
            Assert.IsTrue(values.MoveNext());
            Assert.AreEqual("B", values.Current);
            Assert.IsFalse(values.MoveNext());
        }

        [Test]
        public void Fun()
        {
            Assert.AreEqual("B", dict.Func("T"));
        }


        [Test]
        public void NormalUse()
        {
            Assert.IsTrue(dictenum.MoveNext());
            Assert.AreEqual(dictenum.Current, new System.Collections.Generic.KeyValuePair<string, string>("R", "C"));
            Assert.IsTrue(dictenum.MoveNext());
            Assert.AreEqual(dictenum.Current, new System.Collections.Generic.KeyValuePair<string, string>("S", "A"));
            Assert.IsTrue(dictenum.MoveNext());
            Assert.AreEqual(dictenum.Current, new System.Collections.Generic.KeyValuePair<string, string>("T", "B"));
            Assert.IsFalse(dictenum.MoveNext());
        }
    }


    namespace PathCopyPersistence
    {
        [TestFixture]
        public class Simple
        {
            private TreeDictionary<string, string> dict;

            private TreeDictionary<string, string> snap;


            [SetUp]
            public void Init()
            {
                dict = new TreeDictionary<string, string>(new SC())
                {
                    ["S"] = "A",
                    ["T"] = "B",
                    ["R"] = "C",
                    ["V"] = "G"
                };
                snap = (TreeDictionary<string, string>)dict.Snapshot();
            }


            [Test]
            public void Test()
            {
                dict["SS"] = "D";
                Assert.AreEqual(5, dict.Count);
                Assert.AreEqual(4, snap.Count);
                dict["T"] = "bb";
                Assert.AreEqual(5, dict.Count);
                Assert.AreEqual(4, snap.Count);
                Assert.AreEqual("B", snap["T"]);
                Assert.AreEqual("bb", dict["T"]);
                Assert.IsFalse(dict.IsReadOnly);
                Assert.IsTrue(snap.IsReadOnly);
                //Finally, update of root node:
                _ = (TreeDictionary<string, string>)dict.Snapshot();
                dict["S"] = "abe";
                Assert.AreEqual("abe", dict["S"]);
            }


            [Test]
            public void UpdateSnap()
            {
                Assert.Throws<ReadOnlyCollectionException>(() => snap["Y"] = "J");
            }


            [TearDown]
            public void Dispose()
            {
                dict = null;
                snap = null;
            }
        }
    }

    [TestFixture]
    public class SCGIDictionary
    {
        private SCG.IDictionary<string, string> dict;

        [SetUp]
        public void Init()
        {
            dict = new TreeDictionary<string, string>(new SC())
            {
                { "A", "1" },
                { "C", "2" },
                { "E", "3" }
            };
        }

        [TearDown]
        public void Dispose() { dict = null; }

        [Test]
        public void Add()
        {
            Assert.AreEqual(3, dict.Count);
            dict.Add("S", "4");
            Assert.AreEqual(4, dict.Count);
        }

        [Test]
        public void ContainsKey()
        {
            Assert.IsTrue(dict.ContainsKey("A"));
            Assert.IsFalse(dict.ContainsKey("Z"));
        }

        [Test]
        public void Remove()
        {
            Assert.AreEqual(3, dict.Count);
            Assert.IsTrue(dict.Remove("A"));
            Assert.AreEqual(2, dict.Count);
            Assert.IsFalse(dict.Remove("Z"));
            Assert.AreEqual(2, dict.Count);
        }

        [Test]
        public void TryGetValue()
        {
            Assert.IsTrue(dict.TryGetValue("C", out string value));
            Assert.AreEqual("2", value);
            Assert.IsFalse(dict.TryGetValue("Z", out value));
            Assert.IsNull(value);
        }

        [Test]
        public void GetItem()
        {
            Assert.AreEqual("3", dict["E"]);
            Assert.Throws<NoSuchItemException>(() => { var x = dict["Z"]; });
        }

        [Test]
        public void SetItem()
        {
            dict["C"] = "9";
            Assert.AreEqual("9", dict["C"]);
            dict["Z"] = "5";
            Assert.AreEqual("5", dict["Z"]);
        }

        // ICollection<SCG.KeyValuePair<TKey, TValue>>
        [Test]
        public void Clear()
        {
            dict.Clear();
            Assert.AreEqual(0, dict.Count);
        }

        [Test]
        public void Contains()
        {
            Assert.IsTrue(dict.Contains(new SCG.KeyValuePair<string, string>("C", "2")));
            Assert.IsFalse(dict.Contains(new SCG.KeyValuePair<string, string>("D", "2")));
            Assert.IsFalse(dict.Contains(new SCG.KeyValuePair<string, string>("C", "6")));
        }

        [Test]
        public void CopyTo()
        {
            var pairs = new SCG.KeyValuePair<string, string>[dict.Count];
            dict.CopyTo(pairs, 0);
            Assert.AreEqual("C", pairs[1].Key);
            Assert.AreEqual("2", pairs[1].Value);
            Assert.AreEqual("E", pairs[2].Key);
            Assert.AreEqual("3", pairs[2].Value);
        }

        [Test]
        public void RemovePair()
        {
            Assert.AreEqual(3, dict.Count);
            Assert.IsTrue(dict.Remove(new SCG.KeyValuePair<string, string>("A", "1")));
            Assert.AreEqual(2, dict.Count);
            Assert.IsFalse(dict.Remove(new SCG.KeyValuePair<string, string>("Z", "9")));
            Assert.AreEqual(2, dict.Count);
        }

        [Test]
        public void Count()
        {
            Assert.AreEqual(3, dict.Count);
        }

        [Test]
        public void IsReadOnly()
        {
            Assert.AreEqual(false, dict.IsReadOnly);
        }

        [Test]
        public void GetEnumerable()
        {
            var enumerable = dict.GetEnumerator();
            Assert.IsTrue(enumerable.MoveNext());
            Assert.AreEqual(new SCG.KeyValuePair<string, string>("A", "1"), enumerable.Current);
            Assert.IsTrue(enumerable.MoveNext());
            Assert.AreEqual(new SCG.KeyValuePair<string, string>("C", "2"), enumerable.Current);
            Assert.IsTrue(enumerable.MoveNext());
            Assert.AreEqual(new SCG.KeyValuePair<string, string>("E", "3"), enumerable.Current);
            Assert.IsFalse(enumerable.MoveNext());
        }

        [Test]
        public void Keys_GetEnumerable()
        {
            var enumerable = dict.Keys.GetEnumerator();
            Assert.IsTrue(enumerable.MoveNext());
            Assert.AreEqual("A", enumerable.Current);
            Assert.IsTrue(enumerable.MoveNext());
            Assert.AreEqual("C", enumerable.Current);
            Assert.IsTrue(enumerable.MoveNext());
            Assert.AreEqual("E", enumerable.Current);
            Assert.IsFalse(enumerable.MoveNext());
        }

        [Test]
        public void Keys_Count()
        {
            Assert.AreEqual(3, dict.Keys.Count);
            dict.Remove("C");
            Assert.AreEqual(2, dict.Keys.Count);
        }

        [Test]
        public void Keys_IsReadOnly()
        {
            Assert.AreEqual(true, dict.Keys.IsReadOnly);
        }

        [Test]
        public void Keys_Add()
        {
            Assert.Throws<ReadOnlyCollectionException>(() => dict.Keys.Add("Foo"));
        }

        [Test]
        public void Keys_Clear()
        {
            Assert.Throws<ReadOnlyCollectionException>(() => dict.Keys.Clear());
        }

        [Test]
        public void Keys_Contains()
        {
            Assert.IsTrue(dict.Keys.Contains("A"));
            Assert.IsFalse(dict.Keys.Contains("B"));
        }

        [Test]
        public void Keys_CopyTo()
        {
            var keys = new string[dict.Keys.Count + 2];
            dict.Keys.CopyTo(keys, 1);
            Assert.AreEqual(null, keys[0]);
            Assert.AreEqual("A", keys[1]);
            Assert.AreEqual("C", keys[2]);
            Assert.AreEqual("E", keys[3]);
            Assert.AreEqual(null, keys[4]);
        }

        [Test]
        public void Keys_Remove()
        {
            Assert.Throws<ReadOnlyCollectionException>(() => dict.Keys.Remove("Foo"));
        }

        [Test]
        public void Values_GetEnumerable()
        {
            var enumerable = dict.Values.GetEnumerator();
            Assert.IsTrue(enumerable.MoveNext());
            Assert.AreEqual("1", enumerable.Current);
            Assert.IsTrue(enumerable.MoveNext());
            Assert.AreEqual("2", enumerable.Current);
            Assert.IsTrue(enumerable.MoveNext());
            Assert.AreEqual("3", enumerable.Current);
            Assert.IsFalse(enumerable.MoveNext());
        }

        [Test]
        public void Values_Count()
        {
            Assert.AreEqual(3, dict.Values.Count);
            dict.Remove("C");
            Assert.AreEqual(2, dict.Values.Count);
        }

        [Test]
        public void Values_IsReadOnly()
        {
            Assert.AreEqual(true, dict.Values.IsReadOnly);
        }

        [Test]
        public void Values_Add()
        {
            Assert.Throws<ReadOnlyCollectionException>(() => dict.Values.Add("Foo"));
        }

        [Test]
        public void Values_Clear()
        {
            Assert.Throws<ReadOnlyCollectionException>(() => dict.Values.Clear());
        }

        [Test]
        public void Values_Contains()
        {
            Assert.IsTrue(dict.Values.Contains("1"));
            Assert.IsFalse(dict.Values.Contains("9"));
        }

        [Test]
        public void Values_CopyTo()
        {
            var values = new string[dict.Values.Count + 2];
            dict.Values.CopyTo(values, 1);
            Assert.AreEqual(null, values[0]);
            Assert.AreEqual("1", values[1]);
            Assert.AreEqual("2", values[2]);
            Assert.AreEqual("3", values[3]);
            Assert.AreEqual(null, values[4]);
        }

        [Test]
        public void Values_Remove()
        {
            Assert.Throws<ReadOnlyCollectionException>(() => dict.Values.Remove("1"));
        }
    }
}
