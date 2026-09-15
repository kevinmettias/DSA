namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3977 - forward-only edges (node i gets an
// edge from some earlier node j < i, plus a few random forward extras)
// guarantee node n-1 stays reachable from node 0, the same shape
// NetworkRecoveryWorkloads uses for LC 3620's own DAG. Every node's cost is
// kept well under Power / expected hop count so a run typically has enough
// power to cross several edges before it would run out, keeping the
// remainingPower dimension of the state space genuinely exercised instead of
// collapsing to "stranded after one hop."
internal static class PowerStateWorkloads
{
    private const int WeightUpperBound = 1_000_000;
    private const int ExtraEdgesPerNode = 2;
    private const int MaxCost = 3;

    public static (int[][] Edges, int[] Cost) Build(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        for (var i = 1; i < nodeCount; i++)
        {
            var from = random.Next(i);
            edges.Add([from, i, random.Next(1, WeightUpperBound)]);
        }

        AddExtraForwardEdges(random, edges, nodeCount);

        return ([.. edges], BuildCost(nodeCount, random));
    }

    // Drawn after the spanning edges above and before BuildCost's costs, so the
    // seeded sequence is consumed in exactly the order the graph and its costs
    // depend on.
    private static void AddExtraForwardEdges(Random random, List<int[]> edges, int nodeCount)
    {
        for (var i = 0; i < nodeCount; i++)
        {
            for (var e = 0; e < ExtraEdgesPerNode; e++)
            {
                var to = random.Next(nodeCount);

                if (to > i)
                {
                    edges.Add([i, to, random.Next(1, WeightUpperBound)]);
                }
            }
        }
    }

    private static int[] BuildCost(int nodeCount, Random random)
    {
        var cost = new int[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            cost[i] = random.Next(1, MaxCost + 1);
        }

        return cost;
    }
}
