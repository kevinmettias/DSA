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
        var edges = BuildForwardEdges(nodeCount, extraEdgesPerNode, random);
        var online = BuildOnlineFlags(nodeCount, random);

        return ([.. edges], online);
    }

    private static List<int[]> BuildForwardEdges(int nodeCount, int extraEdgesPerNode, Random random)
    {
        var edges = new List<int[]>();

        AddSpineEdges(edges, nodeCount, random);
        AddExtraForwardEdges(edges, nodeCount, extraEdgesPerNode, random);

        return edges;
    }

    // Every node i > 0 gets an edge from some earlier node j < i, so node n-1 is
    // reachable from node 0 through the graph's own forward edges.
    private static void AddSpineEdges(List<int[]> edges, int nodeCount, Random random)
    {
        for (var i = 1; i < nodeCount; i++)
        {
            var from = random.Next(i);
            edges.Add([from, i, random.Next(CostUpperBound)]);
        }
    }

    // Extra forward-only edges add the density that keeps the binary search's
    // feasibility probes filtering real edges instead of walking a bare spanning
    // path; only i < to pairs are added, so the graph stays acyclic.
    private static void AddExtraForwardEdges(
        List<int[]> edges, int nodeCount, int extraEdgesPerNode, Random random)
    {
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
    }

    // The first and last nodes stay online, so there are always endpoints for a
    // recovery path to work with.
    private static bool[] BuildOnlineFlags(int nodeCount, Random random)
    {
        var online = new bool[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            online[i] = random.NextDouble() >= OfflineFraction;
        }

        online[0] = true;
        online[nodeCount - 1] = true;

        return online;
    }
}
