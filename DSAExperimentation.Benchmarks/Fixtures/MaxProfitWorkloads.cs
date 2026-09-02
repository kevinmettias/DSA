namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3530 - edges are drawn only from a lower
// index to a higher one, which keeps the result acyclic by construction with no
// separate cycle check, at a density that constrains valid orderings without
// collapsing the DAG to a single chain.
internal static class MaxProfitWorkloads
{
    private const int EdgeChancePercent = 35;
    private const int MaxScoreExclusive = 100_001; // LC bounds score to [1, 1e5]

    public static (int[][] Edges, int[] Score) Build(int n, int seed)
    {
        var random = new Random(seed);
        var edges = new List<int[]>();

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                if (random.Next(100) < EdgeChancePercent)
                {
                    edges.Add([i, j]);
                }
            }
        }

        var score = new int[n];

        for (var i = 0; i < n; i++)
        {
            score[i] = 1 + random.Next(MaxScoreExclusive - 1);
        }

        return ([.. edges], score);
    }
}
