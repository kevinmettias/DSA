namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3620 - every node i > 0 gets a forward edge
// from some earlier node j < i (edges[i] = [j, i, cost]), guaranteeing node
// n-1 is reachable from node 0 through the graph's own forward edges; extra
// random forward-only edges add density so the binary search's per-threshold
// feasibility probes filter real edges instead of walking a bare spanning
// path. Edges only ever point from a lower index to a higher one, so the graph
// is acyclic by construction - LC 3620's own DAG guarantee, not a leftover of
// the generator (EdgeWeightGraphWorkloads' own forward/backward analogue).
internal static class NetworkRecoveryWorkloads
{
    private const int CostUpperBound = 1_000_000_000;
    private const double OfflineFraction = 0.2;

    public static (int[][] Edges, bool[] Online) Build(int nodeCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        for (var i = 1; i < nodeCount; i++)
        {
            var from = random.Next(i);
            edges.Add([from, i, random.Next(CostUpperBound)]);
        }

        for (var i = 0; i < nodeCount; i++)
        {
            for (var e = 0; e < extraEdgesPerNode; e++)
            {
                var to = random.Next(nodeCount);

                if (to > i)
                {
                    edges.Add([i, to, random.Next(CostUpperBound)]);
                }
            }
        }

        var online = new bool[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            online[i] = random.NextDouble() >= OfflineFraction;
        }

        online[0] = true;
        online[nodeCount - 1] = true;

        return ([.. edges], online);
    }
}
