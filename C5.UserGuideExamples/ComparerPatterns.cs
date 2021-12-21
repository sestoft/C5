// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: Comparer patterns

namespace C5.UserGuideExamples;

internal class ComparerPatterns
{
    public static void Main()
    {
        var lexico1 = Lexico();
        var lexico2 = Lexico2();

        var r1 = ("Carsten", 1962);
        var r2 = ("Carsten", 1964);
        var r3 = ("Christian", 1932);

        Console.WriteLine(lexico1.Compare(r1, r1) == 0);
        Console.WriteLine(lexico1.Compare(r1, r2) < 0);
        Console.WriteLine(lexico1.Compare(r2, r3) < 0);

        Console.WriteLine(lexico2.Compare(r1, r1) == 0);
        Console.WriteLine(lexico2.Compare(r1, r2) < 0);
        Console.WriteLine(lexico2.Compare(r2, r3) < 0);

        var rev = ReverseComparer(SCG.Comparer<string>.Default);
        Console.WriteLine(rev.Compare("A", "A") == 0);
        Console.WriteLine(rev.Compare("A", "B") > 0);
        Console.WriteLine(rev.Compare("B", "A") < 0);
    }

    static SCG.IComparer<(string, int)> Lexico() => ComparerFactory<(string, int)>.CreateComparer((a, b) =>
    {
        var major = a.Item1.CompareTo(b.Item1);
        return major != 0 ? major : a.Item2.CompareTo(b.Item2);
    });

    static SCG.IComparer<(string, int)> Lexico2() => ComparerFactory<(string, int)>.CreateComparer((a, b) =>
    {
        var major = a.Item1.CompareTo(b.Item1);
        return major != 0 ? major : a.Item2.CompareTo(b.Item2);
    });

    static SCG.IComparer<T> ReverseComparer<T>(SCG.IComparer<T> cmp) => ComparerFactory<T>.CreateComparer((a, b) => cmp.Compare(b, a));
}
