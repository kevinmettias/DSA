namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3970 - forward-only edges (node i gets an
// edge from some earlier node j < i, plus a few random forward extras)
// guarantee node n-1 stays reachable from node 0, the same shape
// NetworkRecoveryWorkloads uses for LC 3620's own DAG. Labels cycle through a
// small alphabet so most crossings genuinely extend or reset a run instead of
// trivially always resetting it, keeping the run-length dimension of the state
// space exercised.
internal static class ConsecutiveRunWorkloads
{
    private const int WeightUpperBound = 10_000;
    private const int ExtraEdgesPerNode = 2;
    private const int AlphabetSize = 3;

    public static (int[][] Edges, string Labels) Build(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        for (var i = 1; i < nodeCount; i++)
        {
            var from = random.Next(i);
            edges.Add([from, i, random.Next(1, WeightUpperBound)]);
        }

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

        return ([.. edges], BuildLabels(nodeCount, random));
    }

    private static string BuildLabels(int nodeCount, Random random)
    {
        var labels = new char[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            labels[i] = (char)('a' + random.Next(AlphabetSize));
        }

        return new string(labels);
    }
}
