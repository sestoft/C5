using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace C5
{
    [Serializable]
    internal abstract class MemorySafeEnumerator<T> : IEnumerator<T>, IEnumerable<T>, IDisposable
    {
        private static int MainThreadId;

        //-1 means an iterator is not in use.
        protected int IteratorState;

        protected MemoryType MemoryType { get; private set; }

        protected static bool IsMainThread
        {
#if NETSTANDARD1_5
            get { return Environment.CurrentManagedThreadId == MainThreadId; }
#else
            get { return Thread.CurrentThread.ManagedThreadId == MainThreadId; }
#endif
        }

        protected MemorySafeEnumerator(MemoryType memoryType)
        {
            MemoryType = memoryType;
#if NETSTANDARD1_5
            MainThreadId = Environment.CurrentManagedThreadId;
#else
            MainThreadId = Thread.CurrentThread.ManagedThreadId;
#endif
            IteratorState = -1;
        }

        protected abstract MemorySafeEnumerator<T> Clone();

        public abstract bool MoveNext();

        public abstract void Reset();

        public T Current { get; protected set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public virtual void Dispose()
        {
            IteratorState = -1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            MemorySafeEnumerator<T> enumerator;

            switch (MemoryType)
            {
                case MemoryType.Normal:
                    enumerator = Clone();
                    break;
                case MemoryType.Safe:
                    if (IsMainThread)
                    {
                        enumerator = IteratorState != -1 ? Clone() : this;

                        IteratorState = 0;
                    }
                    else
                    {
                        enumerator = Clone();
                    }
                    break;
                case MemoryType.Strict:
                    if (!IsMainThread)
                    {
                        throw new ConcurrentEnumerationException("Multithread access detected! In Strict memory mode is not possible to iterate the collection from different threads");
                    }

                    if (IteratorState != -1)
                    {
                        throw new MultipleEnumerationException("Multiple Enumeration detected! In Strict memory mode is not possible to iterate the collection multiple times");
                    }

                    enumerator = this;
                    IteratorState = 0;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
