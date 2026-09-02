namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3047 - bottom-left corners spread over a
// modest coordinate range with a modest side length, so a meaningful
// fraction of pairs actually overlap (a full 1-10,000,000 coordinate spread,
// LC's own bound, would make almost every pair disjoint and let the pruned
// strategy break on its first comparison every time, the same reasoning
// PrefixSuffixPairWorkloads gives for narrowing its alphabet).
internal static class RectangleWorkloads
{
    private const int CoordinateRange = 200;
    private const int MaxSide = 30;

    public static (int[][] BottomLeft, int[][] TopRight) BuildRectangles(int count, int seed)
    {
        var random = new Random(seed);
        var bottomLeft = new int[count][];
        var topRight = new int[count][];

        for (var i = 0; i < count; i++)
        {
            var a = random.Next(1, CoordinateRange);
            var b = random.Next(1, CoordinateRange);
            var width = random.Next(1, MaxSide + 1);
            var height = random.Next(1, MaxSide + 1);

            bottomLeft[i] = [a, b];
            topRight[i] = [a + width, b + height];
        }

        return (bottomLeft, topRight);
    }
}
