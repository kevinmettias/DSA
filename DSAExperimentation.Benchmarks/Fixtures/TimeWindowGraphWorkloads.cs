namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3604 - a random forward-edge backbone
// (guaranteeing node n-1 stays reachable from node 0) plus extra random edges,
// each with a wide-enough time window that scheduling around it is a genuine
// choice rather than an automatic dead end.
internal static class TimeWindowGraphWorkloads
{
    private const int ExtraEdgesPerNode = 2;
    private const int WindowStartCeilingExclusive = 50;
    private const int WindowLengthCeilingExclusive = 20;

    public static int[][] Build(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        AddBackboneEdges(edges, nodeCount, random);
        AddExtraEdges(edges, nodeCount, random);

        return [.. edges];
    }

    // The forward-edge backbone - every node hangs off an earlier one - which is
    // what guarantees node n-1 stays reachable from node 0.
    private static void AddBackboneEdges(List<int[]> edges, int nodeCount, Random random)
    {
        for (var node = 1; node < nodeCount; node++)
        {
            var source = random.Next(node);
            var edge = BuildEdge(source, node, random);
            edges.Add(edge);
        }
    }

    // The extra random edges on top of the backbone.
    private static void AddExtraEdges(List<int[]> edges, int nodeCount, Random random)
    {
        for (var node = 0; node < nodeCount; node++)
        {
            for (var extra = 0; extra < ExtraEdgesPerNode; extra++)
            {
                var target = random.Next(nodeCount);

                if (target != node)
                {
                    var edge = BuildEdge(node, target, random);
                    edges.Add(edge);
                }
            }
        }
    }

    private static int[] BuildEdge(int fromNode, int toNode, Random random)
    {
        var start = random.Next(WindowStartCeilingExclusive);
        var end = start + random.Next(1, WindowLengthCeilingExclusive);
        return [fromNode, toNode, start, end];
    }
}
