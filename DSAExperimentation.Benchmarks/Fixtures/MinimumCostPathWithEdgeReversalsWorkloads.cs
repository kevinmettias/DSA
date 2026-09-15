namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3650 - a random directed graph shaped like
// FindEdgesInShortestPathsWorkloads' own back-edge-plus-extra-edges construction
// (every node i > 0 gets a directed edge from some earlier node j < i, guaranteeing
// node 0 can reach every node - including n-1 - without any reversal, plus extra
// random directed edges for density so a reversal can still occasionally shorten the
// path), emitting LeetCode's own int[][] edges shape directly since that is what both
// MinimumCostPathWithEdgeReversalsSolution strategies take before the graph
// strategy's own hoisted overload turns it into a ReversalGraph.
internal static class MinimumCostPathWithEdgeReversalsWorkloads
{
    private const int EdgeWeightUpperBound = 50;

    public static int[][] BuildEdges(int nodeCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        for (var i = 1; i < nodeCount; i++)
        {
            var j = random.Next(i);
            edges.Add([j, i, random.Next(1, EdgeWeightUpperBound)]);
        }

        AddExtraEdges(edges, random, nodeCount, extraEdgesPerNode);

        return [.. edges];
    }

    // Extra random directed edges for density, so a reversal can still occasionally shorten
    // the path. A self-edge is skipped: it neither reaches a new node nor is worth reversing.
    private static void AddExtraEdges(List<int[]> edges, Random random, int nodeCount, int extraEdgesPerNode)
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
