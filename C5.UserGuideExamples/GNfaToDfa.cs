// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 examples: RegExp -> NFA -> DFA -> Graph
// Java 2000-10-07, GC# 2001-10-23, C# 2.0 2003-09-03, C# 2.0+C5 2004-08-08

// This file contains, in order:
//   * Helper class HashSet<T> defined in terms of C5 classes.
//   * A class Nfa for representing an NFA (a nondeterministic finite 
//     automaton), and for converting it to a DFA (a deterministic 
//     finite automaton).  Most complexity is in this class.
//   * A class Dfa for representing a DFA, a deterministic finite 
//     automaton, and for writing a dot input file representing the DFA.
//   * Classes for representing regular expressions, and for building an 
//     NFA from a regular expression
//   * A test class that creates an NFA, a DFA, and a dot input file 
//     for a number of small regular expressions.  The DFAs are 
//     not minimized.

// Compile with
//   csc /r:netstandard.dll /r:C5.dll GNfaToDfa.cs

using System;
using System.IO;

namespace C5.UserGuideExamples
{
    // ----------------------------------------------------------------------

    // Regular expressions, NFAs, DFAs, and dot graphs
    // sestoft@dina.kvl.dk * 
    // Java 2001-07-10 * C# 2001-10-22 * Gen C# 2001-10-23, 2003-09-03

    // In the Generic C# 2.0 version we 
    //  use Queue<int> and Queue<HashSet<int>> for worklists
    //  use HashSet<int> for pre-DFA states
    //  use ArrayList<Transition> for NFA transition relations
    //  use HashDictionary<HashSet<int>, HashDictionary<string, HashSet<int>>>
    //  and HashDictionary<int, HashDictionary<string, int>> for DFA transition relations

    /* Class Nfa and conversion from NFA to DFA ---------------------------

      A nondeterministic finite automaton (NFA) is represented as a
      dictionary mapping a state number (int) to an arraylist of
      Transitions, a Transition being a pair of a label lab (a string,
      null meaning epsilon) and a target state (an int).

      A DFA is created from an NFA in two steps:

        (1) Construct a DFA whose each of whose states is composite,
            namely a set of NFA states (HashSet of int).  This is done by
            methods CompositeDfaTrans and EpsilonClose.

        (2) Replace composite states (HashSet of int) by simple states
            (int).  This is done by methods Rename and MkRenamer.

      Method CompositeDfaTrans works as follows: 

        Create the epsilon-closure S0 (a HashSet of ints) of the start state
        s0, and put it in a worklist (a Queue).  Create an empty DFA
        transition relation, which is a dictionary mapping a composite
        state (an epsilon-closed set of ints) to a dictionary mapping a
        label (a non-null string) to a composite state.

        Repeatedly choose a composite state S from the worklist.  If it is
        not already in the keyset of the DFA transition relation, compute
        for every non-epsilon label lab the set T of states reachable by
        that label from some state s in S.  Compute the epsilon-closure
        Tclose of every such state T and put it on the worklist.  Then add
        the transition S -lab-> Tclose to the DFA transition relation, for
        every lab.

      Method EpsilonClose works as follows: 

        Given a set S of states.  Put the states of S in a worklist.
        Repeatedly choose a state s from the worklist, and consider all
        epsilon-transitions s -eps-> s' from s.  If s' is in S already,
        then do nothing; otherwise add s' to S and the worklist.  When the
        worklist is empty, S is epsilon-closed; return S.

      Method MkRenamer works as follows: 

        Given a dictionary mapping a set of int to something, create an
        injective dictionary mapping from set of int to int, by choosing a
        fresh int for every key in the given dictionary.

      Method Rename works as follows:

        Given a dictionary mapping a set of int to a dictionary mapping a
        string to set of int, use the result of MkRenamer to replace all
        sets of ints by ints.

    */

    internal class Nfa
    {
        public int Start { get; }

        public int Exit { get; }

        public IDictionary<int, ArrayList<Transition>> Trans { get; }

        public Nfa(int startState, int exitState)
        {
            Start = startState;
            Exit = exitState;
            Trans = new HashDictionary<int, ArrayList<Transition>>();
            if (!startState.Equals(exitState))
            {
                Trans.Add(exitState, new ArrayList<Transition>());
            }
        }

        public void AddTrans(int s1, string lab, int s2)
        {
            ArrayList<Transition> s1Trans;
            if (Trans.Contains(s1))
            {
                s1Trans = Trans[s1];
            }
            else
            {
                s1Trans = new ArrayList<Transition>();
                Trans.Add(s1, s1Trans);
            }
            s1Trans.Add(new Transition(lab, s2));
        }

        public void AddTrans(System.Collections.Generic.KeyValuePair<int, ArrayList<Transition>> tr)
        {
            // Assumption: if tr is in trans, it maps to an empty list (end state)
            Trans.Remove(tr.Key);
            Trans.Add(tr.Key, tr.Value);
        }

        public override string ToString()
        {
            return $"NFA start={Start} exit={Exit}";
        }

        // Construct the transition relation of a composite-state DFA from
        // an NFA with start state s0 and transition relation trans (a
        // dictionary mapping int to arraylist of Transition).  The start
        // state of the constructed DFA is the epsilon closure of s0, and
        // its transition relation is a dictionary mapping a composite state
        // (a set of ints) to a dictionary mapping a label (a string) to a
        // composite state (a set of ints).

        private static IDictionary<HashSet<int>, IDictionary<string, HashSet<int>>> CompositeDfaTrans(int s0, IDictionary<int, ArrayList<Transition>> trans)
        {
            var S0 = EpsilonClose(new HashSet<int> { s0 }, trans);
            var worklist = new CircularQueue<HashSet<int>>();
            worklist.Enqueue(S0);

            // The transition relation of the DFA
            var res = new HashDictionary<HashSet<int>, IDictionary<string, HashSet<int>>>();

            while (!worklist.IsEmpty)
            {
                HashSet<int> S = worklist.Dequeue();
                if (!res.Contains(S))
                {
                    // The S -lab-> T transition relation being constructed for a given S
                    IDictionary<string, HashSet<int>> STrans =
                      new HashDictionary<string, HashSet<int>>();
                    // For all s in S, consider all transitions s -lab-> t
                    foreach (int s in S)
                    {
                        // For all non-epsilon transitions s -lab-> t, add t to T
                        foreach (Transition tr in trans[s])
                        {
                            if (tr.Lab != null)
                            {
                                // Non-epsilon transition
                                HashSet<int> toState;
                                if (STrans.Contains(tr.Lab)) // Already a transition on lab
                                {
                                    toState = STrans[tr.Lab];
                                }
                                else // No transitions on lab yet
                                {
                                    toState = new HashSet<int>();
                                    STrans.Add(tr.Lab, toState);
                                }
                                toState.Add(tr.Target);
                            }
                        }
                    }

                    // Epsilon-close all T such that S -lab-> T, and put on worklist
                    var STransClosed = new HashDictionary<string, HashSet<int>>();
                    foreach (var entry in STrans)
                    {
                        var Tclose = EpsilonClose(entry.Value, trans);
                        STransClosed.Add(entry.Key, Tclose);
                        worklist.Enqueue(Tclose);
                    }
                    res.Add(S, STransClosed);
                }
            }
            return res;
        }

        // Compute epsilon-closure of state set S in transition relation trans.  
        private static HashSet<int> EpsilonClose(HashSet<int> set, IDictionary<int, ArrayList<Transition>> trans)
        {
            // The worklist initially contains all S members
            var worklist = new CircularQueue<int>();
            set.Apply(worklist.Enqueue);
            var res = new HashSet<int>();
            res.AddAll(set);

            while (!worklist.IsEmpty)
            {
                var s = worklist.Dequeue();
                foreach (var tr in trans[s])
                {
                    if (tr.Lab == null && !res.Contains(tr.Target))
                    {
                        res.Add(tr.Target);
                        worklist.Enqueue(tr.Target);
                    }
                }
            }
            return res;
        }

        // Compute a renamer, which is a dictionary mapping set of int to int
        private static IDictionary<HashSet<int>, int> MkRenamer(ICollectionValue<HashSet<int>> states)
        {
            var renamer = new HashDictionary<HashSet<int>, int>();
            var count = 0;
            foreach (var k in states)
            {
                renamer.Add(k, count++);
            }

            return renamer;
        }

        // Using a renamer (a dictionary mapping set of int to int), replace
        // composite (set of int) states with simple (int) states in the
        // transition relation trans, which is a dictionary mapping set of
        // int to a dictionary mapping from string to set of int.  The
        // result is a dictionary mapping from int to a dictionary mapping
        // from string to int.

        private static IDictionary<int, IDictionary<string, int>> Rename(IDictionary<HashSet<int>, int> renamer, IDictionary<HashSet<int>, IDictionary<string, HashSet<int>>> trans)
        {
            var newtrans = new HashDictionary<int, IDictionary<string, int>>();
            foreach (var entry in trans)
            {
                var k = entry.Key;
                var newktrans = new HashDictionary<string, int>();
                foreach (var tr in entry.Value)
                {
                    newktrans.Add(tr.Key, renamer[tr.Value]);
                }

                newtrans.Add(renamer[k], newktrans);
            }
            return newtrans;
        }

        private static HashSet<int> AcceptStates(ICollectionValue<HashSet<int>> states, IDictionary<HashSet<int>, int> renamer, int exit)
        {
            var acceptStates = new HashSet<int>();

            foreach (var state in states)
            {
                if (state.Contains(exit))
                {
                    acceptStates.Add(renamer[state]);
                }
            }

            return acceptStates;
        }

        public Dfa ToDfa()
        {
            var cDfaTrans = CompositeDfaTrans(Start, Trans);
            var cDfaStart = EpsilonClose(new HashSet<int> { Start }, Trans);
            var cDfaStates = cDfaTrans.Keys;
            var renamer = MkRenamer(cDfaStates);
            var simpleDfaTrans = Rename(renamer, cDfaTrans);
            var simpleDfaStart = renamer[cDfaStart];
            var simpleDfaAccept = AcceptStates(cDfaStates, renamer, Exit);

            return new Dfa(simpleDfaStart, simpleDfaAccept, simpleDfaTrans);
        }

        // Nested class for creating distinctly named states when constructing NFAs
        public class NameSource
        {
            private static int _nextName = 0;

            public int Next()
            {
                return _nextName++;
            }
        }

        // Write an input file for the dot program.  You can find dot at
        // http://www.research.att.com/sw/tools/graphviz/
        public void WriteDot(string filename)
        {
            using (var writer = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)))
            {
                writer.WriteLine("// Format this file as a Postscript file with ");
                writer.WriteLine("//    dot " + filename + " -Tps -o out.ps\n");
                writer.WriteLine("digraph nfa {");
                writer.WriteLine("size=\"11,8.25\";");
                writer.WriteLine("rotate=90;");
                writer.WriteLine("rankdir=LR;");
                writer.WriteLine("start [style=invis];");    // Invisible start node
                writer.WriteLine("start -> d" + Start); // Edge into start state

                // The accept state has a double circle
                writer.WriteLine("d" + Exit + " [peripheries=2];");

                // The transitions 
                foreach (var entry in Trans)
                {
                    var s1 = entry.Key;
                    for (var i = 0; i < entry.Value.Count; i++)
                    {
                        var s1Trans = entry.Value[i];
                        var lab = s1Trans.Lab ?? "eps";
                        var s2 = s1Trans.Target;
                        writer.WriteLine("d" + s1 + " -> d" + s2 + " [label=\"" + lab + "\"];");
                    }
                }
                writer.WriteLine("}");
            }
        }
    }

    // Class Transition, a transition from one state to another ----------
    public class Transition
    {
        public string Lab { get; }
        public int Target { get; }

        public Transition(string lab, int target)
        {
            Lab = lab;
            Target = target;
        }

        public override string ToString()
        {
            return $"-{Lab}-> {Target}";
        }
    }

    // Class Dfa, deterministic finite automata --------------------------

    /*
       A deterministic finite automaton (DFA) is represented as a
       dictionary mapping state number (int) to a dictionary mapping label
       (a non-null string) to a target state (an int).
    */
    internal class Dfa
    {
        public int Start { get; }
        public HashSet<int> Accept { get; }
        public IDictionary<int, IDictionary<string, int>> Trans { get; }

        public Dfa(int startState, HashSet<int> acceptStates,
               IDictionary<int, IDictionary<string, int>> trans)
        {
            Start = startState;
            Accept = acceptStates;
            Trans = trans;
        }

        public override string ToString()
        {
            return $"DFA start={Start}{Environment.NewLine}accept={Accept}";
        }

        // Write an input file for the dot program.  You can find dot at
        // http://www.research.att.com/sw/tools/graphviz/
        public void WriteDot(string filename)
        {
            using (var writer = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)))
            {
                writer.WriteLine("// Format this file as a Postscript file with ");
                writer.WriteLine("//    dot " + filename + " -Tps -o out.ps\n");
                writer.WriteLine("digraph dfa {");
                writer.WriteLine("size=\"11,8.25\";");
                writer.WriteLine("rotate=90;");
                writer.WriteLine("rankdir=LR;");
                writer.WriteLine("start [style=invis];");    // Invisible start node
                writer.WriteLine("start -> d" + Start); // Edge into start state

                // Accept states are double circles
                foreach (var state in Trans.Keys)
                {
                    if (Accept.Contains(state))
                    {
                        writer.WriteLine("d" + state + " [peripheries=2];");
                    }
                }

                // The transitions 
                foreach (var entry in Trans)
                {
                    var s1 = entry.Key;
                    foreach (var s1Trans in entry.Value)
                    {
                        var lab = s1Trans.Key;
                        var s2 = s1Trans.Value;
                        writer.WriteLine($"d{s1} -> d{s2} [label=\"{lab}\"];");
                    }
                }
                writer.WriteLine("}");
            }
        }
    }

    // Regular expressions ----------------------------------------------
    //
    // Abstract syntax of regular expressions
    //    r ::= A | r1 r2 | (r1|r2) | r*
    //

    internal abstract class RegexBase
    {
        public abstract Nfa MkNfa(Nfa.NameSource names);
    }

    internal class Eps : RegexBase
    {
        // The resulting nfa0 has form s0s -eps-> s0e

        public override Nfa MkNfa(Nfa.NameSource names)
        {
            var s0s = names.Next();
            var s0e = names.Next();
            var nfa0 = new Nfa(s0s, s0e);
            nfa0.AddTrans(s0s, null, s0e);
            return nfa0;
        }
    }

    internal class Sym : RegexBase
    {
        private readonly string _sym;

        public Sym(string sym)
        {
            _sym = sym;
        }

        // The resulting nfa0 has form s0s -sym-> s0e
        public override Nfa MkNfa(Nfa.NameSource names)
        {
            var s0s = names.Next();
            var s0e = names.Next();
            var nfa0 = new Nfa(s0s, s0e);
            nfa0.AddTrans(s0s, _sym, s0e);
            return nfa0;
        }
    }

    internal class Seq : RegexBase
    {
        private readonly RegexBase _r1;
        private readonly RegexBase _r2;

        public Seq(RegexBase r1, RegexBase r2)
        {
            _r1 = r1;
            _r2 = r2;
        }

        // If   nfa1 has form s1s ----> s1e 
        // and  nfa2 has form s2s ----> s2e 
        // then nfa0 has form s1s ----> s1e -eps-> s2s ----> s2e
        public override Nfa MkNfa(Nfa.NameSource names)
        {
            var nfa1 = _r1.MkNfa(names);
            var nfa2 = _r2.MkNfa(names);
            var nfa0 = new Nfa(nfa1.Start, nfa2.Exit);

            foreach (var entry in nfa1.Trans)
            {
                nfa0.AddTrans(entry);
            }

            foreach (var entry in nfa2.Trans)
            {
                nfa0.AddTrans(entry);
            }

            nfa0.AddTrans(nfa1.Exit, null, nfa2.Start);

            return nfa0;
        }
    }

    internal class Alt : RegexBase
    {
        private readonly RegexBase _r1;
        private readonly RegexBase _r2;

        public Alt(RegexBase r1, RegexBase r2)
        {
            _r1 = r1;
            _r2 = r2;
        }

        // If   nfa1 has form s1s ----> s1e 
        // and  nfa2 has form s2s ----> s2e 
        // then nfa0 has form s0s -eps-> s1s ----> s1e -eps-> s0e
        //                    s0s -eps-> s2s ----> s2e -eps-> s0e
        public override Nfa MkNfa(Nfa.NameSource names)
        {
            var nfa1 = _r1.MkNfa(names);
            var nfa2 = _r2.MkNfa(names);
            var s0s = names.Next();
            var s0e = names.Next();
            var nfa0 = new Nfa(s0s, s0e);

            foreach (var entry in nfa1.Trans)
            {
                nfa0.AddTrans(entry);
            }

            foreach (var entry in nfa2.Trans)
            {
                nfa0.AddTrans(entry);
            }

            nfa0.AddTrans(s0s, null, nfa1.Start);
            nfa0.AddTrans(s0s, null, nfa2.Start);
            nfa0.AddTrans(nfa1.Exit, null, s0e);
            nfa0.AddTrans(nfa2.Exit, null, s0e);

            return nfa0;
        }
    }

    internal class Star : RegexBase
    {
        private readonly RegexBase _r;

        public Star(RegexBase r)
        {
            _r = r;
        }

        // If   nfa1 has form s1s ----> s1e 
        // then nfa0 has form s0s ----> s0s
        //                    s0s -eps-> s1s
        //                    s1e -eps-> s0s
        public override Nfa MkNfa(Nfa.NameSource names)
        {
            var nfa1 = _r.MkNfa(names);
            var s0s = names.Next();
            var nfa0 = new Nfa(s0s, s0s);

            foreach (var entry in nfa1.Trans)
            {
                nfa0.AddTrans(entry);
            }

            nfa0.AddTrans(s0s, null, nfa1.Start);
            nfa0.AddTrans(nfa1.Exit, null, s0s);

            return nfa0;
        }
    }

    // Trying the RE->NFA->DFA translation on three regular expressions
    internal class GNfaToDfa
    {
        public static void Main()
        {
            var a = new Sym("A");
            var b = new Sym("B");
            _ = new Sym("C");
            var abStar = new Star(new Alt(a, b));
            var bb = new Seq(b, b);
            var r = new Seq(abStar, new Seq(a, b));

            // The regular expression (a|b)*ab
            BuildAndShow("ex1", r);

            // The regular expression ((a|b)*ab)*
            BuildAndShow("ex2", new Star(r));

            // The regular expression ((a|b)*ab)((a|b)*ab)
            BuildAndShow("ex3", new Seq(r, r));

            // The regular expression (a|b)*abb, from ASU 1986 p 136
            BuildAndShow("ex4", new Seq(abStar, new Seq(a, bb)));

            // SML reals: sign?((digit+(\.digit+)?))([eE]sign?digit+)?
            var d = new Sym("digit");
            var dPlus = new Seq(d, new Star(d));
            var s = new Sym("sign");
            var sOpt = new Alt(s, new Eps());
            var dot = new Sym(".");
            var dotDigOpt = new Alt(new Eps(), new Seq(dot, dPlus));
            var mant = new Seq(sOpt, new Seq(dPlus, dotDigOpt));
            var e = new Sym("e");
            var exp = new Alt(new Eps(), new Seq(e, new Seq(sOpt, dPlus)));
            var smlReal = new Seq(mant, exp);
            BuildAndShow("ex5", smlReal);
        }

        public static void BuildAndShow(string fileprefix, RegexBase r)
        {
            var nfa = r.MkNfa(new Nfa.NameSource());
            Console.WriteLine(nfa);
            Console.WriteLine("Writing NFA graph to file");
            nfa.WriteDot(fileprefix + "nfa.dot");
            Console.WriteLine("---");
            var dfa = nfa.ToDfa();
            Console.WriteLine(dfa);
            Console.WriteLine("Writing DFA graph to file");
            dfa.WriteDot(fileprefix + "dfa.dot");
            Console.WriteLine();
        }
    }
}
