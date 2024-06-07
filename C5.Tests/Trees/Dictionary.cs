// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

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
            Assert.That(coll.ToString(), Is.EqualTo("[  ]"));
            coll.Add(23, 67); coll.Add(45, 89);
            Assert.Multiple(() =>
            {
                Assert.That(coll.ToString(), Is.EqualTo("[ [23, 67], [45, 89] ]"));
                Assert.That(coll.ToString(null, rad16), Is.EqualTo("[ [17, 43], [2D, 59] ]"));
                Assert.That(coll.ToString("L14", null), Is.EqualTo("[ [23, 67], ... ]"));
                Assert.That(coll.ToString("L14", rad16), Is.EqualTo("[ [17, 43], ... ]"));
            });
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
            Assert.That(dict.Choose(), Is.EqualTo(new System.Collections.Generic.KeyValuePair<string, string>("YES", "NO")));
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
            Assert.Multiple(() =>
            {
                Assert.That(dict.Predecessor("B").Value, Is.EqualTo("1"));
                Assert.That(dict.Predecessor("C").Value, Is.EqualTo("1"));
                Assert.That(dict.WeakPredecessor("B").Value, Is.EqualTo("1"));
                Assert.That(dict.WeakPredecessor("C").Value, Is.EqualTo("2"));
                Assert.That(dict.Successor("B").Value, Is.EqualTo("2"));
                Assert.That(dict.Successor("C").Value, Is.EqualTo("3"));
                Assert.That(dict.WeakSuccessor("B").Value, Is.EqualTo("2"));
                Assert.That(dict.WeakSuccessor("C").Value, Is.EqualTo("2"));
            });
        }

        [Test]
        public void Pred2()
        {
            dict.Add("A", "1");
            dict.Add("C", "2");
            dict.Add("E", "3");
            Assert.Multiple(() =>
            {
                Assert.That(dict.TryPredecessor("B", out System.Collections.Generic.KeyValuePair<string, string> res), Is.True);
                Assert.That(res.Value, Is.EqualTo("1"));
                Assert.That(dict.TryPredecessor("C", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("1"));
                Assert.That(dict.TryWeakPredecessor("B", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("1"));
                Assert.That(dict.TryWeakPredecessor("C", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("2"));
                Assert.That(dict.TrySuccessor("B", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("2"));
                Assert.That(dict.TrySuccessor("C", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("3"));
                Assert.That(dict.TryWeakSuccessor("B", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("2"));
                Assert.That(dict.TryWeakSuccessor("C", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("2"));

                Assert.That(dict.TryPredecessor("A", out res), Is.False);
                Assert.That(res.Key, Is.EqualTo(null));
                Assert.That(res.Value, Is.EqualTo(null));

                Assert.That(dict.TryWeakPredecessor("@", out res), Is.False);
                Assert.That(res.Key, Is.EqualTo(null));
                Assert.That(res.Value, Is.EqualTo(null));

                Assert.That(dict.TrySuccessor("E", out res), Is.False);
                Assert.That(res.Key, Is.EqualTo(null));
                Assert.That(res.Value, Is.EqualTo(null));

                Assert.That(dict.TryWeakSuccessor("F", out res), Is.False);
                Assert.That(res.Key, Is.EqualTo(null));
                Assert.That(res.Value, Is.EqualTo(null));
            });
        }

        [Test]
        public void Initial()
        {
            bool res;
            Assert.Multiple(() =>
            {
                Assert.That(dict.IsReadOnly, Is.False);

                Assert.That(dict.Count, Is.EqualTo(0), "new dict should be empty");
            });
            dict.Add("A", "B");
            Assert.Multiple(() =>
            {
                Assert.That(dict.Count, Is.EqualTo(1), "bad count");
                Assert.That(dict["A"], Is.EqualTo("B"), "Wrong value for dict[A]");
            });
            dict.Add("C", "D");
            Assert.Multiple(() =>
            {
                Assert.That(dict.Count, Is.EqualTo(2), "bad count");
                Assert.That(dict["A"], Is.EqualTo("B"), "Wrong value");
                Assert.That(dict["C"], Is.EqualTo("D"), "Wrong value");
            });
            res = dict.Remove("A");
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.True, "bad return value from Remove(A)");
                Assert.That(dict.Check(), Is.True);
                Assert.That(dict.Count, Is.EqualTo(1), "bad count");
                Assert.That(dict["C"], Is.EqualTo("D"), "Wrong value of dict[C]");
            });
            res = dict.Remove("Z");
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.False, "bad return value from Remove(Z)");
                Assert.That(dict.Count, Is.EqualTo(1), "bad count");
                Assert.That(dict["C"], Is.EqualTo("D"), "Wrong value of dict[C] (2)");
            });
            dict.Clear();
            Assert.That(dict.Count, Is.EqualTo(0), "dict should be empty");
        }
        [Test]
        public void Contains()
        {
            dict.Add("C", "D");
            Assert.Multiple(() =>
            {
                Assert.That(dict.Contains("C"), Is.True);
                Assert.That(dict.Contains("D"), Is.False);
            });
        }


        [Test]
        public void IllegalAdd()
        {
            dict.Add("A", "B");

            var exception = Assert.Throws<DuplicateNotAllowedException>(() => dict.Add("A", "B"));
            Assert.That(exception.Message, Is.EqualTo("Key being added: 'A'"));
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
            Assert.That(dict["R"], Is.EqualTo("UYGUY"));
            dict["R"] = "UIII";
            Assert.That(dict["R"], Is.EqualTo("UIII"));
            dict["S"] = "VVV";
            Assert.Multiple(() =>
            {
                Assert.That(dict["R"], Is.EqualTo("UIII"));
                Assert.That(dict["S"], Is.EqualTo("VVV"));
            });
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
            Assert.Multiple(() =>
            {
                Assert.That(dict.Predecessor("B").Value, Is.EqualTo("1"));
                Assert.That(dict.Predecessor("C").Value, Is.EqualTo("1"));
                Assert.That(dict.WeakPredecessor("B").Value, Is.EqualTo("1"));
                Assert.That(dict.WeakPredecessor("C").Value, Is.EqualTo("2"));
                Assert.That(dict.Successor("B").Value, Is.EqualTo("2"));
                Assert.That(dict.Successor("C").Value, Is.EqualTo("3"));
                Assert.That(dict.WeakSuccessor("B").Value, Is.EqualTo("2"));
                Assert.That(dict.WeakSuccessor("C").Value, Is.EqualTo("2"));
            });
        }

        [Test]
        public void Pred2()
        {
            Assert.Multiple(() =>
            {
                Assert.That(dict.TryPredecessor("B", out System.Collections.Generic.KeyValuePair<string, string> res), Is.True);
                Assert.That(res.Value, Is.EqualTo("1"));
                Assert.That(dict.TryPredecessor("C", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("1"));
                Assert.That(dict.TryWeakPredecessor("B", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("1"));
                Assert.That(dict.TryWeakPredecessor("C", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("2"));
                Assert.That(dict.TrySuccessor("B", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("2"));
                Assert.That(dict.TrySuccessor("C", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("3"));
                Assert.That(dict.TryWeakSuccessor("B", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("2"));
                Assert.That(dict.TryWeakSuccessor("C", out res), Is.True);
                Assert.That(res.Value, Is.EqualTo("2"));

                Assert.That(dict.TryPredecessor("A", out res), Is.False);
                Assert.That(res.Key, Is.EqualTo(null));
                Assert.That(res.Value, Is.EqualTo(null));

                Assert.That(dict.TryWeakPredecessor("@", out res), Is.False);
                Assert.That(res.Key, Is.EqualTo(null));
                Assert.That(res.Value, Is.EqualTo(null));

                Assert.That(dict.TrySuccessor("E", out res), Is.False);
                Assert.That(res.Key, Is.EqualTo(null));
                Assert.That(res.Value, Is.EqualTo(null));

                Assert.That(dict.TryWeakSuccessor("F", out res), Is.False);
                Assert.That(res.Key, Is.EqualTo(null));
                Assert.That(res.Value, Is.EqualTo(null));
            });
        }

        [Test]
        public void Initial()
        {
            Assert.Multiple(() =>
            {
                Assert.That(dict.IsReadOnly, Is.True);

                Assert.That(dict, Has.Count.EqualTo(3));
            });
            Assert.That(dict["A"], Is.EqualTo("1"));
        }

        [Test]
        public void Contains()
        {
            Assert.Multiple(() =>
            {
                Assert.That(dict.Contains("A"), Is.True);
                Assert.That(dict.Contains("1"), Is.False);
            });
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

        private SCG.IEnumerator<SCG.KeyValuePair<string, string>> dictEnum;


        [SetUp]
        public void Init()
        {
            dict = new TreeDictionary<string, string>(new SC())
            {
                ["S"] = "A",
                ["T"] = "B",
                ["R"] = "C"
            };
            dictEnum = dict.GetEnumerator();
        }


        [TearDown]
        public void Dispose()
        {
            dictEnum.Dispose();
            dict = null;
        }

        [Test]
        public void KeysEnumerator()
        {
            SCG.IEnumerator<string> keys = dict.Keys.GetEnumerator();
            Assert.Multiple(() =>
            {
                Assert.That(dict.Keys, Has.Count.EqualTo(3));
                Assert.That(keys.MoveNext(), Is.True);
                Assert.That(keys.Current, Is.EqualTo("R"));
            });
            Assert.That(keys.MoveNext(), Is.True);
            Assert.That(keys.Current, Is.EqualTo("S"));
            Assert.That(keys.MoveNext(), Is.True);
            Assert.That(keys.Current, Is.EqualTo("T"));
            Assert.That(keys.MoveNext(), Is.False);
        }

        [Test]
        public void KeysISorted()
        {
            ISorted<string> keys = dict.Keys;
            Assert.Multiple(() =>
            {
                Assert.That(keys.IsReadOnly, Is.True);
                Assert.That(keys.FindMin(), Is.EqualTo("R"));
                Assert.That(keys.FindMax(), Is.EqualTo("T"));
                Assert.That(keys.Contains("S"), Is.True);
                Assert.That(keys, Has.Count.EqualTo(3));
            });
            Assert.Multiple(() =>
            {
                // This doesn't hold, maybe because the dict uses a special key comparer?
                // Assert.IsTrue(keys.SequencedEquals(new WrappedArray<string>(new string[] { "R", "S", "T" })));
                Assert.That(keys.UniqueItems().All(delegate (string s) { return s == "R" || s == "S" || s == "T"; }), Is.True);
                Assert.That(keys.All(delegate (string s) { return s == "R" || s == "S" || s == "T"; }), Is.True);
                Assert.That(keys.Exists(delegate (string s) { return s != "R" && s != "S" && s != "T"; }), Is.False);
                Assert.That(keys.Find(delegate (string s) { return s == "R"; }, out string res), Is.True);
                Assert.That(res, Is.EqualTo("R"));
                Assert.That(keys.Find(delegate (string s) { return s == "Q"; }, out res), Is.False);
                Assert.That(res, Is.EqualTo(null));
            });
        }

        [Test]
        public void KeysISortedPred()
        {
            ISorted<string> keys = dict.Keys;
            Assert.Multiple(() =>
            {
                Assert.That(keys.TryPredecessor("S", out string res), Is.True);
                Assert.That(res, Is.EqualTo("R"));
                Assert.That(keys.TryWeakPredecessor("R", out res), Is.True);
                Assert.That(res, Is.EqualTo("R"));
                Assert.That(keys.TrySuccessor("S", out res), Is.True);
                Assert.That(res, Is.EqualTo("T"));
                Assert.That(keys.TryWeakSuccessor("T", out res), Is.True);
                Assert.That(res, Is.EqualTo("T"));
                Assert.That(keys.TryPredecessor("R", out res), Is.False);
                Assert.That(res, Is.EqualTo(null));
                Assert.That(keys.TryWeakPredecessor("P", out res), Is.False);
                Assert.That(res, Is.EqualTo(null));
                Assert.That(keys.TrySuccessor("T", out res), Is.False);
                Assert.That(res, Is.EqualTo(null));
                Assert.That(keys.TryWeakSuccessor("U", out res), Is.False);
                Assert.That(res, Is.EqualTo(null));

                Assert.That(keys.Predecessor("S"), Is.EqualTo("R"));
                Assert.That(keys.WeakPredecessor("R"), Is.EqualTo("R"));
                Assert.That(keys.Successor("S"), Is.EqualTo("T"));
                Assert.That(keys.WeakSuccessor("T"), Is.EqualTo("T"));
            });
        }

        [Test]
        public void ValuesEnumerator()
        {
            SCG.IEnumerator<string> values = dict.Values.GetEnumerator();
            Assert.Multiple(() =>
            {
                Assert.That(dict.Values, Has.Count.EqualTo(3));
                Assert.That(values.MoveNext(), Is.True);
                Assert.That(values.Current, Is.EqualTo("C"));
            });
            Assert.That(values.MoveNext(), Is.True);
            Assert.That(values.Current, Is.EqualTo("A"));
            Assert.That(values.MoveNext(), Is.True);
            Assert.That(values.Current, Is.EqualTo("B"));
            Assert.That(values.MoveNext(), Is.False);
        }

        [Test]
        public void Fun()
        {
            Assert.That(dict.Func("T"), Is.EqualTo("B"));
        }


        [Test]
        public void NormalUse()
        {
            Assert.Multiple(() =>
            {
                Assert.That(dictEnum.MoveNext(), Is.True);
                Assert.That(new System.Collections.Generic.KeyValuePair<string, string>("R", "C"), Is.EqualTo(dictEnum.Current));
            });
            Assert.That(dictEnum.MoveNext(), Is.True);
            Assert.That(new System.Collections.Generic.KeyValuePair<string, string>("S", "A"), Is.EqualTo(dictEnum.Current));
            Assert.That(dictEnum.MoveNext(), Is.True);
            Assert.That(new System.Collections.Generic.KeyValuePair<string, string>("T", "B"), Is.EqualTo(dictEnum.Current));
            Assert.That(dictEnum.MoveNext(), Is.False);
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
                Assert.Multiple(() =>
                {
                    Assert.That(dict, Has.Count.EqualTo(5));
                    Assert.That(snap, Has.Count.EqualTo(4));
                });
                dict["T"] = "bb";
                Assert.Multiple(() =>
                {
                    Assert.That(dict, Has.Count.EqualTo(5));
                    Assert.That(snap, Has.Count.EqualTo(4));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(snap["T"], Is.EqualTo("B"));
                    Assert.That(dict["T"], Is.EqualTo("bb"));
                    Assert.That(dict.IsReadOnly, Is.False);
                    Assert.That(snap.IsReadOnly, Is.True);
                });
                //Finally, update of root node:
                _ = (TreeDictionary<string, string>)dict.Snapshot();
                dict["S"] = "abe";
                Assert.That(dict["S"], Is.EqualTo("abe"));
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
}
