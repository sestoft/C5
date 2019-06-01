using System;
using System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// A dictionary with keys of type K and values of type V. Equivalent to a
    /// finite partial map from K to V.
    /// </summary>
    public interface IDictionary<K, V> : ICollectionValue<KeyValuePair<K, V>>
    {
        /// <summary>
        /// The key equalityComparer.
        /// </summary>
        /// <value></value>
        System.Collections.Generic.IEqualityComparer<K> EqualityComparer { get; }

        /// <summary>
        /// Indexer for dictionary.
        /// </summary>
        /// <exception cref="NoSuchItemException"> if no entry is found. </exception>
        /// <value>The value corresponding to the key</value>
        V this[K key] { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <value>True if dictionary is read-only</value>
        bool IsReadOnly { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <value>A collection containing all the keys of the dictionary</value>
        ICollectionValue<K> Keys { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <value>A collection containing all the values of the dictionary</value>
        ICollectionValue<V> Values { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <value>A delegate of type <see cref="T:Func`2"/> defining the partial function from K to V give by the dictionary.</value>
        Func<K, V> Func { get; }


        //TODO: resolve inconsistency: Add thows exception if key already there, AddAll ignores keys already There?
        /// <summary>
        /// Add a new (key, value) pair (a mapping) to the dictionary.
        /// </summary>
        /// <exception cref="DuplicateNotAllowedException"> if there already is an entry with the same key. </exception>>
        /// <param name="key">Key to add</param>
        /// <param name="val">Value to add</param>
        void Add(K key, V val);

        /// <summary>
        /// Add the entries from a collection of <see cref="T:C5.KeyValuePair`2"/> pairs to this dictionary.
        /// </summary>
        /// <exception cref="DuplicateNotAllowedException"> 
        /// If the input contains duplicate keys or a key already present in this dictionary.</exception>
        /// <param name="entries"></param>
        void AddAll<U, W>(IEnumerable<KeyValuePair<U, W>> entries)
            where U : K
            where W : V
        ;

        /// <summary>
        /// The value is symbolic indicating the type of asymptotic complexity
        /// in terms of the size of this collection (worst-case or amortized as
        /// relevant). 
        /// <para>See <see cref="T:C5.Speed"/> for the set of symbols.</para>
        /// </summary>
        /// <value>A characterization of the speed of lookup operations
        /// (<code>Contains()</code> etc.) of the implementation of this dictionary.</value>
        Speed ContainsSpeed { get; }

        /// <summary>
        /// Check whether this collection contains all the values in another collection.
        /// If this collection has bag semantics (<code>AllowsDuplicates==true</code>)
        /// the check is made with respect to multiplicities, else multiplicities
        /// are not taken into account.
        /// </summary>
        /// <param name="items">The </param>
        /// <returns>True if all values in <code>items</code>is in this collection.</returns>
        bool ContainsAll<H>(IEnumerable<H> items) where H : K;

        /// <summary>
        /// Remove an entry with a given key from the dictionary
        /// </summary>
        /// <param name="key">The key of the entry to remove</param>
        /// <returns>True if an entry was found (and removed)</returns>
        bool Remove(K key);


        /// <summary>
        /// Remove an entry with a given key from the dictionary and report its value.
        /// </summary>
        /// <param name="key">The key of the entry to remove</param>
        /// <param name="val">On exit, the value of the removed entry</param>
        /// <returns>True if an entry was found (and removed)</returns>
        bool Remove(K key, out V val);


        /// <summary>
        /// Remove all entries from the dictionary
        /// </summary>
        void Clear();


        /// <summary>
        /// Check if there is an entry with a specified key
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <returns>True if key was found</returns>
        bool Contains(K key);

        /// <summary>
        /// Check if there is an entry with a specified key and report the corresponding
        /// value if found. This can be seen as a safe form of "val = this[key]".
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="val">On exit, the value of the entry</param>
        /// <returns>True if key was found</returns>
        bool Find(ref K key, out V val);


        /// <summary>
        /// Look for a specific key in the dictionary and if found replace the value with a new one.
        /// This can be seen as a non-adding version of "this[key] = val".
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="val">The new value</param>
        /// <returns>True if key was found</returns>
        bool Update(K key, V val);          //no-adding				    	


        /// <summary>
        /// Look for a specific key in the dictionary and if found replace the value with a new one.
        /// This can be seen as a non-adding version of "this[key] = val" reporting the old value.
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="val">The new value</param>
        /// <param name="oldval">The old value if any</param>
        /// <returns>True if key was found</returns>
        bool Update(K key, V val, out V oldval);          //no-adding				    	

        /// <summary>
        /// Look for a specific key in the dictionary. If found, report the corresponding value,
        /// else add an entry with the key and the supplied value.
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="val">On entry the value to add if the key is not found.
        /// On exit the value found if any.</param>
        /// <returns>True if key was found</returns>
        bool FindOrAdd(K key, ref V val);   //mixture


        /// <summary>
        /// Update value in dictionary corresponding to key if found, else add new entry.
        /// More general than "this[key] = val;" by reporting if key was found.
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="val">The value to add or replace with.</param>
        /// <returns>True if key was found and value updated.</returns>
        bool UpdateOrAdd(K key, V val);


        /// <summary>
        /// Update value in dictionary corresponding to key if found, else add new entry.
        /// More general than "this[key] = val;" by reporting if key was found.
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <param name="val">The value to add or replace with.</param>
        /// <param name="oldval">The old value if any</param>
        /// <returns>True if key was found and value updated.</returns>
        bool UpdateOrAdd(K key, V val, out V oldval);


        /// <summary>
        /// Check the integrity of the internal data structures of this dictionary.
        /// Only available in DEBUG builds???
        /// </summary>
        /// <returns>True if check does not fail.</returns>
        bool Check();
    }
}