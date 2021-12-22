// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: locking 2005-11-07

namespace C5.UserGuideExamples
{
    internal class Locking
    {
        private static ArrayList<int> _collection = new ArrayList<int>();
        private static readonly int _count = 1000;

        public static void Main()
        {
            Console.WriteLine("Adding and removing without locking:");
            RunTwoThreads(delegate { AddAndRemove(15000); });
            Console.WriteLine($"Collection has {_collection.Count} items, should be 0");

            _collection = new ArrayList<int>();
            Console.WriteLine("Adding and removing with locking:");
            RunTwoThreads(delegate { SafeAddAndRemove(15000); });
            Console.WriteLine($"Collection has {_collection.Count} items, should be 0");

            Console.WriteLine("Moving items without locking:");
            ArrayList<int> from, to;
            from = new ArrayList<int>();
            to = new ArrayList<int>();

            for (var i = 0; i < _count; i++)
            {
                from.Add(i);
            }

            RunTwoThreads(() =>
            {
                while (!from.IsEmpty)
                {
                    Move(from, to);
                }
            });
            Console.WriteLine($"Collection has {to.Count} items, should be {_count}");

            Console.WriteLine("Moving items with locking:");
            from = new ArrayList<int>();
            to = new ArrayList<int>();

            for (var i = 0; i < _count; i++)
            {
                from.Add(i);
            }

            RunTwoThreads(() =>
            {
                while (!from.IsEmpty)
                {
                    SafeMove(from, to);
                }
            });
            Console.WriteLine("coll has {0} items, should be {1}", to.Count, _count);
        }

        public static void RunTwoThreads(Action run)
        {
            var t1 = new Thread(new ThreadStart(run));
            var t2 = new Thread(new ThreadStart(run));
            t1.Start(); t2.Start();
            t1.Join(); t2.Join();
        }

        // Concurrently adding to and removing from an arraylist
        public static void AddAndRemove(int count)
        {
            for (var i = 0; i < count; i++)
            {
                _collection.Add(i);
            }

            for (var i = 0; i < count; i++)
            {
                _collection.Remove(i);
            }
        }

        private static readonly object _lock = new object();

        public static void SafeAddAndRemove(int count)
        {
            for (var i = 0; i < count; i++)
            {
                lock (_lock)
                {
                    _collection.Add(i);
                }
            }

            for (var i = 0; i < count; i++)
            {
                lock (_lock)
                {
                    _collection.Remove(i);
                }
            }
        }

        public static void SafeAdd<T>(IExtensible<T> coll, T x)
        {
            lock (_lock)
            {
                coll.Add(x);
            }
        }

        public static void Move<T>(ICollection<T> from, ICollection<T> to)
        {
            if (!from.IsEmpty)
            {
                var x = from.Choose();
                Thread.Sleep(0);        // yield processor to other threads
                from.Remove(x);
                to.Add(x);
            }
        }

        public static void SafeMove<T>(ICollection<T> from, ICollection<T> to)
        {
            lock (_lock)
            {
                if (!from.IsEmpty)
                {
                    var x = from.Choose();
                    Thread.Sleep(0);      // yield processor to other threads
                    from.Remove(x);
                    to.Add(x);
                }
            }
        }
    }
}
