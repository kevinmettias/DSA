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

        for (var node = 1; node < nodeCount; node++)
        {
            var source = random.Next(node);
            edges.Add(BuildEdge(source, node, random));
        }

        for (var node = 0; node < nodeCount; node++)
        {
            for (var extra = 0; extra < ExtraEdgesPerNode; extra++)
            {
                var target = random.Next(nodeCount);

                if (target != node)
                {
                    edges.Add(BuildEdge(node, target, random));
                }
            }
        }

        return [.. edges];
    }

    private static int[] BuildEdge(int u, int v, Random random)
    {
        var start = random.Next(WindowStartCeilingExclusive);
        var end = start + random.Next(1, WindowLengthCeilingExclusive);
        return [u, v, start, end];
    }
}
