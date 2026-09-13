namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 882 - the same connected random graph shape
// RandomWeightedGraphs builds (every node i > 0 gets a back edge to some earlier node
// j < i, guaranteeing reachability from node 0, plus extra random edges for density),
// emitted directly in LeetCode's own int[][] {u, v, cnt} shape rather than as a
// node/topology pair, since that is what both ReachableNodesInSubdividedGraphSolution
// strategies take before the Dijkstra strategy's hoisted overload turns it into a
// SubdividedGraph.
//
// A generated weight of w becomes a subdivision count of w - 1, so an edge costs
// exactly w unit moves end to end - the weighting SubdividedGraph.Build applies in
// reverse, and what keeps the subdivided graph's size comparable to the weighted one.
internal static class ReachableNodesInSubdividedGraphWorkloads
{
    // Exclusive upper bound passed to Random.Next(1, _): weights land in [1, 49], so
    // subdivision counts land in [0, 48].
    private const int EdgeWeightUpperBound = 50;

    public static int[][] BuildEdges(int nodeCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        for (var i = 1; i < nodeCount; i++)
        {
            var j = random.Next(i);
            edges.Add([j, i, random.Next(1, EdgeWeightUpperBound) - 1]);
        }

        for (var i = 0; i < nodeCount; i++)
        {
            for (var e = 0; e < extraEdgesPerNode; e++)
            {
                var target = random.Next(nodeCount);

                if (target != i)
                {
                    edges.Add([i, target, random.Next(1, EdgeWeightUpperBound) - 1]);
                }
            }
        }

        return [.. edges];
    }
}
