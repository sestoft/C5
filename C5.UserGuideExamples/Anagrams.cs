// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: anagrams 2004-08-08, 2004-11-16

namespace C5.UserGuideExamples;

internal class Anagrams
{
    private static void Main(string[] args)
    {
        var ss = args.Length == 2 ? ReadFileWords(args[0], int.Parse(args[1])) : args;

        foreach (var s in FirstAnagramOnly(ss))
        {
            Console.WriteLine(s);
        }
        Console.WriteLine("===");

        var sw = Stopwatch.StartNew();
        var classes = AnagramClasses(ss);
        var count = 0;

        foreach (SCG.IEnumerable<string> anagramClass in classes)
        {
            count++;
            foreach (string s in anagramClass)
            {
                Console.Write(s + " ");
            }
            Console.WriteLine();
        }

        Console.WriteLine($"{count} non-trivial anagram classes");

        sw.Stop();

        Console.WriteLine(sw.Elapsed);
    }

    // Read words at most n words from a file
    private static SCG.IEnumerable<string> ReadFileWords(string filename, int n)
    {
        var delimiter = new Regex("[^a-z���A-Z���0-9-]+");

        using var reader = File.OpenText(filename);
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
        {
            foreach (var s in delimiter.Split(line))
            {
                if (s != "")
                {
                    yield return s.ToLower();
                }
                if (--n == 0)
                {
                    yield break;
                }
            }
        }
    }

    // From an anagram point of view, a word is just a bag of
    // characters.  So an anagram class is represented as TreeBag<char>
    // which permits fast equality comparison -- we shall use them as
    // elements of hash sets or keys in hash maps.

    // From an anagram point of view, a word is just a bag of
    // characters.  So an anagram class is represented as HashBag<char>
    // which permits fast equality comparison -- we shall use them as
    // elements of hash sets or keys in hash maps.
    private static TreeBag<char> AnagramClass(string s)
    {
        var anagram = new TreeBag<char>();

        foreach (char c in s)
        {
            anagram.Add(c);
        }

        return anagram;
    }

    // Given a sequence of strings, return only the first member of each
    // anagram class.
    private static SCG.IEnumerable<string> FirstAnagramOnly(SCG.IEnumerable<string> ss)
    {
        var tbh = UnsequencedCollectionEqualityComparer<TreeBag<char>, char>.Default;
        var anagrams = new HashSet<TreeBag<char>>(tbh);
        foreach (var s in ss)
        {
            var anagram = AnagramClass(s);
            if (!anagrams.Contains(anagram))
            {
                anagrams.Add(anagram);
                yield return s;
            }
        }
    }

    // Given a sequence of strings, return all non-trivial anagram
    // classes.  Should use a *sequenced* equalityComparer on a TreeBag<char>,
    // obviously: after all, characters can be sorted by ASCII code.  On
    // 347 000 distinct Danish words this takes 70 cpu seconds, 180 MB
    // memory, and 263 wall-clock seconds (due to swapping).

    // Using a TreeBag<char> and a sequenced equalityComparer takes 82 cpu seconds
    // and 180 MB RAM to find the 26,058 anagram classes among 347,000
    // distinct words.

    // Using an unsequenced equalityComparer on TreeBag<char> or HashBag<char>
    // makes it criminally slow: at least 1200 cpu seconds.  This must
    // be because many bags get the same hash code, so that there are
    // many collisions.  But exactly how the unsequenced equalityComparer works is
    // not clear ... or is it because unsequenced equality is slow?
    private static SCG.IEnumerable<SCG.IEnumerable<string>> AnagramClasses(SCG.IEnumerable<string> ss, bool unsequenced = true)
    {
        var classes = unsequenced
            ? new HashDictionary<TreeBag<char>, TreeSet<string>>(UnsequencedCollectionEqualityComparer<TreeBag<char>, char>.Default)
            : new HashDictionary<TreeBag<char>, TreeSet<string>>(SequencedCollectionEqualityComparer<TreeBag<char>, char>.Default);

        foreach (var s in ss)
        {
            var anagram = AnagramClass(s);

            if (!classes.Find(ref anagram, out TreeSet<string> anagramClass))
            {
                classes[anagram] = anagramClass = new TreeSet<string>();
            }

            anagramClass.Add(s);
        }

        foreach (var anagramClass in classes.Values)
        {
            if (anagramClass.Count > 1)
            {
                yield return anagramClass;
            }
        }
    }
}
