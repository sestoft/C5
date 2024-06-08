// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: Keyword recognition 2004-12-20

namespace C5.UserGuideExamples;

internal class KeywordRecognition
{
    // Array of 77 keywords:
    private static readonly string[] _keywordArray =
    [
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
            "char", "checked", "class", "const", "continue", "decimal", "default",
            "delegate", "do", "double", "else", "enum", "event", "explicit",
            "extern", "false", "finally", "fixed", "float", "for", "foreach",
            "goto", "if", "implicit", "in", "int", "interface", "internal", "is",
            "lock", "long", "namespace", "new", "null", "object", "operator",
            "out", "override", "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
            "stackalloc", "static", "string", "struct", "switch", "this", "throw",
            "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
            "ushort", "using", "virtual", "void", "volatile", "while"
        ];

    private static readonly ICollection<string> _keywords1;

    private static readonly ICollection<string> _keywords2;

    private static readonly ICollection<string> _keywords3;

    private static readonly SCG.IDictionary<string, bool> _keywords4;

    private class SC : SCG.IComparer<string>
    {
        public int Compare(string a, string b)
        {
            return StringComparer.InvariantCulture.Compare(a, b);
        }
    }

    private class SH : SCG.IEqualityComparer<string>
    {
        public int GetHashCode(string item)
        {
            return item.GetHashCode();
        }

        public bool Equals(string i1, string i2)
        {
            return i1 == null ? i2 == null : i1.Equals(i2, StringComparison.InvariantCulture);
        }
    }

    static KeywordRecognition()
    {
        _keywords1 = new HashSet<string>();
        _keywords1.AddAll(_keywordArray);
        _keywords2 = new TreeSet<string>(StringComparer.InvariantCulture);
        _keywords2.AddAll(_keywordArray);
        _keywords3 = new SortedArray<string>(StringComparer.InvariantCulture);
        _keywords3.AddAll(_keywordArray);
        _keywords4 = new SCG.Dictionary<string, bool>();

        foreach (var keyword in _keywordArray)
        {
            _keywords4.Add(keyword, false);
        }
    }

    public static bool IsKeyword1(string s)
    {
        return _keywords1.Contains(s);
    }

    public static bool IsKeyword2(string s)
    {
        return _keywords2.Contains(s);
    }

    public static bool IsKeyword3(string s)
    {
        return _keywords3.Contains(s);
    }

    public static bool IsKeyword4(string s)
    {
        return _keywords4.ContainsKey(s);
    }

    public static bool IsKeyword5(string s)
    {
        return Array.BinarySearch(_keywordArray, s) >= 0;
    }

    public static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: KeywordRecognition <iterations> <word>");
        }
        else
        {
            var count = int.Parse(args[0]);
            var id = args[1];

            {
                Console.Write("HashSet.Contains ");
                var sw = Stopwatch.StartNew();

                for (var i = 0; i < count; i++)
                {
                    IsKeyword1(id);
                }

                sw.Stop();
                Console.WriteLine(sw.Elapsed);
            }

            {
                Console.Write("TreeSet.Contains ");
                var sw = Stopwatch.StartNew();

                for (var i = 0; i < count; i++)
                {
                    IsKeyword2(id);
                }

                sw.Stop();
                Console.WriteLine(sw.Elapsed);
            }

            {
                Console.Write("SortedArray.Contains ");
                var sw = Stopwatch.StartNew();

                for (var i = 0; i < count; i++)
                {
                    IsKeyword3(id);
                }

                sw.Stop();
                Console.WriteLine(sw.Elapsed);
            }

            {
                Console.Write("SCG.Dictionary.ContainsKey ");
                var sw = Stopwatch.StartNew();

                for (var i = 0; i < count; i++)
                {
                    IsKeyword4(id);
                }

                sw.Stop();
                Console.WriteLine(sw.Elapsed);
            }

            {
                Console.Write("Array.BinarySearch ");
                var sw = Stopwatch.StartNew();

                for (var i = 0; i < count; i++)
                {
                    IsKeyword5(id);
                }

                sw.Stop();
                Console.WriteLine(sw.Elapsed);
            }
        }
    }
}
