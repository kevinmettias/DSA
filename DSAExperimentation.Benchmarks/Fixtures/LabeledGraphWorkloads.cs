namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3615 - a random connected graph on at most 14
// labeled nodes (the problem's own upper bound), built the same "back edge from
// each node to an earlier one, plus a little extra density" way
// EdgeWeightGraphWorkloads does for weighted graphs, just unweighted and with a
// random lowercase label per node instead of a random weight per edge. A small
// alphabet keeps same-label pairs frequent, so both strategies still do real
// palindrome-growing work instead of bottoming out at length 1 immediately. An extra
// edge that lands on its own node or on a pair already joined is dropped, keeping
// LC 3615's "no duplicate edges" guarantee.
internal static class LabeledGraphWorkloads
{
    private const int AlphabetSize = 4;

    public static (int[][] Edges, string Label) Build(int nodeCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();
        var joinedPairs = new HashSet<(int Low, int High)>();

        for (var i = 1; i < nodeCount; i++)
        {
            var earlier = random.Next(i);
            joinedPairs.Add((earlier, i));
            edges.Add([i, earlier]);
        }

        for (var i = 0; i < nodeCount; i++)
        {
            for (var e = 0; e < extraEdgesPerNode; e++)
            {
                AddExtraEdge(edges, joinedPairs, i, random.Next(nodeCount));
            }
        }

        return ([.. edges], BuildLabel(nodeCount, random));
    }

    // The density edge from node to target, unless it would be a self-loop or repeat a pair.
    private static void AddExtraEdge(List<int[]> edges, HashSet<(int Low, int High)> joinedPairs, int node, int target)
    {
        var pair = (Math.Min(node, target), Math.Max(node, target));
        var isNewPair = target != node && joinedPairs.Add(pair);

        if (isNewPair)
        {
            edges.Add([node, target]);
        }
    }

    private static string BuildLabel(int nodeCount, Random random)
    {
        var chars = new char[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        return new string(chars);
    }
}
