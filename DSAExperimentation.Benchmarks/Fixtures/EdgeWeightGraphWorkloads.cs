namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3419 - every node i > 0 gets a "toward node 0"
// edge to some earlier node j < i (edges[i] = [i, j, weight]), guaranteeing every
// node can reach node 0 through the graph's own forward edges the way the problem
// requires; extra random edges add density so the binary search's feasibility
// checks do real filtering work instead of walking a bare spanning tree.
internal static class EdgeWeightGraphWorkloads
{
    private const int EdgeWeightUpperBound = 1_000_000;

    public static int[][] BuildEdges(int nodeCount, int extraEdgesPerNode, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        for (var i = 1; i < nodeCount; i++)
        {
            var parent = random.Next(i);
            edges.Add([i, parent, random.Next(1, EdgeWeightUpperBound)]);
        }

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

        return [.. edges];
    }
}
