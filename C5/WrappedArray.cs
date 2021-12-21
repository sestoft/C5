// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using System;
using System.Text;
using SCG = System.Collections.Generic;
namespace C5
{
    /// <summary>
    /// An advanced interface to operations on an array. The array is viewed as an
    /// <see cref="T:C5.IList`1"/> of fixed size, and so all operations that would change the
    /// size of the array will be invalid (and throw <see cref="T:C5.FixedSizeCollectionException"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class WrappedArray<T> : IList<T>, SCG.IList<T>
    {
        [Serializable]
        private class InnerList : ArrayList<T>
        {
            internal InnerList(T[] array) { this.array = array; size = array.Length; }
        }

        private readonly ArrayList<T> innerlist;

        //TODO: remember a ref to the wrapped array in WrappedArray to save a little on indexing?
        private readonly WrappedArray<T>? underlying;

        /// <summary>
        ///
        /// </summary>
        /// <param name="wrappedarray"></param>
        public WrappedArray(T[] wrappedarray) { innerlist = new InnerList(wrappedarray); }

        //for views
        private WrappedArray(ArrayList<T> arraylist, WrappedArray<T> underlying) { innerlist = arraylist; this.underlying = underlying; }

        #region IList<T> Members

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public T First => innerlist.First;

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public T Last => innerlist.Last;

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get => innerlist[index];
            set => innerlist[index] = value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<T> FindAll(Func<T, bool> filter) { return innerlist.FindAll(filter); }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public IList<V> Map<V>(Func<T, V> mapper) { return innerlist.Map<V>(mapper); }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public IList<V> Map<V>(Func<T, V> mapper, SCG.IEqualityComparer<V> equalityComparer) { return innerlist.Map<V>(mapper, equalityComparer); }

        /// <summary>
        /// ???? should we throw NotRelevantException
        /// </summary>
        /// <value></value>
        public bool FIFO
        {
            get => throw new FixedSizeCollectionException();
            set => throw new FixedSizeCollectionException();
        }

        /// <summary>
        ///
        /// </summary>
        public virtual bool IsFixedSize => true;

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            throw new FixedSizeCollectionException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="item"></param>
        public void Insert(IList<T> pointer, T item)
        {
            throw new FixedSizeCollectionException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        public void InsertFirst(T item)
        {
            throw new FixedSizeCollectionException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        public void InsertLast(T item)
        {
            throw new FixedSizeCollectionException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <param name="items"></param>
        public void InsertAll(int i, System.Collections.Generic.IEnumerable<T> items)
        {
            throw new FixedSizeCollectionException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public T Remove()
        {
            throw new FixedSizeCollectionException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public T RemoveFirst()
        {
            throw new FixedSizeCollectionException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public T RemoveLast()
        {
            throw new FixedSizeCollectionException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IList<T>? View(int start, int count)
        {
            return new WrappedArray<T>((ArrayList<T>)innerlist.View(start, count)!, underlying ?? this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IList<T>? ViewOf(T item)
        {
            return new WrappedArray<T>((ArrayList<T>)innerlist.ViewOf(item)!, underlying ?? this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IList<T>? LastViewOf(T item)
        {
            return new WrappedArray<T>((ArrayList<T>)innerlist.LastViewOf(item)!, underlying ?? this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public IList<T>? Underlying => underlying;

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public int Offset => innerlist.Offset;

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public bool IsValid => innerlist.IsValid;

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public IList<T> Slide(int offset) { return innerlist.Slide(offset); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IList<T> Slide(int offset, int size) { return innerlist.Slide(offset, size); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool TrySlide(int offset) { return innerlist.TrySlide(offset); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool TrySlide(int offset, int size) { return innerlist.TrySlide(offset, size); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="otherView"></param>
        /// <returns></returns>
        public IList<T>? Span(IList<T> otherView) { return innerlist.Span(((WrappedArray<T>)otherView).innerlist); }

        /// <summary>
        ///
        /// </summary>
        public void Reverse() { innerlist.Reverse(); }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool IsSorted() { return innerlist.IsSorted(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool IsSorted(SCG.IComparer<T> comparer) { return innerlist.IsSorted(comparer); }

        /// <summary>
        ///
        /// </summary>
        public void Sort() { innerlist.Sort(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(SCG.IComparer<T> comparer) { innerlist.Sort(comparer); }

        /// <summary>
        ///
        /// </summary>
        public void Shuffle() { innerlist.Shuffle(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rnd"></param>
        public void Shuffle(Random rnd) { innerlist.Shuffle(rnd); }

        #endregion

        #region IIndexed<T> Members

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public Speed IndexingSpeed => Speed.Constant;

        /// <summary>
        ///
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IDirectedCollectionValue<T> this[int start, int count] => innerlist[start, count];

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item) { return innerlist.IndexOf(item); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int LastIndexOf(T item) { return innerlist.LastIndexOf(item); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int FindIndex(Func<T, bool> predicate) { return innerlist.FindIndex(predicate); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int FindLastIndex(Func<T, bool> predicate) { return innerlist.FindLastIndex(predicate); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T RemoveAt(int i) { throw new FixedSizeCollectionException(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        public void RemoveInterval(int start, int count) { throw new FixedSizeCollectionException(); }

        #endregion

        #region ISequenced<T> Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public int GetSequencedHashCode() { return innerlist.GetSequencedHashCode(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public bool SequencedEquals(ISequenced<T> that) { return innerlist.SequencedEquals(that); }

        #endregion

        #region ICollection<T> Members
        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public Speed ContainsSpeed => Speed.Linear;

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public int GetUnsequencedHashCode() { return innerlist.GetUnsequencedHashCode(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public bool UnsequencedEquals(ICollection<T> that) { return innerlist.UnsequencedEquals(that); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item) { return innerlist.Contains(item); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int ContainsCount(T item) { return innerlist.ContainsCount(item); }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ICollectionValue<T> UniqueItems() { return innerlist.UniqueItems(); }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ICollectionValue<System.Collections.Generic.KeyValuePair<T, int>> ItemMultiplicities() { return innerlist.ItemMultiplicities(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool ContainsAll(System.Collections.Generic.IEnumerable<T> items)
        { return innerlist.ContainsAll(items); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Find(ref T item) { return innerlist.Find(ref item); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool FindOrAdd(ref T item) { throw new FixedSizeCollectionException(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Update(T item) { throw new FixedSizeCollectionException(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <param name="olditem"></param>
        /// <returns></returns>
        public bool Update(T item, out T olditem) { throw new FixedSizeCollectionException(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool UpdateOrAdd(T item) { throw new FixedSizeCollectionException(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <param name="olditem"></param>
        /// <returns></returns>
        public bool UpdateOrAdd(T item, out T olditem) { throw new FixedSizeCollectionException(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item) { throw new FixedSizeCollectionException(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <param name="removeditem"></param>
        /// <returns></returns>
        public bool Remove(T item, out T removeditem) { throw new FixedSizeCollectionException(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        public void RemoveAllCopies(T item) { throw new FixedSizeCollectionException(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        public void RemoveAll(System.Collections.Generic.IEnumerable<T> items) { throw new FixedSizeCollectionException(); }

        /// <summary>
        ///
        /// </summary>
        public void Clear() { throw new FixedSizeCollectionException(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        public void RetainAll(System.Collections.Generic.IEnumerable<T> items) { throw new FixedSizeCollectionException(); }

        #endregion

        #region IExtensible<T> Members

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public bool IsReadOnly => true;

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public bool AllowsDuplicates => true;

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public SCG.IEqualityComparer<T> EqualityComparer => innerlist.EqualityComparer;

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public bool DuplicatesByCounting => false;

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(T item)
        {
            throw new FixedSizeCollectionException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        public void AddAll(System.Collections.Generic.IEnumerable<T> items)
        {
            throw new FixedSizeCollectionException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            return innerlist.Check() && (underlying == null || underlying.innerlist == innerlist.Underlying);
        }

        #endregion

        #region ICollectionValue<T> Members
        /// <summary>
        /// No listeners may be installed
        /// </summary>
        /// <value>0</value>
        public virtual EventType ListenableEvents => 0;

        /// <summary>
        /// No listeners ever installed
        /// </summary>
        /// <value>0</value>
        public virtual EventType ActiveEvents => 0;

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public event CollectionChangedHandler<T> CollectionChanged
        {
            add { throw new UnlistenableEventException(); }
            remove { throw new UnlistenableEventException(); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public event CollectionClearedHandler<T> CollectionCleared
        {
            add { throw new UnlistenableEventException(); }
            remove { throw new UnlistenableEventException(); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public event ItemsAddedHandler<T> ItemsAdded
        {
            add { throw new UnlistenableEventException(); }
            remove { throw new UnlistenableEventException(); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public event ItemInsertedHandler<T> ItemInserted
        {
            add { throw new UnlistenableEventException(); }
            remove { throw new UnlistenableEventException(); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public event ItemsRemovedHandler<T> ItemsRemoved
        {
            add { throw new UnlistenableEventException(); }
            remove { throw new UnlistenableEventException(); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public event ItemRemovedAtHandler<T> ItemRemovedAt
        {
            add { throw new UnlistenableEventException(); }
            remove { throw new UnlistenableEventException(); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public bool IsEmpty => innerlist.IsEmpty;

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public int Count => innerlist.Count;

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public Speed CountSpeed => innerlist.CountSpeed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(T[] array, int index) { innerlist.CopyTo(array, index); }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public T[] ToArray() { return innerlist.ToArray(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="action"></param>
        public void Apply(Action<T> action) { innerlist.Apply(action); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Exists(Func<T, bool> predicate) { return innerlist.Exists(predicate); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Find(Func<T, bool> predicate, out T item) { return innerlist.Find(predicate, out item); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool All(Func<T, bool> predicate) { return innerlist.All(predicate); }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public T Choose() { return innerlist.Choose(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public SCG.IEnumerable<T> Filter(Func<T, bool> filter) { return innerlist.Filter(filter); }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public SCG.IEnumerator<T> GetEnumerator() { return innerlist.GetEnumerator(); }
        #endregion

        #region IShowable Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="stringbuilder"></param>
        /// <param name="rest"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public bool Show(StringBuilder stringbuilder, ref int rest, IFormatProvider? formatProvider)
        { return innerlist.Show(stringbuilder, ref rest, formatProvider); }

        #endregion

        #region IFormattable Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return innerlist.ToString(); }


        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public virtual string ToString(string format, IFormatProvider formatProvider) { return innerlist.ToString(format, formatProvider); }

        #endregion

        #region IDirectedCollectionValue<T> Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IDirectedCollectionValue<T> Backwards() { return innerlist.Backwards(); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool FindLast(Func<T, bool> predicate, out T item) { return innerlist.FindLast(predicate, out item); }

        #endregion

        #region IDirectedEnumerable<T> Members

        IDirectedEnumerable<T> IDirectedEnumerable<T>.Backwards() { return Backwards(); }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public EnumerationDirection Direction => EnumerationDirection.Forwards;

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose this if a view else operation is illegal
        /// </summary>
        /// <exception cref="FixedSizeCollectionException">If not a view</exception>
        public void Dispose()
        {
            if (underlying == null)
            {
                throw new FixedSizeCollectionException();
            }
            else
            {
                innerlist.Dispose();
            }
        }

        #endregion

        #region System.Collections.Generic.IList<T> Members

        void System.Collections.Generic.IList<T>.RemoveAt(int index)
        {
            throw new FixedSizeCollectionException();
        }

        void System.Collections.Generic.ICollection<T>.Add(T item)
        {
            throw new FixedSizeCollectionException();
        }

        #endregion

        #region System.Collections.ICollection Members

        bool System.Collections.ICollection.IsSynchronized => false;

        [Obsolete]
        Object System.Collections.ICollection.SyncRoot => ((System.Collections.IList)innerlist).SyncRoot;

        void System.Collections.ICollection.CopyTo(Array arr, int index)
        {
            if (index < 0 || index + Count > arr.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            foreach (T item in this)
            {
                arr.SetValue(item, index++);
            }
        }

        #endregion

        #region System.Collections.IList Members

        Object System.Collections.IList.this[int index]
        {
            get => this[index]!;
            set => this[index] = (T)value;
        }

        int System.Collections.IList.Add(Object o)
        {
            bool added = Add((T)o);
            // What position to report if item not added? SC.IList.Add doesn't say
            return added ? Count - 1 : -1;
        }

        bool System.Collections.IList.Contains(Object o)
        {
            return Contains((T)o);
        }

        int System.Collections.IList.IndexOf(Object o)
        {
            return Math.Max(-1, IndexOf((T)o));
        }

        void System.Collections.IList.Insert(int index, Object o)
        {
            Insert(index, (T)o);
        }

        void System.Collections.IList.Remove(Object o)
        {
            Remove((T)o);
        }

        void System.Collections.IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}