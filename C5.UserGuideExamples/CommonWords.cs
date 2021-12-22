// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: Find and print the most common words in a text file.
// Programming pearl by D.E. Knuth in CACM 29 (June 1986) 471-483.

namespace C5.UserGuideExamples;

internal class CommonWords
{
    private static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: Commonwords <maxwords> <filename>");
        }
        else
        {
            PrintMostCommon(int.Parse(args[0]), args[1]);
        }
    }

    private static void PrintMostCommon(int maxWords, string filename)
    {
        var wordbag = new HashBag<string>();
        var delimiter = new Regex("[^a-zA-Z0-9]+");
        using (var reader = File.OpenText(filename))
        {
            for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                foreach (var s in delimiter.Split(line))
                {
                    if (s != "")
                    {
                        wordbag.Add(s);
                    }
                }
            }
        }
        var frequency = wordbag.ItemMultiplicities().ToArray();
        Sorting.IntroSort(frequency, 0, frequency.Length,
            // Lexicographic ordering: decreasing frequency, then increasing string
            ComparerFactory<System.Collections.Generic.KeyValuePair<string, int>>.CreateComparer(
                (p1, p2) =>
                {
                    int major = p2.Value.CompareTo(p1.Value);
                    return major != 0 ? major : p1.Key.CompareTo(p2.Key);
                }));
        var stop = Math.Min(frequency.Length, maxWords);
        for (var i = 0; i < stop; i++)
        {
            var p = frequency[i];
            Console.WriteLine("{0,4} occurrences of {1}", p.Value, p.Key);
        }
    }
}
