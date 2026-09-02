namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3108 - a connected random graph (every node
// i > 0 gets a "back edge" to some earlier node j < i, guaranteeing one component,
// plus extra random edges for density) alongside a batch of random queries over
// it, so both strategies do genuine walk-cost work rather than mostly reporting -1
// for disconnected pairs.
internal static class MinimumCostWalkWorkloads
{
    private const int ExtraEdgesPerNode = 2;
    private const int MaxWeightExclusive = 100_001; // LC bounds edge weight to [0, 1e5]

    public static (int[][] Edges, int[][] Query) Build(int nodeCount, int queryCount, int seed)
    {
        var random = new Random(seed);
        var edges = BuildConnectedEdges(nodeCount, random);
        var query = BuildQueries(nodeCount, queryCount, random);

        return (edges, query);
    }

    private static int[][] BuildConnectedEdges(int nodeCount, Random random)
    {
        var edges = new List<int[]>();

        for (var i = 1; i < nodeCount; i++)
        {
            var j = random.Next(i);
            edges.Add([j, i, random.Next(MaxWeightExclusive)]);
        }

        for (var i = 0; i < nodeCount; i++)
        {
            for (var e = 0; e < ExtraEdgesPerNode; e++)
            {
                var target = random.Next(nodeCount);

                if (target != i)
                {
                    edges.Add([i, target, random.Next(MaxWeightExclusive)]);
                }
            }
        }

        return [.. edges];
    }

    private static int[][] BuildQueries(int nodeCount, int queryCount, Random random)
    {
        var query = new int[queryCount][];

        for (var i = 0; i < queryCount; i++)
        {
            var source = random.Next(nodeCount);
            int target;

            do
            {
                target = random.Next(nodeCount);
            }
            while (target == source);

            query[i] = [source, target];
        }

        return query;
    }
}
