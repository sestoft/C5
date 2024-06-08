// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
using SCG = System.Collections.Generic;

namespace C5.Tests
{
    internal class TenEqualityComparer : SCG.IEqualityComparer<int>, SCG.IComparer<int>
    {
        private TenEqualityComparer() { }

        public static TenEqualityComparer Instance { get; } = new();

        public int GetHashCode(int item) { return (item / 10).GetHashCode(); }

        public bool Equals(int item1, int item2) { return item1 / 10 == item2 / 10; }

        public int Compare(int a, int b) { return (a / 10).CompareTo(b / 10); }
    }

    internal class IntegerComparer : SCG.IComparer<int>, IComparable<int>
    {
        public int Compare(int a, int b) => a > b ? 1 : a < b ? -1 : 0;

        public int I { get; }

        public IntegerComparer() { }

        public IntegerComparer(int i) => I = i;

        public int CompareTo(int that) => I > that ? 1 : I < that ? -1 : 0;

        public bool Equals(int that) => I == that;
    }

    internal class ReverseIntegerComparer : SCG.IComparer<int>
    {
        public int Compare(int a, int b) => a.CompareTo(b) * -1;
    }

    public class FuncEnumerable : SCG.IEnumerable<int>
    {
        private readonly int size;
        private readonly Func<int, int> f;

        public FuncEnumerable(int size, Func<int, int> f)
        {
            this.size = size; this.f = f;
        }

        public SCG.IEnumerator<int> GetEnumerator()
        {
            for (int i = 0; i < size; i++)
            {
                yield return f(i);
            }
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class BadEnumerableException : Exception { }

    public class BadEnumerable<T> : CollectionValueBase<T>, ICollectionValue<T>
    {
        private readonly T[] contents;
        private readonly Exception exception;

        public BadEnumerable(Exception exception, params T[] contents)
        {
            this.contents = (T[])contents.Clone();
            this.exception = exception;
        }

        public override SCG.IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < contents.Length; i++)
            {
                yield return contents[i];
            }

            throw exception;
        }

        public override bool IsEmpty => false;

        public override int Count => contents.Length + 1;

        public override Speed CountSpeed => Speed.Constant;

        public override T Choose() { throw exception; }
    }

    public class CollectionEventList<T>(SCG.IEqualityComparer<T> itemequalityComparer)
    {
        private readonly ArrayList<CollectionEvent<T>> happened = [];
        private EventType listenTo;

        public void Listen(ICollectionValue<T> list, EventType listenTo)
        {
            this.listenTo = listenTo;
            if ((listenTo & EventType.Changed) != 0)
            {
                list.CollectionChanged += new CollectionChangedHandler<T>(changed);
            }

            if ((listenTo & EventType.Cleared) != 0)
            {
                list.CollectionCleared += new CollectionClearedHandler<T>(cleared);
            }

            if ((listenTo & EventType.Removed) != 0)
            {
                list.ItemsRemoved += new ItemsRemovedHandler<T>(removed);
            }

            if ((listenTo & EventType.Added) != 0)
            {
                list.ItemsAdded += new ItemsAddedHandler<T>(added);
            }

            if ((listenTo & EventType.Inserted) != 0)
            {
                list.ItemInserted += new ItemInsertedHandler<T>(inserted);
            }

            if ((listenTo & EventType.RemovedAt) != 0)
            {
                list.ItemRemovedAt += new ItemRemovedAtHandler<T>(removedAt);
            }
        }
        public void Add(CollectionEvent<T> e) { happened.Add(e); }
        /// <summary>
        /// Check that we have seen exactly the events in expected that match listenTo.
        /// </summary>
        /// <param name="expected"></param>
        public void Check(SCG.IEnumerable<CollectionEvent<T>> expected)
        {
            int i = 0;
            foreach (CollectionEvent<T> expectedEvent in expected)
            {
                if ((expectedEvent.Act & listenTo) == 0)
                {
                    continue;
                }

                if (i >= happened.Count)
                {
                    Assert.Fail(string.Format("Event number {0} did not happen:\n expected {1}", i, expectedEvent));
                }

                if (!expectedEvent.Equals(happened[i], itemequalityComparer))
                {
                    Assert.Fail(string.Format("Event number {0}:\n expected {1}\n but saw {2}", i, expectedEvent, happened[i]));
                }

                i++;
            }
            if (i < happened.Count)
            {
                Assert.Fail(string.Format("Event number {0} seen but no event expected:\n {1}", i, happened[i]));
            }

            happened.Clear();
        }
        public void Clear() { happened.Clear(); }
        public void Print(System.IO.TextWriter writer)
        {
            happened.Apply(delegate (CollectionEvent<T> e) { writer.WriteLine(e); });
        }

        private void changed(object sender)
        {
            happened.Add(new CollectionEvent<T>(EventType.Changed, new EventArgs(), sender));
        }

        private void cleared(object sender, ClearedEventArgs eventArgs)
        {
            happened.Add(new CollectionEvent<T>(EventType.Cleared, eventArgs, sender));
        }

        private void added(object sender, ItemCountEventArgs<T> eventArgs)
        {
            happened.Add(new CollectionEvent<T>(EventType.Added, eventArgs, sender));
        }

        private void removed(object sender, ItemCountEventArgs<T> eventArgs)
        {
            happened.Add(new CollectionEvent<T>(EventType.Removed, eventArgs, sender));
        }

        private void inserted(object sender, ItemAtEventArgs<T> eventArgs)
        {
            happened.Add(new CollectionEvent<T>(EventType.Inserted, eventArgs, sender));
        }

        private void removedAt(object sender, ItemAtEventArgs<T> eventArgs)
        {
            happened.Add(new CollectionEvent<T>(EventType.RemovedAt, eventArgs, sender));
        }
    }

    public sealed class CollectionEvent<T>
    {
        public EventType Act { get; }

        public EventArgs Args { get; }

        public object Sender { get; }

        public CollectionEvent(EventType act, EventArgs args, object sender)
        {
            Act = act;
            Args = args;
            Sender = sender;
        }

        public bool Equals(CollectionEvent<T> otherEvent, SCG.IEqualityComparer<T> itemequalityComparer)
        {
            if (otherEvent == null || Act != otherEvent.Act || !ReferenceEquals(Sender, otherEvent.Sender))
            {
                return false;
            }

            switch (Act)
            {
                case EventType.None:
                    break;
                case EventType.Changed:
                    return true;
                case EventType.Cleared:
                    if (Args is ClearedRangeEventArgs)
                    {
                        ClearedRangeEventArgs a = Args as ClearedRangeEventArgs;
                        if (!(otherEvent.Args is ClearedRangeEventArgs o))
                        {
                            return false;
                        }

                        return a.Full == o.Full && a.Start == o.Start && a.Count == o.Count;
                    }
                    else
                    {
                        if (otherEvent.Args is ClearedRangeEventArgs)
                        {
                            return false;
                        }

                        ClearedEventArgs a = Args as ClearedEventArgs, o = otherEvent.Args as ClearedEventArgs;
                        return a.Full == o.Full && a.Count == o.Count;
                    }
                case EventType.Added:
                    {
                        ItemCountEventArgs<T> a = Args as ItemCountEventArgs<T>, o = otherEvent.Args as ItemCountEventArgs<T>;
                        return itemequalityComparer.Equals(a.Item, o.Item) && a.Count == o.Count;
                    }
                case EventType.Removed:
                    {
                        ItemCountEventArgs<T> a = Args as ItemCountEventArgs<T>, o = otherEvent.Args as ItemCountEventArgs<T>;
                        return itemequalityComparer.Equals(a.Item, o.Item) && a.Count == o.Count;
                    }
                case EventType.Inserted:
                    {
                        ItemAtEventArgs<T> a = Args as ItemAtEventArgs<T>, o = otherEvent.Args as ItemAtEventArgs<T>;
                        return a.Index == o.Index && itemequalityComparer.Equals(a.Item, o.Item);
                    }
                case EventType.RemovedAt:
                    {
                        ItemAtEventArgs<T> a = Args as ItemAtEventArgs<T>, o = otherEvent.Args as ItemAtEventArgs<T>;
                        return a.Index == o.Index && itemequalityComparer.Equals(a.Item, o.Item);
                    }
            }
            throw new ApplicationException("Illegal Action: " + Act);
        }

        public override string ToString()
        {
            return string.Format("Action: {0}, Args : {1}, Source : {2}", Act, Args, Sender);
        }

    }

    public class CHC
    {
        public static int UnsequencedHashCode(params int[] a)
        {
            int h = 0;
            foreach (int i in a)
            {
                h += (int)(((uint)i * 1529784657 + 1) ^ ((uint)i * 2912831877) ^ ((uint)i * 1118771817 + 2));
            }
            return h;
        }
        public static int SequencedHashCode(params int[] a)
        {
            int h = 0;
            foreach (int i in a) { h = h * 31 + i; }
            return h;
        }
    }

    //This class is a modified sample from VS2005 beta1 documentation
    public class RadixFormatProvider : IFormatProvider
    {
        private readonly RadixFormatter _radixformatter;
        public RadixFormatProvider(int radix)
        {
            if (radix < 2 || radix > 36)
            {
                throw new ArgumentException(string.Format(
                    "The radix \"{0}\" is not in the range 2..36.",
                    radix));
            }

            _radixformatter = new RadixFormatter(radix);
        }
        public object GetFormat(Type argType)
        {
            if (argType == typeof(ICustomFormatter))
            {
                return _radixformatter;
            }
            else
            {
                return null;
            }
        }
    }

    //This class is a modified sample from VS2005 beta1 documentation
    public class RadixFormatter : ICustomFormatter
    {
        private readonly int radix;
        public RadixFormatter(int radix)
        {
            if (radix < 2 || radix > 36)
            {
                throw new ArgumentException(string.Format(
                    "The radix \"{0}\" is not in the range 2..36.",
                    radix));
            }

            this.radix = radix;
        }

        // The value to be formatted is returned as a signed string
        // of digits from the rDigits array.
        private static readonly char[] rDigits = [
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
        'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
        'U', 'V', 'W', 'X', 'Y', 'Z' ];

        public string Format(string? format, object? arg, IFormatProvider? formatProvider)
        {
            int intToBeFormatted;
            try
            {
                intToBeFormatted = (int)arg!;
            }
            catch (Exception)
            {
                if (arg is IFormattable formattable)
                {
                    return formattable.ToString(format, formatProvider);
                }
                else if (IsKeyValuePair(arg))
                {
                    var (key, value) = GetKeyValuePair(arg);
                    var formattedKey = Format(format, key, formatProvider);
                    var formattedValue = Format(format, value, formatProvider);
                    return $"[{formattedKey}, {formattedValue}]";
                }
                else
                {
                    return arg.ToString();
                }
            }
            return formatInt(intToBeFormatted);
        }

        private string formatInt(int intToBeFormatted)
        {
            // The formatting is handled here.
            if (intToBeFormatted == 0)
            {
                return "0";
            }

            int intPositive;
            char[] outDigits = new char[31];

            // Verify that the argument can be converted to a int integer.
            // Extract the magnitude for conversion.
            intPositive = Math.Abs(intToBeFormatted);
            int digitIndex;
            // Convert the magnitude to a digit string.
            for (digitIndex = 0; digitIndex <= 32; digitIndex++)
            {
                if (intPositive == 0)
                {
                    break;
                }

                outDigits[outDigits.Length - digitIndex - 1] =
                    rDigits[intPositive % radix];
                intPositive /= radix;
            }

            // Add a minus sign if the argument is negative.
            if (intToBeFormatted < 0)
            {
                outDigits[outDigits.Length - digitIndex++ - 1] =
                    '-';
            }

            return new string(outDigits,
                outDigits.Length - digitIndex, digitIndex);
        }

        // SCG.KeyValuePair is not showable, so we hack it now.
        private static bool IsKeyValuePair(object arg)
        {
            if (arg != null)
            {
                var type = arg.GetType();
                if (type.IsGenericType)
                {
                    var baseType = type.GetGenericTypeDefinition();

                    return baseType == typeof(SCG.KeyValuePair<,>);
                }
            }
            return false;
        }

        private static (object? key, object? value) GetKeyValuePair(object arg)
        {
            var type = arg.GetType();

            var key = type.GetProperty("Key")?.GetValue(arg, null);
            var value = type.GetProperty("Value")?.GetValue(arg, null);

            return (key, value);
        }
    }
}
