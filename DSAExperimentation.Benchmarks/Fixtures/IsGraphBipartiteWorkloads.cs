namespace DSAExperimentation.Benchmarks.Fixtures;

// Workload sizing for LC 785: an undirected graph split into an A half and a B half,
// in the problem's own int[][] adjacency shape, whose every edge crosses the split, so
// it is bipartite by construction and neither strategy short-circuits on an early
// color conflict. Every B node gets one edge back to a random A node, and every node
// then draws DensityEdgesPerNode more cross edges for density. An edge drawn twice is
// joined once, since LC promises every neighbor list holds distinct values.
internal static class IsGraphBipartiteWorkloads
{
    // The graph is split into exactly two sides (A and B) to stay bipartite by
    // construction.
    private const int PartitionCount = 2;

    // Extra cross-only edges added per node for density.
    private const int DensityEdgesPerNode = 2;

    public static int[][] BuildAdjacency(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var half = nodeCount / PartitionCount;
        var edges = new List<(int From, int To)>();

        AddConnectivityEdges(edges, random, (half, nodeCount));
        AddDensityEdges(edges, random, (half, nodeCount));

        return JoinOnce(nodeCount, edges);
    }

    // Every B-side node gets one cross edge back to a random A-side node.
    private static void AddConnectivityEdges(List<(int From, int To)> edges, Random random, (int Half, int NodeCount) split)
    {
        for (var i = split.Half; i < split.NodeCount; i++)
        {
            edges.Add((random.Next(split.Half), i));
        }
    }

    // Extra cross-only edges for density - still strictly A-to-B, so the graph
    // stays bipartite by construction.
    private static void AddDensityEdges(List<(int From, int To)> edges, Random random, (int Half, int NodeCount) split)
    {
        for (var i = 0; i < split.NodeCount; i++)
        {
            for (var e = 0; e < DensityEdgesPerNode; e++)
            {
                var inA = i < split.Half;
                var target = inA ? RandomBNode(random, split) : random.Next(split.Half);
                edges.Add((i, target));
            }
        }
    }

    // A random node on the B side: the B nodes are the upper half of the range.
    private static int RandomBNode(Random random, (int Half, int NodeCount) split) =>
        split.Half + random.Next(split.NodeCount - split.Half);

    private static int[][] JoinOnce(int nodeCount, List<(int From, int To)> edges)
    {
        var adjacency = Enumerable.Range(0, nodeCount).Select(_ => new List<int>()).ToArray();
        var joinedPairs = new HashSet<(int Low, int High)>();

        foreach (var (from, to) in edges)
        {
            var pair = (Math.Min(from, to), Math.Max(from, to));

            if (joinedPairs.Add(pair))
            {
                adjacency[from].Add(to);
                adjacency[to].Add(from);
            }
        }

        return adjacency.Select(neighbors => neighbors.ToArray()).ToArray();
    }
}
