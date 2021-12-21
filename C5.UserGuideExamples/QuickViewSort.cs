// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: Quicksort using views as cursors
// Not very efficient due to the many views created.
// Moreover, on LinkedList it seems that .Count or .Offset is
// less efficient than strictly necessary: if count and offset
// were maintained correctly, there should be no dramatic slowdown
// for long lists.
// Is it possible to reuse views more?
// Might be useful to have list.FirstView and list.LastView in addition
// to .First and .Last?
// 2010-03-14

namespace C5.UserGuideExamples;

internal class QuickViewSort
{
    public static void Main(string[] args)
    {
        var count = args.Length > 0 ? int.Parse(args[0]) : 10;
        var arr = RandomInts(count);
        // int[] arr = { 2, 5, 1, 8, 11, 22, 4, 5 };
        // qwsort(arr);
        // Print(arr);
        IList<int> list = new LinkedList<int>();
        list.AddAll(arr);
        var t = new Timer();
        QwSort(list);
        Console.WriteLine("Time = {0} sec", t.Check());
        Console.WriteLine(list[0]);
        Print(list);
    }

    private static void Print<T>(SCG.IEnumerable<T> xs)
    {
        foreach (var x in xs)
        {
            Console.Write(x + " ");
        }

        Console.WriteLine();
    }

    // Array quicksort a la Wirth
    private static void QwSort<T>(T[] arr, int a, int b) where T : IComparable<T>
    {
        // sort arr[a..b]
        if (a < b)
        {
            int i = a, j = b;
            T x = arr[(i + j) / 2];
            do
            {
                while (arr[i].CompareTo(x) < 0)
                {
                    i++;
                }

                while (x.CompareTo(arr[j]) < 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    var tmp = arr[i]; arr[i] = arr[j]; arr[j] = tmp;
                    i++; j--;
                }
            } while (i <= j);
            QwSort(arr, a, j);
            QwSort(arr, i, b);
        }
    }

    public static void QwSort<T>(T[] arr) where T : IComparable<T>
    {
        QwSort(arr, 0, arr.Length - 1);
    }

    private static readonly Random _random = new Random();

    private static int[] RandomInts(int n)
    {
        var arr = new int[n];

        for (var i = 0; i < n; i++)
        {
            arr[i] = _random.Next(100000000);
        }

        return arr;
    }

    // View quicksort a la Wirth
    private static void QwSort<T>(IList<T> list) where T : IComparable<T>
    {
        if (list.Count >= 2)
        {
            IList<T> a = list.View(0, 1), b = list.View(list.Count - 1, 1);
            var x = a[0];
            do
            {
                while (a[0].CompareTo(x) < 0)
                {
                    a.Slide(+1, 1);
                }

                while (x.CompareTo(b[0]) < 0)
                {
                    b.Slide(-1, 1);
                }

                if (a.Offset <= b.Offset)
                {
                    var tmp = a[0]; a[0] = b[0]; b[0] = tmp;
                    a.TrySlide(+1, 1); b.TrySlide(-1, 1);
                }
            } while (a.Offset <= b.Offset);
            QwSort(list.Span(b));
            QwSort(a.Span(list));
        }
    }

    // View quicksort a la Lomuto/Bentley
    private static void QlSort<T>(IList<T> list) where T : IComparable<T>
    {
        if (list.Count >= 2)
        {
            IList<T> a = list.View(0, 1), b = list.View(0, 1);
            T tmp;

            while (b.TrySlide(+1, 1))
            {
                if (b[0].CompareTo(list[0]) < 0)
                {
                    a.Slide(+1, 1);
                    tmp = a[0]; a[0] = b[0]; b[0] = tmp;
                }
            }

            tmp = a[0]; a[0] = list[0]; list[0] = tmp;
            QlSort(list.Span(a.Slide(0, 0)));
            QlSort(a.Slide(+1, 0).Span(list));
        }
    }
}

// Crude timing utility ----------------------------------------
public class Timer
{
    private readonly Stopwatch _stopwatch;

    public Timer()
    {
        _stopwatch = new Stopwatch();
        _stopwatch.Reset();
        _stopwatch.Start();
    }

    public double Check()
    {
        return _stopwatch.ElapsedMilliseconds / 1000.0;
    }
}
