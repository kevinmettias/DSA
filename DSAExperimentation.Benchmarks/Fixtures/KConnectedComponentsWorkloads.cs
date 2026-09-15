namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3608 - a random simple graph on n nodes with up
// to 2n distinct edges, each given a distinct time in insertion order, dense enough
// that the graph starts out mostly (or fully) connected and removing edges by time
// actually has to do real component-splitting work before k is reached.
internal static class KConnectedComponentsWorkloads
{
    public static int[][] BuildEdges(int n, int seed)
    {
        var random = new Random(seed);
        var edgeCount = Math.Min(n * 2, n * (n - 1) / 2);
        var seen = new HashSet<UndirectedEdge>();
        var edges = new List<int[]>();
        var time = 1;

        while (edges.Count < edgeCount)
        {
            var drawn = DrawUnseenPair(random, n, seen);

            if (drawn is (int u, int v))
            {
                edges.Add([u, v, time++]);
            }
        }

        return [.. edges];
    }

    // One draw of an unordered endpoint pair, or null when the draw is rejected: two
    // equal endpoints, or a pair already claimed. A rejected draw leaves `seen`
    // untouched and does not count against the edge budget, so the caller draws again.
    private static (int U, int V)? DrawUnseenPair(Random random, int n, HashSet<UndirectedEdge> seen)
    {
        var u = random.Next(n);
        var v = random.Next(n);

        if (u == v)
        {
            return null;
        }

        if (!seen.Add(u < v ? EdgeKey(u, v) : EdgeKey(v, u)))
        {
            return null;
        }

        return (u, v);
    }

    // The undirected edge's canonical key: the two endpoints written low first, so
    // a later (v, u) for the same edge hashes to the same key.
    private static UndirectedEdge EdgeKey(int low, int high) => new(low, high);
}
