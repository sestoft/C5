// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: A bag collection in which items that compare equal may
// be distinct objects or values; that is, a collection with
// AllowsDuplicates=true and DuplicatesByCounting=false.

// The collection may contain distinct objects x1 and x2 for which the
// equality comparer (or comparer) says that x1 and x2 are equal.
// This can be implemented by a dictionary whose keys are the objects
// themselves (using the given equality comparer or comparer), and
// whose values are hashsets that use reference equality for equality
// comparison.

// Such a bag-with-actual-duplicates can be used to index objects, for
// instance Person objects, by name and birthdate.  Several Person
// objects may have the same name, or the same birthdate, or even the
// same name and birthdate, yet be distinct Person objects.

// How much of this can be implemented on top of CollectionBase<T>?

// What should the meaning of Contains(x) be?

namespace C5.UserGuideExamples;

internal class DistinctBagProgram
{
    private static void Main()
    {
        var nameEqualityComparer = ComparerFactory<PersonDistinctBag>.CreateEqualityComparer((a, b) => StringComparer.InvariantCultureIgnoreCase.Equals(a.Name, b.Name), a => a.Name.GetHashCode());
        var nameColl = new DistinctHashBag<PersonDistinctBag>(nameEqualityComparer);
        var p1 = new PersonDistinctBag("Peter", 19620625);
        var p2 = new PersonDistinctBag("Carsten", 19640627);
        var p3 = new PersonDistinctBag("Carsten", 19640628);
        nameColl.Add(p1);
        nameColl.Add(p2);
        nameColl.Add(p3);
        Console.WriteLine("nameColl = {0}", nameColl);
    }
}

public class DistinctHashBag<T> where T : class
{
    private readonly HashDictionary<T, HashSet<T>> _dict;

    public DistinctHashBag(SCG.IEqualityComparer<T> eqc)
    {
        _dict = new HashDictionary<T, HashSet<T>>(eqc);
    }

    public DistinctHashBag() : this(EqualityComparer<T>.Default)
    {
    }

    public bool Add(T item)
    {
        if (!_dict.Contains(item))
        {
            _dict.Add(item, new HashSet<T>(EqualityComparer<T>.Default));
        }
        return _dict[item].Add(item);
    }

    public bool Remove(T item)
    {
        var result = false;
        if (_dict.Contains(item))
        {
            var set = _dict[item];
            result = set.Remove(item);
            if (set.IsEmpty)
            {
                _dict.Remove(item);
            }
        }
        return result;
    }

    public SCG.IEnumerator<T> GetEnumerator()
    {
        foreach (var entry in _dict)
        {
            foreach (T item in entry.Value)
            {
                yield return item;
            }
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (T item in this)
        {
            sb.Append(item).Append(" ");
        }
        return sb.ToString();
    }
}

public class PersonDistinctBag(string name, int date)
{
    public string Name { get; } = name;
    public int Date { get; } = date;

    public override string ToString() => $"{Name} ({Date})";
}
