namespace DSAExperimentation.Benchmarks.Fixtures;

// Workload sizing for LC 3887: random (u, v, w) triples with u < v (LC's own
// 0 <= ui < vi < n) and a random 0/1 weight. Node ids are drawn from a pool as large
// as the edge count, so a fair share of edges land inside an already-connected
// component (the case the pruned strategy still has to fall back to BFS for) rather
// than always stitching together fresh ones. A pair an earlier edge already joined is
// redrawn, which keeps LC's "all edges are distinct" guarantee.
internal static class IncrementalEvenWeightedCycleQueriesWorkloads
{
    private const int WeightCount = 2;

    public static int[][] BuildEdges(int nodeCount, int edgeCount, int seed)
    {
        var random = new Random(seed);
        var edges = new int[edgeCount][];
        var joinedPairs = new HashSet<(int U, int V)>();

        for (var i = 0; i < edgeCount; i++)
        {
            (int U, int V) pair;

            do
            {
                pair = DrawPair(nodeCount, random);
            }
            while (!joinedPairs.Add(pair));

            var weight = random.Next(0, WeightCount);
            edges[i] = [pair.U, pair.V, weight];
        }

        return edges;
    }

    private static (int U, int V) DrawPair(int nodeCount, Random random)
    {
        var a = random.Next(0, nodeCount);
        var b = random.Next(0, nodeCount);

        while (b == a)
        {
            b = random.Next(0, nodeCount);
        }

        return (Math.Min(a, b), Math.Max(a, b));
    }
}
