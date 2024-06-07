// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: hash codes, good and bad 2005-02-28

namespace C5.UserGuideExamples;

internal class HashCodes
{
    public static void Main(string[] args)
    {
        var count = int.Parse(args[0]);
        {
            Console.Write("Good hash function: ");
            var sw = Stopwatch.StartNew();
            var good = MakeRandom(count, new GoodIntegerEqualityComparer());
            sw.Stop();
            Console.WriteLine($"({sw.Elapsed.TotalSeconds} sec, {good.Count} items)");
            var bcd = good.BucketCostDistribution();
            foreach (var entry in bcd)
            {
                Console.WriteLine("{0,7} bucket(s) with cost {1,5}", entry.Value, entry.Key);
            }
        }
        {
            Console.Write("Bad hash function: ");
            var sw = Stopwatch.StartNew();
            var bad = MakeRandom(count, new BadIntegerEqualityComparer());
            sw.Stop();
            Console.WriteLine($"({sw.Elapsed.TotalSeconds} sec, {bad.Count} items)");
            var bcd = bad.BucketCostDistribution();
            foreach (var entry in bcd)
            {
                Console.WriteLine("{0,7} bucket(s) with cost {1,5}", entry.Value, entry.Key);
            }
        }
    }

    private static readonly C5Random rnd = new();

    public static HashSet<int> MakeRandom(int count, SCG.IEqualityComparer<int> eqc)
    {
        var res = eqc == null ? new HashSet<int>() : new HashSet<int>(eqc);

        for (var i = 0; i < count; i++)
        {
            res.Add(rnd.Next(1000000));
        }

        return res;
    }

    private class BadIntegerEqualityComparer : SCG.IEqualityComparer<int>
    {
        public bool Equals(int i1, int i2)
        {
            return i1 == i2;
        }

        public int GetHashCode(int i)
        {
            return i % 7;
        }
    }

    private class GoodIntegerEqualityComparer : SCG.IEqualityComparer<int>
    {
        public bool Equals(int i1, int i2)
        {
            return i1 == i2;
        }

        public int GetHashCode(int i)
        {
            return i;
        }
    }
}
