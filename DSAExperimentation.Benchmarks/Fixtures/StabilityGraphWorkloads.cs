namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3600 - a random spanning tree's own edges are
// marked must (guaranteeing they never form a cycle, so Feasible's binary search
// always has real work to do rather than short-circuiting to -1) plus extra
// random optional edges layered on top for the algorithm to actually choose
// among.
internal static class StabilityGraphWorkloads
{
    private const int ExtraOptionalEdgesPerNode = 2;
    private const int StrengthCeilingExclusive = 100_000;
    private const int UpgradeBudgetDivisor = 4;

    public static (int[][] Edges, int K) Build(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        for (var node = 1; node < nodeCount; node++)
        {
            var parent = random.Next(node);
            edges.Add([parent, node, random.Next(1, StrengthCeilingExclusive), 1]);
        }

        for (var node = 0; node < nodeCount; node++)
        {
            for (var extra = 0; extra < ExtraOptionalEdgesPerNode; extra++)
            {
                var target = random.Next(nodeCount);

                if (target != node)
                {
                    edges.Add([node, target, random.Next(1, StrengthCeilingExclusive), 0]);
                }
            }
        }

        return ([.. edges], nodeCount / UpgradeBudgetDivisor);
    }
}
