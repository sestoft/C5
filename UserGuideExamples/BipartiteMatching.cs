/*
 Copyright (c) 2003-2006 Niels Kokholm and Peter Sestoft
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
*/

// C5 example: bipartite matching 2006-02-04

// Compile with 
//   csc /r:C5.dll BipartiteMatching.cs 

using System;
using System.Diagnostics;
// StreamReader, TextReader
// Encoding
// Regex
using C5;
using SCG = System.Collections.Generic;

namespace BipartiteMatching
{
  class MyTest
  {
    public static void Main(String[] args)
    {
      BipartiteMatching<int, char> bmi = new BipartiteMatching<int, char>(generateGraph(args));
      HashDictionary<int, char> res = new HashDictionary<int, char>();
      foreach (Rec<int,char> rec in bmi.Result)
      {
        res.Add(rec.X1, rec.X2);
      }
      for (int i = 0; i < args.Length; i++)
      {
        char c;
        if (!res.Find(i,out c))
          c='-';
        Console.WriteLine(@"""{0}"" -> '{1}'", args[i], c);
      }
    }

    static SCG.IEnumerable<Rec<int, char>> generateGraph(string[] words)
    {
      int i = 0;
      foreach (string s in words)
      {
        foreach (char c in s)
        {
          yield return new Rec<int, char>(i, c);
        }
        i++;
      }
    }

    /// <summary>
    /// Implements Hopcroft and Karps algorithm for Maximum bipartite matching.
    /// 
    /// Input (to the constructor): the edges of the graph as a collection of pairs 
    ///                             of labels of left and right nodes.
    /// 
    /// Output (from Result property): a maximum matching as a collection of pairs 
    ///                             of labels of left and right nodes.
    /// 
    /// The algorithm uses natural equality on the label types to identify nodes.
    /// 
    /// </summary>
    /// <typeparam name="TLeftLabel"></typeparam>
    /// <typeparam name="TRightLabel"></typeparam>
    public class BipartiteMatching<TLeftLabel, TRightLabel>
    {
      LeftNode[] leftNodes;
      RightNode[] rightNodes;

      class LeftNode
      {
        public TLeftLabel label;
        public RightNode match;
        public RightNode[] edges;
        public LeftNode(TLeftLabel label, params RightNode[] edges)
        {
          this.label = label;
          this.edges = (RightNode[])edges.Clone();
        }
        public override string ToString()
        {
          return string.Format(@"""{0}"" -> '{1}'", label, match);
        }
      }
      class RightNode
      {
        public TRightLabel label;
        public LeftNode match;
        public LeftNode backref;
        public LeftNode origin;
        public RightNode(TRightLabel label)
        {
          this.label = label;
        }
        public override string ToString()
        {
          return string.Format(@"'{0}'", label);
        }
      }
      public BipartiteMatching(SCG.IEnumerable<Rec<TLeftLabel, TRightLabel>> graph)
      {
        HashDictionary<TRightLabel, RightNode> rdict = new HashDictionary<TRightLabel, RightNode>();
        HashDictionary<TLeftLabel, HashSet<RightNode>> edges = new HashDictionary<TLeftLabel, HashSet<RightNode>>();
        HashSet<RightNode> newrnodes = new HashSet<RightNode>();
        foreach (Rec<TLeftLabel, TRightLabel> edge in graph)
        {
          RightNode rnode;
          if (!rdict.Find(edge.X2, out rnode))
            rdict.Add(edge.X2, rnode = new RightNode(edge.X2));

          HashSet<RightNode> ledges = newrnodes;
          if (!edges.FindOrAdd(edge.X1, ref ledges))
            newrnodes = new HashSet<RightNode>();
          ledges.Add(rnode);
        }

        rightNodes = rdict.Values.ToArray();

        leftNodes = new LeftNode[edges.Count];
        int li = 0;
        foreach (KeyValuePair<TLeftLabel, HashSet<RightNode>> les in edges)
        {
          leftNodes[li++] = new LeftNode(les.Key, les.Value.ToArray());
        }

        Compute();
      }

      public SCG.IEnumerable<Rec<TLeftLabel, TRightLabel>> Result
      {
        get
        {
          foreach (LeftNode l in leftNodes)
          {
            if (l.match != null)
            {
              yield return new Rec<TLeftLabel, TRightLabel>(l.label, l.match.label);
            }
          }
        }
      }

      HashSet<RightNode> endPoints;
      bool foundAugmentingPath;

      void Compute()
      {
        HashSet<LeftNode> unmatchedLeftNodes = new HashSet<LeftNode>();
        unmatchedLeftNodes.AddAll(leftNodes);
        foundAugmentingPath = true;
        Debug.Print("Compute start");
        while (foundAugmentingPath)
        {
          Debug.Print("Start outer");
          foundAugmentingPath = false;
          endPoints = new HashSet<RightNode>();
          foreach (RightNode rightNode in rightNodes)
            rightNode.backref = null;
          foreach (LeftNode l in unmatchedLeftNodes)
          {
            Debug.Print("Unmatched: {0}", l);
            search(l, l);
          }
          while (!foundAugmentingPath && endPoints.Count > 0)
          {
            HashSet<RightNode> oldLayer = endPoints;
            endPoints = new HashSet<RightNode>();
            foreach (RightNode rb in oldLayer)
              search(rb.match, rb.origin);
          }
          if (endPoints.Count == 0)
            return;
          //Flip
          Debug.Print("Flip");
          foreach (RightNode r in endPoints)
          {
            if (r.match == null && unmatchedLeftNodes.Contains(r.origin))
            {
              RightNode nextR = r;
              LeftNode nextL = null;
              while (nextR != null)
              {
                nextL = nextR.match = nextR.backref;
                RightNode rSwap = nextL.match;
                nextL.match = nextR;
                nextR = rSwap;
              }
              unmatchedLeftNodes.Remove(nextL);
            }
          }
        }
      }

      void search(LeftNode l, LeftNode origin)
      {
        foreach (RightNode r in l.edges)
        {
          if (r.backref == null)
          {
            r.backref = l;
            r.origin = origin;
            endPoints.Add(r);
            if (r.match == null)
              foundAugmentingPath = true;
            //First round should be greedy
            if (l == origin)
              return;
          }
        }
      }
    }
  }
}