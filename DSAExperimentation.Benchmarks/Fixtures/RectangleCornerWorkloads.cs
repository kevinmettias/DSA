namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3235 - circles are scattered across a fixed
// rectangle with a radius large enough relative to the coordinate spread that a
// meaningful fraction actually overlap each other (a radius tiny compared to the
// spread would make every circle its own isolated component and let both strategies
// skip the pairwise-overlap work entirely), the same reasoning RectangleWorkloads
// gives for narrowing LC 3047's own coordinate range.
internal static class RectangleCornerWorkloads
{
    private const int MinRadius = 20;
    private const int MaxRadius = 80;

    public static int[][] BuildCircles(int count, int seed)
    {
        var random = new Random(seed);
        var circles = new int[count][];

        for (var i = 0; i < count; i++)
        {
            var x = random.Next(1, RectangleCornerScenario.XCorner);
            var y = random.Next(1, RectangleCornerScenario.YCorner);
            var r = random.Next(MinRadius, MaxRadius + 1);
            circles[i] = [x, y, r];
        }

        return circles;
    }
}
