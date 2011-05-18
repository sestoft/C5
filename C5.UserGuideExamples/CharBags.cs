// C5 example
// 2004-09-01

using System;
using C5;
using SCG = System.Collections.Generic;

class MyTest {
  static readonly IHasher<TreeBag<char>> sequencedTreeBagHasher
    = new C5.HasherBuilder.SequencedHasher<TreeBag<char>,char>();
  static readonly IHasher<TreeBag<char>> unsequencedTreeBagHasher
    = new C5.HasherBuilder.UnsequencedHasher<TreeBag<char>,char>();

  static readonly IHasher<HashBag<char>> unsequencedHashBagHasher
    = new C5.HasherBuilder.UnsequencedHasher<HashBag<char>,char>();

  public static void Main(String[] args) {
  }

  public static void FindCollisions(SCG.IEnumerable<String> ss) {
    HashBag<int> occurrences = new HashBag<int>();
    foreach (String s in ss) {
      TreeBag<char> tb = TreeBag(s);
      // HashBag<char> hb = HashBag(s);
      occurrences.Add(sequencedTreeBagHasher.GetHashCode(tb));
      // unsequencedTreeBagHasher.GetHashCode(tb);
      // unsequencedHashBagHasher.GetHashCode(hb);
    }
    
  }
  
  public static TreeBag<char> TreeBag(String s) { 
    TreeBag<char> anagram = new TreeBag<char>();
    foreach (char c in s) 
      anagram.Add(c);
    return anagram;
  }

  public static HashBag<char> HashBag(String s) { 
    HashBag<char> anagram = new HashBag<char>();
    foreach (char c in s) 
      anagram.Add(c);
    return anagram;
  }
}


