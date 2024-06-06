// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

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
                public ABT() : base(8, EqualityComparer<string>.Default) { }

                public override string Choose() { if (size > 0) { return array[0]; } throw new NoSuchItemException(); }

                public string this[int i] { get => array[i]; set => array[i] = value; }


                public int thesize { get => size; set => size = value; }
            }


            [Test]
            public void Check()
            {
                ABT abt = new()
                {
                    thesize = 3
                };
                abt[2] = "aaa";
                // Assert.IsFalse(abt.Check());
                abt[0] = "##";
                abt[1] = "##";
                Assert.That(abt.Check(), Is.True);
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
                dbl s = new(3.4);
                dbl t = new(3.4);
                dbl u = new(7.4);

                Assert.Multiple(() =>
                {
                    Assert.That(h.Compare(s, t), Is.EqualTo(0));
                    Assert.That(h.Compare(s, u), Is.LessThan(0));
                });
            }


            [Test]
            public void OrdinaryC()
            {
                SCG.IComparer<string> h = SCG.Comparer<string>.Default;
                string s = "bamse";
                string t = "bamse";
                string u = "bimse";

                Assert.Multiple(() =>
                {
                    Assert.That(h.Compare(s, t), Is.EqualTo(0));
                    Assert.That(h.Compare(s, u), Is.LessThan(0));
                });
            }


            [Test]
            public void GenericCViaBuilder()
            {
                SCG.IComparer<dbl> h = SCG.Comparer<dbl>.Default;
                dbl s = new(3.4);
                dbl t = new(3.4);
                dbl u = new(7.4);

                Assert.Multiple(() =>
                {
                    Assert.That(h.Compare(s, t), Is.EqualTo(0));
                    Assert.That(h.Compare(s, u), Is.LessThan(0));
                    Assert.That(SCG.Comparer<dbl>.Default, Is.SameAs(h));
                });
            }


            [Test]
            public void OrdinaryCViaBuilder()
            {
                SCG.IComparer<string> h = SCG.Comparer<string>.Default;
                string s = "bamse";
                string t = "bamse";
                string u = "bimse";

                Assert.Multiple(() =>
                {
                    Assert.That(h.Compare(s, t), Is.EqualTo(0));
                    Assert.That(h.Compare(s, u), Is.LessThan(0));
                    Assert.That(SCG.Comparer<string>.Default, Is.SameAs(h));
                });

            }

            public void ComparerViaBuilderTest<T>(T item1, T item2)
                where T : IComparable<T>
            {
                SCG.IComparer<T> h = SCG.Comparer<T>.Default;
                Assert.Multiple(() =>
                {
                    Assert.That(SCG.Comparer<T>.Default, Is.SameAs(h));
                    Assert.That(h.Compare(item1, item1), Is.EqualTo(0));
                    Assert.That(h.Compare(item2, item2), Is.EqualTo(0));
                    Assert.That(h.Compare(item1, item2), Is.LessThan(0));
                    Assert.That(h.Compare(item2, item1), Is.GreaterThan(0));
                    Assert.That(Math.Sign(h.Compare(item1, item2)), Is.EqualTo(Math.Sign(item1.CompareTo(item2))));
                    Assert.That(Math.Sign(h.Compare(item2, item1)), Is.EqualTo(Math.Sign(item2.CompareTo(item1))));
                });
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

                Assert.Multiple(() =>
                {
                    Assert.That(h.Compare(s, t), Is.EqualTo(0));
                    Assert.That(h.Compare(s, u), Is.LessThan(0));
                    Assert.That(SCG.Comparer<int>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void Nulls()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(SCG.Comparer<string>.Default.Compare(null, "abe"), Is.LessThan(0));
                    Assert.That(SCG.Comparer<string>.Default.Compare(null, null), Is.EqualTo(0));
                    Assert.That(SCG.Comparer<string>.Default.Compare("abe", null), Is.GreaterThan(0));
                });
            }
        }

        [TestFixture]
        public class EqualityComparers
        {
            [Test]
            public void ReftypeequalityComparer()
            {
                SCG.IEqualityComparer<string> h = EqualityComparer<string>.Default;
                string s = "bamse";
                string t = "bamse";
                string u = "bimse";

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                });
            }


            [Test]
            public void ValuetypeequalityComparer()
            {
                SCG.IEqualityComparer<double> h = EqualityComparer<double>.Default;
                double s = 3.4;
                double t = 3.4;
                double u = 5.7;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                });
            }

            [Test]
            public void ReftypeequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<string> h = EqualityComparer<string>.Default;
                string s = "bamse";
                string t = "bamse";
                string u = "bimse";

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<string>.Default, Is.SameAs(h));
                });
            }


            [Test]
            public void ValuetypeequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<double> h = EqualityComparer<double>.Default;
                double s = 3.4;
                double t = 3.4;
                double u = 5.7;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<double>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void CharequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<char> h = EqualityComparer<char>.Default;
                char s = '�';
                char t = '�';
                char u = 'r';

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<char>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void SbyteequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<sbyte> h = EqualityComparer<sbyte>.Default;
                sbyte s = 3;
                sbyte t = 3;
                sbyte u = -5;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<sbyte>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void ByteequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<byte> h = EqualityComparer<byte>.Default;
                byte s = 3;
                byte t = 3;
                byte u = 5;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<byte>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void ShortequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<short> h = EqualityComparer<short>.Default;
                short s = 3;
                short t = 3;
                short u = -5;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<short>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void UshortequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<ushort> h = EqualityComparer<ushort>.Default;
                ushort s = 3;
                ushort t = 3;
                ushort u = 60000;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<ushort>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void IntequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<int> h = EqualityComparer<int>.Default;
                int s = 3;
                int t = 3;
                int u = -5;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<int>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void UintequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<uint> h = EqualityComparer<uint>.Default;
                uint s = 3;
                uint t = 3;
                uint u = 3000000000;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<uint>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void LongequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<long> h = EqualityComparer<long>.Default;
                long s = 3;
                long t = 3;
                long u = -500000000000000L;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<long>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void UlongequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<ulong> h = EqualityComparer<ulong>.Default;
                ulong s = 3;
                ulong t = 3;
                ulong u = 500000000000000UL;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<ulong>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void FloatequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<float> h = EqualityComparer<float>.Default;
                float s = 3.1F;
                float t = 3.1F;
                float u = -5.2F;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<float>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void DoubleequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<double> h = EqualityComparer<double>.Default;
                double s = 3.12345;
                double t = 3.12345;
                double u = -5.2;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<double>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void DecimalequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<decimal> h = EqualityComparer<decimal>.Default;
                decimal s = 3.0001M;
                decimal t = 3.0001M;
                decimal u = -500000000000000M;

                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<decimal>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void UnseqequalityComparerViaBuilder()
            {
                SCG.IEqualityComparer<C5.ICollection<int>> h = EqualityComparer<ICollection<int>>.Default;
                C5.ICollection<int> s = new LinkedList<int>();
                C5.ICollection<int> t = new LinkedList<int>();
                C5.ICollection<int> u = new LinkedList<int>();
                s.Add(1); s.Add(2); s.Add(3);
                t.Add(3); t.Add(2); t.Add(1);
                u.Add(3); u.Add(2); u.Add(4);
                Assert.Multiple(() =>
                {
                    Assert.That(h.GetHashCode(s), Is.EqualTo(s.GetUnsequencedHashCode()));
                    Assert.That(h.Equals(s, t), Is.True);
                    Assert.That(h.Equals(s, u), Is.False);
                    Assert.That(EqualityComparer<ICollection<int>>.Default, Is.SameAs(h));
                });
            }

            [Test]
            public void SeqequalityComparerViaBuilder2()
            {
                SCG.IEqualityComparer<LinkedList<int>> h = EqualityComparer<LinkedList<int>>.Default;
                LinkedList<int> s = new() { 1, 2, 3 };
                Assert.That(h.GetHashCode(s), Is.EqualTo(CHC.SequencedHashCode(1, 2, 3)));
            }

            [Test]
            public void UnseqequalityComparerViaBuilder2()
            {
                SCG.IEqualityComparer<C5.HashSet<int>> h = EqualityComparer<HashSet<int>>.Default;
                C5.HashSet<int> s = new() { 1, 2, 3 };
                Assert.That(h.GetHashCode(s), Is.EqualTo(CHC.UnsequencedHashCode(1, 2, 3)));
            }

            //generic types implementing collection interfaces
            [Test]
            public void SeqequalityComparerViaBuilder3()
            {
                SCG.IEqualityComparer<C5.IList<int>> h = EqualityComparer<IList<int>>.Default;
                C5.IList<int> s = new LinkedList<int>() { 1, 2, 3 };
                Assert.That(h.GetHashCode(s), Is.EqualTo(CHC.SequencedHashCode(1, 2, 3)));
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
                SCG.IEqualityComparer<IFoo<int>> h = EqualityComparer<IFoo<int>>.Default;
                IFoo<int> s = new Foo<int>() { 1, 2, 3 };
                Assert.That(h.GetHashCode(s), Is.EqualTo(CHC.UnsequencedHashCode(1, 2, 3)));
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
                SCG.IEqualityComparer<IBaz> h = EqualityComparer<IBaz>.Default;
                IBaz s = new Baz() { 1, 2, 3 };
                Assert.That(h.GetHashCode(s), Is.EqualTo(CHC.SequencedHashCode(1, 2, 3)));
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
                SCG.IEqualityComparer<IBar> h = EqualityComparer<IBar>.Default;
                IBar s = new Bar() { 1, 2, 3 };
                Assert.That(h.GetHashCode(s), Is.EqualTo(CHC.UnsequencedHashCode(1, 2, 3)));
            }

            [Test]
            public void StaticEqualityComparerWithNull()
            {
                ArrayList<double> arr = new();
                SCG.IEqualityComparer<double> eqc = EqualityComparer<double>.Default;
                Assert.Multiple(() =>
                {
                    Assert.That(CollectionBase<double>.StaticEquals(arr, arr, eqc), Is.True);
                    Assert.That(CollectionBase<double>.StaticEquals(null, null, eqc), Is.True);
                    Assert.That(CollectionBase<double>.StaticEquals(arr, null, eqc), Is.False);
                    Assert.That(CollectionBase<double>.StaticEquals(null, arr, eqc), Is.False);
                });
            }

            private class EveryThingIsEqual : SCG.IEqualityComparer<object>
            {
                public new bool Equals(object o1, object o2)
                {
                    return true;
                }

                public int GetHashCode(object o)
                {
                    return 1;
                }
            }

            [Test]
            public void UnsequencedCollectionComparerEquality()
            {
                // Repro for bug20101103
                SCG.IEqualityComparer<object> eqc = new EveryThingIsEqual();
                object o1 = new(), o2 = new();
                C5.ICollection<Object> coll1 = new ArrayList<object>(eqc);
                C5.ICollection<Object> coll2 = new ArrayList<object>(eqc);
                coll1.Add(o1);
                coll2.Add(o2);
                Assert.Multiple(() =>
                {
                    Assert.That(o1.Equals(o2), Is.False);
                    Assert.That(coll1.UnsequencedEquals(coll2), Is.True);
                });
            }
        }
    }
}
