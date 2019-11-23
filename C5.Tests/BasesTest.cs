// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;


namespace C5.Tests.support
{
    namespace bases
    {
        [TestFixture]
        public class ArrayBaseTest
        {
            private class ABT : ArrayBase<string>
            {
                public ABT() : base(8, C5.EqualityComparer<string>.Default) { }

                public override string Choose() { if (size > 0) { return array[0]; } throw new NoSuchItemException(); }

                public string this[int i] { get => array[i]; set => array[i] = value; }


                public int thesize { get => size; set => size = value; }
            }


            [Test]
            public void Check()
            {
                ABT abt = new ABT
                {
                    thesize = 3
                };
                abt[2] = "aaa";
                // Assert.IsFalse(abt.Check());
                abt[0] = "##";
                abt[1] = "##";
                Assert.IsTrue(abt.Check());
            }
        }
    }

    namespace itemops
    {
        [TestFixture]
        public class Comparers
        {
            private class dbl : IComparable<dbl>
            {
                private readonly double d;

                public dbl(double din) { d = din; }

                public int CompareTo(dbl that)
                {
                    return d < that.d ? -1 : d == that.d ? 0 : 1;
                }
                public bool Equals(dbl that) { return d == that.d; }
            }

            [Test]
            public void GenericC()
            {
                SCG.IComparer<dbl> h = SCG.Comparer<dbl>.Default;
                dbl s = new dbl(3.4);
                dbl t = new dbl(3.4);
                dbl u = new dbl(7.4);

                Assert.AreEqual(0, h.Compare(s, t));
                Assert.IsTrue(h.Compare(s, u) < 0);
            }


            [Test]
            public void OrdinaryC()
            {
                SCG.IComparer<string> h = SCG.Comparer<string>.Default;
                string s = "bamse";
                string t = "bamse";
                string u = "bimse";

                Assert.AreEqual(0, h.Compare(s, t));
                Assert.IsTrue(h.Compare(s, u) < 0);
            }


            [Test]
            public void GenericCViaBuilder()
            {
                SCG.IComparer<dbl> h = SCG.Comparer<dbl>.Default;
                dbl s = new dbl(3.4);
                dbl t = new dbl(3.4);
                dbl u = new dbl(7.4);

                Assert.AreEqual(0, h.Compare(s, t));
                Assert.IsTrue(h.Compare(s, u) < 0);
                Assert.AreSame(h, SCG.Comparer<dbl>.Default);
            }


            [Test]
            public void OrdinaryCViaBuilder()
            {
                SCG.IComparer<string> h = SCG.Comparer<string>.Default;
                string s = "bamse";
                string t = "bamse";
                string u = "bimse";

                Assert.AreEqual(0, h.Compare(s, t));
                Assert.IsTrue(h.Compare(s, u) < 0);
                Assert.AreSame(h, SCG.Comparer<string>.Default);

            }

            public void ComparerViaBuilderTest<T>(T item1, T item2)
                where T : IComparable<T>
            {
                SCG.IComparer<T> h = SCG.Comparer<T>.Default;
                Assert.AreSame(h, SCG.Comparer<T>.Default);
                Assert.AreEqual(0, h.Compare(item1, item1));
                Assert.AreEqual(0, h.Compare(item2, item2));
                Assert.IsTrue(h.Compare(item1, item2) < 0);
                Assert.IsTrue(h.Compare(item2, item1) > 0);
                Assert.AreEqual(Math.Sign(item1.CompareTo(item2)), Math.Sign(h.Compare(item1, item2)));
                Assert.AreEqual(Math.Sign(item2.CompareTo(item1)), Math.Sign(h.Compare(item2, item1)));
            }

            [Test]
            public void PrimitiveComparersViaBuilder()
            {
                ComparerViaBuilderTest<char>('A', 'a');
                ComparerViaBuilderTest<sbyte>(-122, 126);
                ComparerViaBuilderTest<byte>(122, 126);
                ComparerViaBuilderTest<short>(-30000, 3);
                ComparerViaBuilderTest<ushort>(3, 50000);
                ComparerViaBuilderTest<int>(-10000000, 10000);
                ComparerViaBuilderTest<uint>(10000000, 3000000000);
                ComparerViaBuilderTest<long>(-1000000000000, 10000000);
                ComparerViaBuilderTest<ulong>(10000000000000UL, 10000000000004UL);
                ComparerViaBuilderTest<float>(-0.001F, 0.00001F);
                ComparerViaBuilderTest<double>(-0.001, 0.00001E-200);
                ComparerViaBuilderTest<decimal>(-20.001M, 19.999M);
            }

            // This test is obsoleted by the one above, but we keep it for good measure
            [Test]
            public void IntComparerViaBuilder()
            {
                SCG.IComparer<int> h = SCG.Comparer<int>.Default;
                int s = 4;
                int t = 4;
                int u = 5;

                Assert.AreEqual(0, h.Compare(s, t));
                Assert.IsTrue(h.Compare(s, u) < 0);
                Assert.AreSame(h, SCG.Comparer<int>.Default);
            }

            [Test]
            public void Nulls()
            {
                Assert.IsTrue(SCG.Comparer<string>.Default.Compare(null, "abe") < 0);
                Assert.IsTrue(SCG.Comparer<string>.Default.Compare(null, null) == 0);
                Assert.IsTrue(SCG.Comparer<string>.Default.Compare("abe", null) > 0);
            }
        }

        [TestFixture]
        public class EqualityComparers
        {
            [Test]
            public void ReftypeequalityComparer()
            {
                SCG.IEqualityComparer<string> h = C5.EqualityComparer<string>.Default;
                string s = "bamse";
                string t = "bamse";
                string u = "bimse";

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
            }


            [Test]
            public void ValuetypeequalityComparer()
            {
                SCG.IEqualityComparer<double> h = C5.EqualityComparer<double>.Default;
                double s = 3.4;
                double t = 3.4;
                double u = 5.7;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
            }

            [Test]
            public void ReftypeequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<string> h = C5.EqualityComparer<string>.Default;
                string s = "bamse";
                string t = "bamse";
                string u = "bimse";

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<string>.Default);
            }


            [Test]
            public void ValuetypeequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<double> h = C5.EqualityComparer<double>.Default;
                double s = 3.4;
                double t = 3.4;
                double u = 5.7;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<double>.Default);
            }

            [Test]
            public void CharequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<char> h = C5.EqualityComparer<char>.Default;
                char s = 'å';
                char t = 'å';
                char u = 'r';

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<char>.Default);
            }

            [Test]
            public void SbyteequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<sbyte> h = C5.EqualityComparer<sbyte>.Default;
                sbyte s = 3;
                sbyte t = 3;
                sbyte u = -5;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<sbyte>.Default);
            }

            [Test]
            public void ByteequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<byte> h = C5.EqualityComparer<byte>.Default;
                byte s = 3;
                byte t = 3;
                byte u = 5;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<byte>.Default);
            }

            [Test]
            public void ShortequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<short> h = C5.EqualityComparer<short>.Default;
                short s = 3;
                short t = 3;
                short u = -5;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<short>.Default);
            }

            [Test]
            public void UshortequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<ushort> h = C5.EqualityComparer<ushort>.Default;
                ushort s = 3;
                ushort t = 3;
                ushort u = 60000;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<ushort>.Default);
            }

            [Test]
            public void IntequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<int> h = C5.EqualityComparer<int>.Default;
                int s = 3;
                int t = 3;
                int u = -5;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<int>.Default);
            }

            [Test]
            public void UintequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<uint> h = C5.EqualityComparer<uint>.Default;
                uint s = 3;
                uint t = 3;
                uint u = 3000000000;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<uint>.Default);
            }

            [Test]
            public void LongequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<long> h = C5.EqualityComparer<long>.Default;
                long s = 3;
                long t = 3;
                long u = -500000000000000L;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<long>.Default);
            }

            [Test]
            public void UlongequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<ulong> h = C5.EqualityComparer<ulong>.Default;
                ulong s = 3;
                ulong t = 3;
                ulong u = 500000000000000UL;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<ulong>.Default);
            }

            [Test]
            public void FloatequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<float> h = C5.EqualityComparer<float>.Default;
                float s = 3.1F;
                float t = 3.1F;
                float u = -5.2F;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<float>.Default);
            }

            [Test]
            public void DoubleequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<double> h = C5.EqualityComparer<double>.Default;
                double s = 3.12345;
                double t = 3.12345;
                double u = -5.2;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<double>.Default);
            }

            [Test]
            public void DecimalequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<decimal> h = C5.EqualityComparer<decimal>.Default;
                decimal s = 3.0001M;
                decimal t = 3.0001M;
                decimal u = -500000000000000M;

                Assert.AreEqual(s.GetHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<decimal>.Default);
            }

            [Test]
            public void UnseqequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<C5.ICollection<int>> h = C5.EqualityComparer<C5.ICollection<int>>.Default;
                C5.ICollection<int> s = new LinkedList<int>();
                C5.ICollection<int> t = new LinkedList<int>();
                C5.ICollection<int> u = new LinkedList<int>();
                s.Add(1); s.Add(2); s.Add(3);
                t.Add(3); t.Add(2); t.Add(1);
                u.Add(3); u.Add(2); u.Add(4);
                Assert.AreEqual(s.GetUnsequencedHashCode(), h.GetHashCode(s));
                Assert.IsTrue(h.Equals(s, t));
                Assert.IsFalse(h.Equals(s, u));
                Assert.AreSame(h, C5.EqualityComparer<C5.ICollection<int>>.Default);
            }

            [Test]
            public void SeqequalityComparerViaBuilder2()
            {
                SCG.IEqualityComparer<LinkedList<int>> h = C5.EqualityComparer<LinkedList<int>>.Default;
                LinkedList<int> s = new LinkedList<int>() { 1, 2, 3 };
                Assert.AreEqual(CHC.SequencedHashCode(1, 2, 3), h.GetHashCode(s));
            }

            [Test]
            public void UnseqequalityComparerViaBuilder2()
            {
                SCG.IEqualityComparer<C5.HashSet<int>> h = C5.EqualityComparer<C5.HashSet<int>>.Default;
                C5.HashSet<int> s = new C5.HashSet<int>() { 1, 2, 3 };
                Assert.AreEqual(CHC.UnsequencedHashCode(1, 2, 3), h.GetHashCode(s));
            }

            //generic types implementing collection interfaces
            [Test]
            public void SeqequalityComparerViaBuilder3()
            {
                SCG.IEqualityComparer<C5.IList<int>> h = C5.EqualityComparer<C5.IList<int>>.Default;
                C5.IList<int> s = new LinkedList<int>() { 1, 2, 3 };
                Assert.AreEqual(CHC.SequencedHashCode(1, 2, 3), h.GetHashCode(s));
            }

            private interface IFoo<T> : C5.ICollection<T> { void Bamse(); }

            private class Foo<T> : C5.HashSet<T>, IFoo<T>
            {
                internal Foo() : base() { }
                public void Bamse() { }
            }

            [Test]
            public void UnseqequalityComparerViaBuilder3()
            {
                SCG.IEqualityComparer<IFoo<int>> h = C5.EqualityComparer<IFoo<int>>.Default;
                IFoo<int> s = new Foo<int>() { 1, 2, 3 };
                Assert.AreEqual(CHC.UnsequencedHashCode(1, 2, 3), h.GetHashCode(s));
            }

            //Nongeneric types implementing collection types:
            private interface IBaz : ISequenced<int> { void Bamse(); }

            private class Baz : LinkedList<int>, IBaz
            {
                internal Baz() : base() { }
                public void Bamse() { }
                //int ISequenced<int>.GetHashCode() { return sequencedhashcode(); }
                //bool ISequenced<int>.Equals(ISequenced<int> that) { return sequencedequals(that); }
            }

            [Test]
            public void SeqequalityComparerViaBuilder4()
            {
                SCG.IEqualityComparer<IBaz> h = C5.EqualityComparer<IBaz>.Default;
                IBaz s = new Baz() { 1, 2, 3 };
                Assert.AreEqual(CHC.SequencedHashCode(1, 2, 3), h.GetHashCode(s));
            }

            private interface IBar : C5.ICollection<int>
            {
                void Bamse();
            }

            private class Bar : C5.HashSet<int>, IBar
            {
                internal Bar() : base() { }
                public void Bamse() { }

                //TODO: remove all this workaround stuff:

                bool C5.ICollection<int>.ContainsAll(System.Collections.Generic.IEnumerable<int> items)
                {
                    throw new NotImplementedException();
                }

                void C5.ICollection<int>.RemoveAll(System.Collections.Generic.IEnumerable<int> items)
                {
                    throw new NotImplementedException();
                }

                void C5.ICollection<int>.RetainAll(System.Collections.Generic.IEnumerable<int> items)
                {
                    throw new NotImplementedException();
                }

                void IExtensible<int>.AddAll(SCG.IEnumerable<int> enumerable)
                {
                    throw new NotImplementedException();
                }

            }

            [Test]
            public void UnseqequalityComparerViaBuilder4()
            {
                SCG.IEqualityComparer<IBar> h = C5.EqualityComparer<IBar>.Default;
                IBar s = new Bar() { 1, 2, 3 };
                Assert.AreEqual(CHC.UnsequencedHashCode(1, 2, 3), h.GetHashCode(s));
            }

            [Test]
            public void StaticEqualityComparerWithNull()
            {
                ArrayList<double> arr = new ArrayList<double>();
                SCG.IEqualityComparer<double> eqc = C5.EqualityComparer<double>.Default;
                Assert.IsTrue(CollectionBase<double>.StaticEquals(arr, arr, eqc));
                Assert.IsTrue(CollectionBase<double>.StaticEquals(null, null, eqc));
                Assert.IsFalse(CollectionBase<double>.StaticEquals(arr, null, eqc));
                Assert.IsFalse(CollectionBase<double>.StaticEquals(null, arr, eqc));
            }

            private class EveryThingIsEqual : SCG.IEqualityComparer<Object>
            {
                public new bool Equals(Object o1, Object o2)
                {
                    return true;
                }

                public int GetHashCode(Object o)
                {
                    return 1;
                }
            }

            [Test]
            public void UnsequencedCollectionComparerEquality()
            {
                // Repro for bug20101103
                SCG.IEqualityComparer<Object> eqc = new EveryThingIsEqual();
                Object o1 = new Object(), o2 = new Object();
                C5.ICollection<Object> coll1 = new ArrayList<Object>(eqc);
                C5.ICollection<Object> coll2 = new ArrayList<Object>(eqc);
                coll1.Add(o1);
                coll2.Add(o2);
                Assert.IsFalse(o1.Equals(o2));
                Assert.IsTrue(coll1.UnsequencedEquals(coll2));
            }
        }
    }
}
