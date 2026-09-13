namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1643. The problem's own input is only a
// destination and a rank, so all that is chosen here is how far away the
// destination sits and which rank to ask for: the median one, so the greedy walk
// has to make a real decision at nearly every step rather than running straight
// down one edge of the lexicographic order.
internal static class KthSmallestInstructionsWorkloads
{
    // destination is [Size, Size], so every route is Size vertical and Size
    // horizontal steps in some order - C(2 * Size, Size) of them.
    private const int TotalStepsMultiplier = 2;

    private const int MedianSequenceDivisor = 2;

    public static int[] SquareDestination(int size) => [size, size];

    public static long MedianRank(int size) => RouteCount(size) / MedianSequenceDivisor;

    private static long RouteCount(int size)
    {
        long count = 1;

        for (var i = 0; i < size; i++)
        {
            count = count * ((TotalStepsMultiplier * size) - i) / (i + 1);
        }

        return count;
    }
}
