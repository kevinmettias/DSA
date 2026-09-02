namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3112 - a connected random graph (back edge per
// node plus extra random edges) paired with a generous per-node disappear
// ceiling, so deadlines rarely prune a node and both strategies do genuine
// full-graph Dijkstra work rather than mostly reporting -1.
internal static class DisappearingNodesWorkloads
{
    private const int ExtraEdgesPerNode = 2;
    private const int MaxLengthExclusive = 101; // kept small so distances stay well under the disappear ceiling
    private const int DisappearCeilingExclusive = 1_000_001; // comfortably above any reachable distance at this scale

    public static (int[][] Edges, int[] Disappear) Build(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var edges = BuildConnectedEdges(nodeCount, random);
        var disappear = BuildDisappear(nodeCount, random);

        return (edges, disappear);
    }

    private static int[][] BuildConnectedEdges(int nodeCount, Random random)
    {
        var edges = new List<int[]>();

        for (var i = 1; i < nodeCount; i++)
        {
            var j = random.Next(i);
            edges.Add([j, i, random.Next(1, MaxLengthExclusive)]);
        }

        for (var i = 0; i < nodeCount; i++)
        {
            for (var e = 0; e < ExtraEdgesPerNode; e++)
            {
                var target = random.Next(nodeCount);

                if (target != i)
                {
                    edges.Add([i, target, random.Next(1, MaxLengthExclusive)]);
                }
            }
        }

        return [.. edges];
    }

    private static int[] BuildDisappear(int nodeCount, Random random)
    {
        var disappear = new int[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            disappear[i] = random.Next(1, DisappearCeilingExclusive);
        }

        return disappear;
    }
}
