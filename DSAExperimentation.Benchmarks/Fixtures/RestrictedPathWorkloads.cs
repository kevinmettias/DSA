namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1786 - only how big the graph is and what shape
// of adversary it presents; the graph itself, and the Dijkstra labelling that makes
// it this problem's graph, are RestrictedPathGraph's job.
//
// Node u gets a weight-1 edge to u + 1 and a weight-2 edge to u + 2, both equally
// shortest, so every node's distance to the last one is exactly its remaining step
// count and the same downstream node is reached two ways at every layer - the
// Fibonacci recurrence that makes the unmemoized arm's recount genuinely
// exponential.
internal static class RestrictedPathWorkloads
{
    // The chain spans stepCount weight-1 edges, so it has one more node than steps.
    public static int NodeCount(int stepCount) => stepCount + 1;

    private const int TwoStepOffset = 2;

    public static int[][] BuildTwoStepEdges(int stepCount)
    {
        var edges = new List<int[]>();

        for (var u = 1; u <= stepCount; u++)
        {
            edges.Add([u, u + 1, 1]);
        }

        for (var u = 1; u <= stepCount - 1; u++)
        {
            edges.Add([u, u + TwoStepOffset, TwoStepOffset]);
        }

        return [.. edges];
    }
}
