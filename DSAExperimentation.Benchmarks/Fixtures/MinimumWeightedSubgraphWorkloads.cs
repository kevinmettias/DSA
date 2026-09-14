namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2203 - a random directed weighted graph shaped
// like RandomWeightedGraphs' own back-edge-plus-extra-edges construction (every
// node i > 0 gets a back edge from some earlier node j < i, so node 0 reaches
// everyone), with one extra detail this problem needs: node 1 is the second source,
// so it gets its own spread of outgoing edges rather than only whatever reach the
// back-edge chain happens to grant it. Emits LeetCode's own int[][] edges shape
// directly rather than a node/topology pair, since that is what both
// MinimumWeightedSubgraphWithTheRequiredPathsSolution strategies take before the
// Dijkstra strategy's hoisted overload turns it into a RequiredPathsGraph.
internal static class MinimumWeightedSubgraphWorkloads
{
    private const int EdgeWeightUpperBound = 50;
    private const int SecondSourceId = 1;

    public static int[][] BuildEdges(int nodeCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        AddSpanningBackEdges(edges, nodeCount, random);
        AddSecondSourceSpread(edges, nodeCount, extraEdgesPerNode, random);
        AddRandomExtraEdges(edges, nodeCount, extraEdgesPerNode, random);

        return [.. edges];
    }

    private static void AddSpanningBackEdges(List<int[]> edges, int nodeCount, Random random)
    {
        for (var i = 1; i < nodeCount; i++)
        {
            var j = random.Next(i);
            edges.Add([j, i, random.Next(1, EdgeWeightUpperBound)]);
        }
    }

    private static void AddSecondSourceSpread(List<int[]> edges, int nodeCount, int extraEdgesPerNode, Random random)
    {
        for (var e = 0; e < extraEdgesPerNode; e++)
        {
            var target = random.Next(nodeCount);
            edges.Add([SecondSourceId, target, random.Next(1, EdgeWeightUpperBound)]);
        }
    }

    private static void AddRandomExtraEdges(List<int[]> edges, int nodeCount, int extraEdgesPerNode, Random random)
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
