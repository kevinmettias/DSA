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
        var seen = new HashSet<(int, int)>();
        var edges = new List<int[]>();
        var time = 1;

        while (edges.Count < edgeCount)
        {
            var u = random.Next(n);
            var v = random.Next(n);

            if (u == v)
            {
                continue;
            }

            var key = u < v ? (u, v) : (v, u);

            if (!seen.Add(key))
            {
                continue;
            }

            edges.Add([u, v, time++]);
        }

        return [.. edges];
    }
}
