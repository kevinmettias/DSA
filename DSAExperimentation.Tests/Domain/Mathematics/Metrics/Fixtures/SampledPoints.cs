namespace DSAExperimentation.Tests.Domain.Mathematics.Metrics.Fixtures;

// Points to hold the norms to, drawn once from a fixed seed so every run checks the same ones: a
// dozen in each of several dimensions, the line, the plane, space and five dimensions, with whole
// coordinates from -9 to 9 held as doubles, so a sum or a maximum of them is exact and only a root
// rounds. Each dimension's first point is the origin.
internal static class SampledPoints
{
    private const int Seed = 1_266;
    private const int PointsPerDimension = 12;
    private const int SmallestCoordinate = -9;
    private const int LargestCoordinate = 9;

    // The line, the plane, space, and five dimensions.
    private static readonly int[] Dimensions = [1, 2, 3, 5];

    // Each dimension sampled, and its points.
    public static IReadOnlyList<(int Dimension, double[][] Points)> ByDimension { get; } = Draw();

    private static (int Dimension, double[][] Points)[] Draw()
    {
        var random = new Random(Seed);

        return [.. Dimensions.Select(dimension => (dimension, PointsOf(dimension, random)))];
    }

    private static double[][] PointsOf(int dimension, Random random) =>
        [new double[dimension], .. Enumerable.Range(1, PointsPerDimension - 1)
            .Select(_ => PointOf(dimension, random))];

    private static double[] PointOf(int dimension, Random random) =>
        [.. Enumerable.Range(0, dimension)
            .Select(_ => (double)random.Next(SmallestCoordinate, LargestCoordinate + 1))];
}
