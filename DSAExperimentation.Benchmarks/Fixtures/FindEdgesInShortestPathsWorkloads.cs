namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3123 - a connected random weighted graph shaped
// like RandomWeightedGraphs' own back-edge-plus-extra-edges construction (every
// node i > 0 gets a back edge to some earlier node j < i, guaranteeing reachability
// from node 0, plus extra random edges for density), but emitting LeetCode's own
// int[][] edges shape directly rather than a node/topology pair, since that is what
// both FindEdgesInShortestPathsSolution strategies take before the graph strategy's
// own hoisted overload turns it into an EdgeGraph.
internal static class FindEdgesInShortestPathsWorkloads
{
    private const int EdgeWeightUpperBound = 50;

    public static int[][] BuildEdges(int nodeCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        AddBackEdges(edges, nodeCount, random);
        AddExtraEdges(edges, nodeCount, extraEdgesPerNode, random);

        return [.. edges];
    }

    // A back edge from every node i > 0 to some earlier node j < i, which is what
    // guarantees node 0 reaches every other node.
    private static void AddBackEdges(List<int[]> edges, int nodeCount, Random random)
    {
        for (var i = 1; i < nodeCount; i++)
        {
            var j = random.Next(i);
            edges.Add([j, i, random.Next(1, EdgeWeightUpperBound)]);
        }
    }

    // Extra random edges on top of the back-edge backbone, for density.
    private static void AddExtraEdges(List<int[]> edges, int nodeCount, int extraEdgesPerNode, Random random)
    {
        for (var i = 0; i < nodeCount; i++)
        {
            for (var e = 0; e < extraEdgesPerNode; e++)
            {
                var target = random.Next(nodeCount);

                if (target != i)
                {
                    edges.Add([i, target, random.Next(1, EdgeWeightUpperBound)]);
                }
            }
        }
    }
}
