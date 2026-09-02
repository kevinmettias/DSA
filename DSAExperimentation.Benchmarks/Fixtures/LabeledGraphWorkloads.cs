namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3615 - a random connected graph on at most 14
// labeled nodes (the problem's own upper bound), built the same "back edge from
// each node to an earlier one, plus a little extra density" way
// EdgeWeightGraphWorkloads does for weighted graphs, just unweighted and with a
// random lowercase label per node instead of a random weight per edge. A small
// alphabet keeps same-label pairs frequent, so both strategies still do real
// palindrome-growing work instead of bottoming out at length 1 immediately.
internal static class LabeledGraphWorkloads
{
    private const int AlphabetSize = 4;

    public static (int[][] Edges, string Label) Build(int nodeCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        for (var i = 1; i < nodeCount; i++)
        {
            edges.Add([i, random.Next(i)]);
        }

        for (var i = 0; i < nodeCount; i++)
        {
            for (var e = 0; e < extraEdgesPerNode; e++)
            {
                var target = random.Next(nodeCount);

                if (target != i)
                {
                    edges.Add([i, target]);
                }
            }
        }

        return ([.. edges], BuildLabel(nodeCount, random));
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
