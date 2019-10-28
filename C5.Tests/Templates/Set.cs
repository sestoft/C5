using NUnit.Framework;
using System;
using System.Collections.Generic;
using SCG = System.Collections.Generic;

namespace C5.Tests.Templates.Set
{
    [TestFixture]
    public abstract class SCG_ISetBase
    {
        private SCG.ISet<string> set;

        [SetUp]
        public void Init()
        {
            set = CreateSet(new SC(), "A", "C", "E");
        }

        [TearDown]
        public void Dispose()
        {
            set = null;
        }

        public abstract ISet<string> CreateSet(IEqualityComparer<string> equalityComparer, params string[] values);

        [Test]
        public void Add()
        {
            Assert.IsTrue(set.Add("Z"));
            Assert.AreEqual(4, set.Count);
            Assert.IsTrue(set.Contains("Z"));
            Assert.IsFalse(set.Add("A"));
        }

        [Test]
        public virtual void ExceptWith()
        {
            set.ExceptWith(new SCG.List<string> { "C", "E", "Z" });
            Assert.AreEqual(1, set.Count);
            Assert.IsTrue(set.Contains("A"));
        }

        [Test]
        public virtual void ExceptWith_SameEqualityComparer()
        {
            set.ExceptWith(new TreeSet<string>(new SC(), new SC()) { "C", "E", "Z" });
            Assert.AreEqual(1, set.Count);
            Assert.IsTrue(set.Contains("A"));
        }

        [Test]
        public virtual void IntersectWith()
        {
            set.IntersectWith(new SCG.List<string> { "C", "E", "Z" });
            Assert.AreEqual(2, set.Count);
            Assert.IsTrue(set.Contains("C"));
            Assert.IsTrue(set.Contains("E"));
        }

        [Test]
        public virtual void IntersectWith_SameEqualityComparer()
        {
            set.IntersectWith(new TreeSet<string>(new SC(), new SC()) { "C", "E", "Z" });
            Assert.AreEqual(2, set.Count);
            Assert.IsTrue(set.Contains("C"));
            Assert.IsTrue(set.Contains("E"));
        }

        [Test]
        public virtual void IsProperSubsetOf()
        {
            Assert.IsFalse(set.IsProperSubsetOf(new SCG.List<string>()));
            Assert.IsFalse(set.IsProperSubsetOf(new SCG.List<string> { "C", "E", "A" }));
            Assert.IsTrue(set.IsProperSubsetOf(new SCG.List<string> { "C", "E", "A", "X" }));
            Assert.IsFalse(set.IsProperSubsetOf(new SCG.List<string> { "C", "Z" }));
            set.Clear();
            Assert.IsTrue(set.IsProperSubsetOf(new SCG.List<string> { "C", "A" }));
        }

        [Test]
        public virtual void IsProperSubsetOf_SameEqualityComparer()
        {
            Assert.IsFalse(set.IsProperSubsetOf(new TreeSet<string>(new SC(), new SC())));
            Assert.IsFalse(set.IsProperSubsetOf(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A" }));
            Assert.IsTrue(set.IsProperSubsetOf(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A", "X" }));
            Assert.IsFalse(set.IsProperSubsetOf(new TreeSet<string>(new SC(), new SC()) { "C", "Z" }));
            set.Clear();
            Assert.IsTrue(set.IsProperSubsetOf(new TreeSet<string>(new SC(), new SC()) { "C", "A" }));
        }

        [Test]
        public virtual void IsProperSupersetOf()
        {
            Assert.IsTrue(set.IsProperSupersetOf(new SCG.List<string>()));
            Assert.IsFalse(set.IsProperSupersetOf(new SCG.List<string> { "C", "E", "A" }));
            Assert.IsTrue(set.IsProperSupersetOf(new SCG.List<string> { "C", "A" }));
            Assert.IsFalse(set.IsProperSupersetOf(new SCG.List<string> { "C", "Z" }));
            set.Clear();
            Assert.IsFalse(set.IsProperSupersetOf(new SCG.List<string> { "C", "A" }));
        }

        [Test]
        public virtual void IsProperSupersetOf_SameEqualityComparer()
        {
            Assert.IsTrue(set.IsProperSupersetOf(new TreeSet<string>(new SC(), new SC())));
            Assert.IsFalse(set.IsProperSupersetOf(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A" }));
            Assert.IsTrue(set.IsProperSupersetOf(new TreeSet<string>(new SC(), new SC()) { "C", "A" }));
            Assert.IsFalse(set.IsProperSupersetOf(new TreeSet<string>(new SC(), new SC()) { "C", "Z" }));
            set.Clear();
            Assert.IsFalse(set.IsProperSupersetOf(new TreeSet<string>(new SC(), new SC()) { "C", "A" }));
        }

        [Test]
        public virtual void IsSubsetOf()
        {
            Assert.IsFalse(set.IsSubsetOf(new SCG.List<string>()));
            Assert.IsTrue(set.IsSubsetOf(new SCG.List<string> { "C", "E", "A" }));
            Assert.IsTrue(set.IsSubsetOf(new SCG.List<string> { "C", "E", "A", "X" }));
            Assert.IsFalse(set.IsSubsetOf(new SCG.List<string> { "C", "Z" }));
            Assert.IsFalse(set.IsSubsetOf(new SCG.List<string> { "C", "A", "Z" }));
            set.Clear();
            Assert.IsTrue(set.IsSubsetOf(new SCG.List<string> { "C", "A" }));
        }

        [Test]
        public virtual void IsSubsetOf_SameEqualityComparer()
        {
            Assert.IsFalse(set.IsSubsetOf(new TreeSet<string>(new SC(), new SC())));
            Assert.IsTrue(set.IsSubsetOf(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A" }));
            Assert.IsTrue(set.IsSubsetOf(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A", "X" }));
            Assert.IsFalse(set.IsSubsetOf(new TreeSet<string>(new SC(), new SC()) { "C", "Z" }));
            Assert.IsFalse(set.IsSubsetOf(new TreeSet<string>(new SC(), new SC()) { "C", "A", "Z" }));
            set.Clear();
            Assert.IsTrue(set.IsSubsetOf(new TreeSet<string>(new SC(), new SC()) { "C", "A" }));
        }

        [Test]
        public virtual void IsSupersetOf()
        {
            Assert.IsTrue(set.IsSupersetOf(new SCG.List<string>()));
            Assert.IsTrue(set.IsSupersetOf(new SCG.List<string> { "C", "E", "A" }));
            Assert.IsFalse(set.IsSupersetOf(new SCG.List<string> { "C", "E", "A", "X" }));
            Assert.IsFalse(set.IsSupersetOf(new SCG.List<string> { "C", "Z" }));
            Assert.IsTrue(set.IsSupersetOf(new SCG.List<string> { "C", "A" }));
            set.Clear();
            Assert.IsFalse(set.IsSupersetOf(new SCG.List<string> { "C", "A" }));
        }

        [Test]
        public virtual void IsSupersetOf_SameEqualityComparer()
        {
            Assert.IsTrue(set.IsSupersetOf(new TreeSet<string>(new SC(), new SC())));
            Assert.IsTrue(set.IsSupersetOf(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A" }));
            Assert.IsFalse(set.IsSupersetOf(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A", "X" }));
            Assert.IsFalse(set.IsSupersetOf(new TreeSet<string>(new SC(), new SC()) { "C", "Z" }));
            Assert.IsTrue(set.IsSupersetOf(new TreeSet<string>(new SC(), new SC()) { "C", "A" }));
            set.Clear();
            Assert.IsFalse(set.IsSupersetOf(new TreeSet<string>(new SC(), new SC()) { "C", "A" }));
        }

        [Test]
        public virtual void Overlaps()
        {
            Assert.IsFalse(set.Overlaps(new SCG.List<string>()));
            Assert.IsTrue(set.Overlaps(new SCG.List<string> { "C", "E", "A" }));
            Assert.IsTrue(set.Overlaps(new SCG.List<string> { "C", "E", "A", "X" }));
            Assert.IsFalse(set.Overlaps(new SCG.List<string> { "X", "Z" }));
            Assert.IsTrue(set.Overlaps(new SCG.List<string> { "C", "A" }));
            set.Clear();
            Assert.IsFalse(set.Overlaps(new SCG.List<string> { "C", "A" }));
        }

        [Test]
        public virtual void Overlaps_SameEqualityComparer()
        {
            Assert.IsFalse(set.Overlaps(new TreeSet<string>(new SC(), new SC())));
            Assert.IsTrue(set.Overlaps(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A" }));
            Assert.IsTrue(set.Overlaps(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A", "X" }));
            Assert.IsFalse(set.Overlaps(new TreeSet<string>(new SC(), new SC()) { "X", "Z" }));
            Assert.IsTrue(set.Overlaps(new TreeSet<string>(new SC(), new SC()) { "C", "A" }));
            set.Clear();
            Assert.IsFalse(set.Overlaps(new TreeSet<string>(new SC(), new SC()) { "C", "A" }));
        }

        [Test]
        public virtual void SetEquals()
        {
            Assert.IsFalse(set.SetEquals(new SCG.List<string>()));
            Assert.IsTrue(set.SetEquals(new SCG.List<string> { "C", "E", "A" }));
            Assert.IsFalse(set.SetEquals(new SCG.List<string> { "C", "E", "A", "X" }));
            Assert.IsFalse(set.SetEquals(new SCG.List<string> { "X", "Z" }));
            Assert.IsFalse(set.SetEquals(new SCG.List<string> { "C", "A" }));
            set.Clear();
            Assert.IsFalse(set.SetEquals(new SCG.List<string> { "C", "A" }));
            Assert.IsTrue(set.SetEquals(new SCG.List<string>()));
        }

        [Test]
        public virtual void SetEquals_SameEqualityComparer()
        {
            Assert.IsFalse(set.SetEquals(new TreeSet<string>(new SC(), new SC())));
            Assert.IsTrue(set.SetEquals(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A" }));
            Assert.IsFalse(set.SetEquals(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A", "X" }));
            Assert.IsFalse(set.SetEquals(new TreeSet<string>(new SC(), new SC()) { "X", "Z" }));
            Assert.IsFalse(set.SetEquals(new TreeSet<string>(new SC(), new SC()) { "C", "A" }));
            set.Clear();
            Assert.IsFalse(set.SetEquals(new TreeSet<string>(new SC(), new SC()) { "C", "A" }));
            Assert.IsTrue(set.SetEquals(new TreeSet<string>(new SC(), new SC())));
        }

        [Test]
        public virtual void SymmetricExceptWith()
        {
            set.SymmetricExceptWith(new SCG.List<string>());
            Assert.AreEqual(3, set.Count);
            set.SymmetricExceptWith(new SCG.List<string> { "C", "E", "R", "X" });
            Assert.AreEqual(3, set.Count);
            Assert.IsTrue(set.SetEquals(new SCG.List<string> { "A", "R", "X" }));
            set.SymmetricExceptWith(new SCG.List<string> { "A", "R", "X" });
            Assert.AreEqual(0, set.Count);

            set.Clear();
            set.SymmetricExceptWith(new SCG.List<string> { "C", "E", "A" });
            Assert.IsTrue(set.SetEquals(new SCG.List<string> { "C", "E", "A" }));
        }

        [Test]
        public virtual void SymmetricExceptWith_SameEqualityComparer()
        {
            set.SymmetricExceptWith(new TreeSet<string>(new SC(), new SC()));
            Assert.AreEqual(3, set.Count);
            set.SymmetricExceptWith(new TreeSet<string>(new SC(), new SC()) { "C", "E", "R", "X" });
            Assert.AreEqual(3, set.Count);
            Assert.IsTrue(set.SetEquals(new SCG.List<string> { "A", "R", "X" }));
            set.SymmetricExceptWith(new TreeSet<string>(new SC(), new SC()) { "A", "R", "X" });
            Assert.AreEqual(0, set.Count);

            set.Clear();
            set.SymmetricExceptWith(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A" });
            Assert.IsTrue(set.SetEquals(new SCG.List<string> { "C", "E", "A" }));
        }

        [Test]
        public virtual void UnionWith()
        {
            set.UnionWith(new SCG.List<string>());
            Assert.AreEqual(3, set.Count);
            set.UnionWith(new SCG.List<string> { "C", "E", "R", "X" });
            Assert.AreEqual(5, set.Count);
            Assert.IsTrue(set.SetEquals(new SCG.List<string> { "A", "C", "E", "R", "X" }));
            set.UnionWith(new SCG.List<string> { "A", "R", "X" });
            Assert.AreEqual(5, set.Count);
            Assert.IsTrue(set.SetEquals(new SCG.List<string> { "A", "C", "E", "R", "X" }));

            set.Clear();
            set.UnionWith(new SCG.List<string> { "C", "E", "A" });
            Assert.IsTrue(set.SetEquals(new SCG.List<string> { "C", "E", "A" }));
        }

        [Test]
        public virtual void UnionWith_SameEqualityComparer()
        {
            set.UnionWith(new TreeSet<string>(new SC(), new SC()));
            Assert.AreEqual(3, set.Count);
            set.UnionWith(new TreeSet<string>(new SC(), new SC()) { "C", "E", "R", "X" });
            Assert.AreEqual(5, set.Count);
            Assert.IsTrue(set.SetEquals(new SCG.List<string> { "A", "C", "E", "R", "X" }));
            set.UnionWith(new TreeSet<string>(new SC(), new SC()) { "A", "R", "X" });
            Assert.AreEqual(5, set.Count);
            Assert.IsTrue(set.SetEquals(new SCG.List<string> { "A", "C", "E", "R", "X" }));

            set.Clear();
            set.UnionWith(new TreeSet<string>(new SC(), new SC()) { "C", "E", "A" });
            Assert.IsTrue(set.SetEquals(new SCG.List<string> { "C", "E", "A" }));
        }

        // ICollection<T> members
        [Test]
        public virtual void Clear()
        {
            Assert.AreEqual(3, set.Count);
            set.Clear();
            Assert.AreEqual(0, set.Count);
        }

        [Test]
        public virtual void Contains()
        {
            Assert.IsTrue(set.Contains("A"));
            Assert.IsFalse(set.Contains("Z"));
        }

        [Test]
        public virtual void CopyTo()
        {
            var values = new string[set.Count + 2];
            set.CopyTo(values, 1);
            Assert.AreEqual(null, values[0]);
            Assert.AreEqual("A", values[1]);
            Assert.AreEqual("C", values[2]);
            Assert.AreEqual("E", values[3]);
            Assert.AreEqual(null, values[4]);
        }

        [Test]
        public virtual void Remove()
        {
            Assert.AreEqual(3, set.Count);
            Assert.IsTrue(set.Remove("A"));
            Assert.AreEqual(2, set.Count);
            Assert.IsFalse(set.Remove("A"));
            Assert.AreEqual(2, set.Count);
        }

        [Test]
        public virtual void Count()
        {
            Assert.AreEqual(3, set.Count);
            set.Add("Foo");
            Assert.AreEqual(4, set.Count);
        }

        [Test]
        public virtual void IsReadOnly()
        {
            Assert.AreEqual(false, set.IsReadOnly);
        }
    }
}
