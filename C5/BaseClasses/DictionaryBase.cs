using System;
using System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// A base class for implementing a dictionary based on a set collection implementation.
    /// <i>See the source code for <see cref="T:C5.HashDictionary`2"/> for an example</i>
    /// 
    /// </summary>
    [Serializable]
    public abstract class DictionaryBase<K, V> : CollectionValueBase<KeyValuePair<K, V>>, IDictionary<K, V>
    {
        /// <summary>
        /// The set collection of entries underlying this dictionary implementation
        /// </summary>
        protected ICollection<KeyValuePair<K, V>> pairs;
        private readonly System.Collections.Generic.IEqualityComparer<K> keyequalityComparer;

        #region Events
        private ProxyEventBlock<KeyValuePair<K, V>>? eventBlock;

        /// <summary>
        /// The change event. Will be raised for every change operation on the collection.
        /// </summary>
        public override event CollectionChangedHandler<KeyValuePair<K, V>> CollectionChanged
        {
            add { (eventBlock ?? (eventBlock = new ProxyEventBlock<KeyValuePair<K, V>>(this, pairs))).CollectionChanged += value; }
            remove
            {
                if (eventBlock != null)
                {
                    eventBlock.CollectionChanged -= value;
                }
            }
        }

        /// <summary>
        /// The change event. Will be raised for every change operation on the collection.
        /// </summary>
        public override event CollectionClearedHandler<KeyValuePair<K, V>> CollectionCleared
        {
            add { (eventBlock ?? (eventBlock = new ProxyEventBlock<KeyValuePair<K, V>>(this, pairs))).CollectionCleared += value; }
            remove
            {
                if (eventBlock != null)
                {
                    eventBlock.CollectionCleared -= value;
                }
            }
        }

        /// <summary>
        /// The item added  event. Will be raised for every individual addition to the collection.
        /// </summary>
        public override event ItemsAddedHandler<KeyValuePair<K, V>> ItemsAdded
        {
            add { (eventBlock ?? (eventBlock = new ProxyEventBlock<KeyValuePair<K, V>>(this, pairs))).ItemsAdded += value; }
            remove
            {
                if (eventBlock != null)
                {
                    eventBlock.ItemsAdded -= value;
                }
            }
        }

        /// <summary>
        /// The item added  event. Will be raised for every individual removal from the collection.
        /// </summary>
        public override event ItemsRemovedHandler<KeyValuePair<K, V>> ItemsRemoved
        {
            add { (eventBlock ?? (eventBlock = new ProxyEventBlock<KeyValuePair<K, V>>(this, pairs))).ItemsRemoved += value; }
            remove
            {
                if (eventBlock != null)
                {
                    eventBlock.ItemsRemoved -= value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override EventType ListenableEvents => EventType.Basic;

        /// <summary>
        /// 
        /// </summary>
        public override EventType ActiveEvents => pairs.ActiveEvents;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyequalityComparer"></param>
        protected DictionaryBase(System.Collections.Generic.IEqualityComparer<K> keyequalityComparer)
        {
            this.keyequalityComparer = keyequalityComparer ?? throw new NullReferenceException("Key equality comparer cannot be null");
        }

        #region IDictionary<K,V> Members

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual System.Collections.Generic.IEqualityComparer<K> EqualityComparer => keyequalityComparer;


        /// <summary>
        /// Add a new (key, value) pair (a mapping) to the dictionary.
        /// </summary>
        /// <exception cref="DuplicateNotAllowedException"> if there already is an entry with the same key. </exception>
        /// <param name="key">Key to add</param>
        /// <param name="value">Value to add</param>
        public virtual void Add(K key, V value)
        {
            KeyValuePair<K, V> p = new KeyValuePair<K, V>(key, value);

            if (!pairs.Add(p))
            {
                throw new DuplicateNotAllowedException("Key being added: '" + key + "'");
            }
        }

        /// <summary>
        /// Add the entries from a collection of <see cref="T:C5.KeyValuePair`2"/> pairs to this dictionary.
        /// <para><b>TODO: add restrictions L:K and W:V when the .Net SDK allows it </b></para>
        /// </summary>
        /// <exception cref="DuplicateNotAllowedException"> 
        /// If the input contains duplicate keys or a key already present in this dictionary.</exception>
        /// <param name="entries"></param>
        public virtual void AddAll<L, W>(System.Collections.Generic.IEnumerable<KeyValuePair<L, W>> entries)
            where L : K
            where W : V
        {
            foreach (KeyValuePair<L, W> pair in entries)
            {
                KeyValuePair<K, V> p = new KeyValuePair<K, V>(pair.Key, pair.Value);
                if (!pairs.Add(p))
                {
                    throw new DuplicateNotAllowedException("Key being added: '" + pair.Key + "'");
                }
            }
        }

        /// <summary>
        /// Remove an entry with a given key from the dictionary
        /// </summary>
        /// <param name="key">The key of the entry to remove</param>
        /// <returns>True if an entry was found (and removed)</returns>
        public virtual bool Remove(K key)
        {
            KeyValuePair<K, V> p = new KeyValuePair<K, V>(key);

            return pairs.Remove(p);
        }


        /// <summary>
        /// Remove an entry with a given key from the dictionary and report its value.
        /// </summary>
        /// <param name="key">The key of the entry to remove</param>
        /// <param name="value">On exit, the value of the removed entry</param>
        /// <returns>True if an entry was found (and removed)</returns>
        public virtual bool Remove(K key, out V value)
        {
            KeyValuePair<K, V> p = new KeyValuePair<K, V>(key);

            if (pairs.Remove(p, out p))
            {
                value = p.Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }


        /// <summary>
        /// Remove all entries from the dictionary
        /// </summary>
        public virtual void Clear() { pairs.Clear(); }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public virtual Speed ContainsSpeed => pairs.ContainsSpeed;

        /// <summary>
        /// Check if there is an entry with a specified key
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <returns>True if key was found</returns>
        public virtual bool Contains(K key)
        {
            KeyValuePair<K, V> p = new KeyValuePair<K, V>(key);

            return pairs.Contains(p);
        }

        [Serializable]
        private class LiftedEnumerable<H> : IEnumerable<KeyValuePair<K, V>> where H : K
        {
            private readonly System.Collections.Generic.IEnumerable<H> keys;
            public LiftedEnumerable(System.Collections.Generic.IEnumerable<H> keys) { this.keys = keys; }
            public System.Collections.Generic.IEnumerator<KeyValuePair<K, V>> GetEnumerator()
            {
                foreach (H key in keys)
                {
                    yield return new KeyValuePair<K, V>(key);
                }
            }

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual bool ContainsAll<H>(System.Collections.Generic.IEnumerable<H> keys) where H : K
        {
            return pairs.ContainsAll(new LiftedEnumerable<H>(keys));
        }

        /// <summary>
        /// Check if there is an entry with a specified key and report the corresponding
        /// value if found. This can be seen as a safe form of "val = this[key]".
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="value">On exit, the value of the entry</param>
        /// <returns>True if key was found</returns>
        public virtual bool Find(ref K key, out V value)
        {
            KeyValuePair<K, V> p = new KeyValuePair<K, V>(key);

            if (pairs.Find(ref p))
            {
                key = p.Key;
                value = p.Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }


        /// <summary>
        /// Look for a specific key in the dictionary and if found replace the value with a new one.
        /// This can be seen as a non-adding version of "this[key] = val".
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="value">The new value</param>
        /// <returns>True if key was found</returns>
        public virtual bool Update(K key, V value)
        {
            KeyValuePair<K, V> p = new KeyValuePair<K, V>(key, value);

            return pairs.Update(p);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="oldvalue"></param>
        /// <returns></returns>
        public virtual bool Update(K key, V value, out V oldvalue)
        {
            KeyValuePair<K, V> p = new KeyValuePair<K, V>(key, value);

            bool retval = pairs.Update(p, out p);
            oldvalue = p.Value;
            return retval;
        }


        /// <summary>
        /// Look for a specific key in the dictionary. If found, report the corresponding value,
        /// else add an entry with the key and the supplied value.
        /// </summary>
        /// <param name="key">On entry the key to look for</param>
        /// <param name="value">On entry the value to add if the key is not found.
        /// On exit the value found if any.</param>
        /// <returns>True if key was found</returns>
        public virtual bool FindOrAdd(K key, ref V value)
        {
            KeyValuePair<K, V> p = new KeyValuePair<K, V>(key, value);

            if (!pairs.FindOrAdd(ref p))
            {
                return false;
            }
            else
            {
                value = p.Value;
                //key = p.key;
                return true;
            }
        }


        /// <summary>
        /// Update value in dictionary corresponding to key if found, else add new entry.
        /// More general than "this[key] = val;" by reporting if key was found.
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="value">The value to add or replace with.</param>
        /// <returns>True if entry was updated.</returns>
        public virtual bool UpdateOrAdd(K key, V value)
        {
            return pairs.UpdateOrAdd(new KeyValuePair<K, V>(key, value));
        }


        /// <summary>
        /// Update value in dictionary corresponding to key if found, else add new entry.
        /// More general than "this[key] = val;" by reporting if key was found and the old value if any.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="oldvalue"></param>
        /// <returns></returns>
        public virtual bool UpdateOrAdd(K key, V value, out V oldvalue)
        {
            KeyValuePair<K, V> p = new KeyValuePair<K, V>(key, value);
            bool retval = pairs.UpdateOrAdd(p, out p);
            oldvalue = p.Value;
            return retval;
        }



        #region Keys,Values support classes
        [Serializable]
        internal class ValuesCollection : CollectionValueBase<V>, ICollectionValue<V>
        {
            private readonly ICollection<KeyValuePair<K, V>> pairs;


            internal ValuesCollection(ICollection<KeyValuePair<K, V>> pairs)
            { this.pairs = pairs; }


            public override V Choose() { return pairs.Choose().Value; }

            public override System.Collections.Generic.IEnumerator<V> GetEnumerator()
            {
                //Updatecheck is performed by the pairs enumerator
                foreach (KeyValuePair<K, V> p in pairs)
                {
                    yield return p.Value;
                }
            }

            public override bool IsEmpty => pairs.IsEmpty;

            public override int Count => pairs.Count;

            public override Speed CountSpeed => Speed.Constant;
        }

        [Serializable]
        internal class KeysCollection : CollectionValueBase<K>, ICollectionValue<K>
        {
            private readonly ICollection<KeyValuePair<K, V>> pairs;


            internal KeysCollection(ICollection<KeyValuePair<K, V>> pairs)
            { this.pairs = pairs; }

            public override K Choose() { return pairs.Choose().Key; }

            public override System.Collections.Generic.IEnumerator<K> GetEnumerator()
            {
                foreach (KeyValuePair<K, V> p in pairs)
                {
                    yield return p.Key;
                }
            }

            public override bool IsEmpty => pairs.IsEmpty;

            public override int Count => pairs.Count;

            public override Speed CountSpeed => pairs.CountSpeed;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <value>A collection containing all the keys of the dictionary</value>
        public virtual ICollectionValue<K> Keys => new KeysCollection(pairs);


        /// <summary>
        /// 
        /// </summary>
        /// <value>A collection containing all the values of the dictionary</value>
        public virtual ICollectionValue<V> Values => new ValuesCollection(pairs);

        /// <summary>
        /// 
        /// </summary>
        public virtual Func<K, V> Func => delegate (K k) { return this[k]; };

        /// <summary>
        /// Indexer by key for dictionary. 
        /// <para>The get method will throw an exception if no entry is found. </para>
        /// <para>The set method behaves like <see cref="M:C5.DictionaryBase`2.UpdateOrAdd(`0,`1)"/>.</para>
        /// </summary>
        /// <exception cref="NoSuchItemException"> On get if no entry is found. </exception>
        /// <value>The value corresponding to the key</value>
        public virtual V this[K key]
        {
            get
            {
                KeyValuePair<K, V> p = new KeyValuePair<K, V>(key);

                if (pairs.Find(ref p))
                {
                    return p.Value;
                }
                else
                {
                    throw new NoSuchItemException("Key '" + key!.ToString() + "' not present in Dictionary");
                }
            }
            set => pairs.UpdateOrAdd(new KeyValuePair<K, V>(key, value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <value>True if dictionary is read  only</value>
        public virtual bool IsReadOnly => pairs.IsReadOnly;


        /// <summary>
        /// Check the integrity of the internal data structures of this dictionary.
        /// </summary>
        /// <returns>True if check does not fail.</returns>
        public virtual bool Check() { return pairs.Check(); }

        #endregion

        #region ICollectionValue<KeyValuePair<K,V>> Members

        /// <summary>
        /// 
        /// </summary>
        /// <value>True if this collection is empty.</value>
        public override bool IsEmpty => pairs.IsEmpty;


        /// <summary>
        /// 
        /// </summary>
        /// <value>The number of entries in the dictionary</value>
        public override int Count => pairs.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <value>The number of entries in the dictionary</value>
        public override Speed CountSpeed => pairs.CountSpeed;

        /// <summary>
        /// Choose some entry in this Dictionary. 
        /// </summary>
        /// <exception cref="NoSuchItemException">if collection is empty.</exception>
        /// <returns></returns>
        public override KeyValuePair<K, V> Choose() { return pairs.Choose(); }

        /// <summary>
        /// Create an enumerator for the collection of entries of the dictionary
        /// </summary>
        /// <returns>The enumerator</returns>
        public override System.Collections.Generic.IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return pairs.GetEnumerator(); ;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringbuilder"></param>
        /// <param name="rest"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public override bool Show(System.Text.StringBuilder stringbuilder, ref int rest, IFormatProvider? formatProvider)
        {
            return Showing.ShowDictionary<K, V>(this, stringbuilder, ref rest, formatProvider);
        }
    }
}