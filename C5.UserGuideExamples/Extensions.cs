// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// Experiment with extension methods and C5, 2007-10-31

namespace C5.UserGuideExamples;

internal static class Extension
{
    public static int Added<T>(this ICollection<T> coll, int x)
    {
        return coll.Count + x;
    }

    public static SCG.IEnumerable<T> Where<T>(this ICollection<T> coll, Expression<Func<T, bool>> predicate)
    {
        var p = predicate.Compile();
        foreach (T item in coll)
        {
            if (p(item))
            {
                yield return item;
            }
        }
    }

    private static void Main()
    {
        HashSet<NamedPerson> hs = [new("Ole"), new("Hans")];

        foreach (NamedPerson q in (from p in hs where p.Name.Length == 4 select p))
        {
            Console.WriteLine(q);
        }
    }
}

internal class NamedPerson(string name)
{
    public string Name { get; } = name;

    public override string ToString()
    {
        return Name;
    }
}
