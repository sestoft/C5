// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

// C5 example: bipartite matching 2006-02-04

namespace C5.UserGuideExamples;

internal class BipartiteMatchingProgram
{
    private static void Main(string[] args)
    {
        var bmi = new BipartiteMatching<int, char>(GenerateGraph(args));
        var res = new HashDictionary<int, char>();
        foreach (var rec in bmi.Result)
        {
            res.Add(rec.Item1, rec.Item2);
        }
        for (var i = 0; i < args.Length; i++)
        {
            var j = i;
            if (!res.Find(ref j, out char c))
            {
                c = '-';
            }
            Console.WriteLine($@"""{args[i]}"" -> '{c}'");
        }
    }

    private static SCG.IEnumerable<(int, char)> GenerateGraph(string[] words)
    {
        var i = 0;
        foreach (var s in words)
        {
            foreach (char c in s)
            {
                yield return (i, c);
            }
            i++;
        }
    }

    /// <summary>
    /// Implements Hopcroft and Karps algorithm for Maximum bipartite matching.
    /// Input (to the constructor): the edges of the graph as a collection of pairs of labels of left and right nodes.
    /// Output (from Result property): a maximum matching as a collection of pairs of labels of left and right nodes.
    /// The algorithm uses natural equality on the label types to identify nodes.
    /// </summary>
    /// <typeparam name="TLeftLabel"></typeparam>
    /// <typeparam name="TRightLabel"></typeparam>
    public class BipartiteMatching<TLeftLabel, TRightLabel>
    {
        private readonly LeftNode[] _leftNodes;
        private readonly RightNode[] _rightNodes;

        private class LeftNode
        {
            public TLeftLabel Label { get; }
            public RightNode Match { get; set; }
            public RightNode[] Edges { get; }

            public LeftNode(TLeftLabel label, params RightNode[] edges)
            {
                Label = label;
                Edges = (RightNode[])edges.Clone();
            }

            public override string ToString()
            {
                return $@"""{Label}"" -> '{Match}'";
            }
        }

        private class RightNode
        {
            public TRightLabel Label { get; }
            public LeftNode Match { get; set; }
            public LeftNode BackRef { get; set; }
            public LeftNode Origin { get; set; }

            public RightNode(TRightLabel label)
            {
                Label = label;
            }

            public override string ToString()
            {
                return $"'{Label}'";
            }
        }

        public BipartiteMatching(SCG.IEnumerable<(TLeftLabel, TRightLabel)> graph)
        {
            var rdict = new HashDictionary<TRightLabel, RightNode>();
            var edges = new HashDictionary<TLeftLabel, HashSet<RightNode>>();
            var newrnodes = new HashSet<RightNode>();

            foreach (var edge in graph)
            {
                var x2 = edge.Item2;
                if (!rdict.Find(ref x2, out var rnode))
                {
                    rdict.Add(edge.Item2, rnode = new RightNode(edge.Item2));
                }
                HashSet<RightNode> ledges = newrnodes;
                if (!edges.FindOrAdd(edge.Item1, ref ledges))
                {
                    newrnodes = new HashSet<RightNode>();
                }
                ledges.Add(rnode);
            }

            _rightNodes = rdict.Values.ToArray();

            _leftNodes = new LeftNode[edges.Count];

            var li = 0;

            foreach (var les in edges)
            {
                _leftNodes[li++] = new LeftNode(les.Key, les.Value.ToArray());
            }

            Compute();
        }

        public SCG.IEnumerable<(TLeftLabel, TRightLabel)> Result
        {
            get
            {
                foreach (var l in _leftNodes)
                {
                    if (l.Match != null)
                    {
                        yield return (l.Label, l.Match.Label);
                    }
                }
            }
        }

        private HashSet<RightNode> _endPoints;
        private bool _foundAugmentingPath;

        private void Compute()
        {
            var unmatchedLeftNodes = new HashSet<LeftNode>();
            unmatchedLeftNodes.AddAll(_leftNodes);
            _foundAugmentingPath = true;
            Debug.Print("Compute start");
            while (_foundAugmentingPath)
            {
                Debug.Print("Start outer");
                _foundAugmentingPath = false;
                _endPoints = new HashSet<RightNode>();
                foreach (var rightNode in _rightNodes)
                {
                    rightNode.BackRef = null;
                }
                foreach (LeftNode l in unmatchedLeftNodes)
                {
                    Debug.Print("Unmatched: {0}", l);
                    Search(l, l);
                }
                while (!_foundAugmentingPath && _endPoints.Count > 0)
                {
                    var oldLayer = _endPoints;
                    _endPoints = new HashSet<RightNode>();
                    foreach (var rb in oldLayer)
                    {
                        Search(rb.Match, rb.Origin);
                    }
                }
                if (_endPoints.Count == 0)
                {
                    return;
                }
                //Flip
                Debug.Print("Flip");
                foreach (RightNode r in _endPoints)
                {
                    if (r.Match == null && unmatchedLeftNodes.Contains(r.Origin))
                    {
                        RightNode nextR = r;
                        LeftNode nextL = null;
                        while (nextR != null)
                        {
                            nextL = nextR.Match = nextR.BackRef;
                            var rSwap = nextL.Match;
                            nextL.Match = nextR;
                            nextR = rSwap;
                        }
                        unmatchedLeftNodes.Remove(nextL);
                    }
                }
            }
        }

        private void Search(LeftNode l, LeftNode origin)
        {
            foreach (RightNode r in l.Edges)
            {
                if (r.BackRef == null)
                {
                    r.BackRef = l;
                    r.Origin = origin;
                    _endPoints.Add(r);

                    if (r.Match == null)
                    {
                        _foundAugmentingPath = true;
                    }

                    // First round should be greedy

                    if (l == origin)
                    {
                        return;
                    }
                }
            }
        }
    }
}
