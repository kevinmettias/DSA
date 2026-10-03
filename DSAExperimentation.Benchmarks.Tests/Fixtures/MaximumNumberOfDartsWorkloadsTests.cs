using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MaximumNumberOfDartsWorkloads (ARCHITECTURE 17.7). The reading depends on a
// dart cloud spread across a square around the origin, and on staying inside LC 1453's contract:
// every dart unique. At this size and seed an unchecked draw repeats a dart, so the distinctness
// assertion is load-bearing.
public sealed partial class MaximumNumberOfDartsWorkloadsTests
{
    private const int DartCount = 20; // the benchmark's smallest size
    private const int CoordinateRange = 100;
    private const int Seed = 1453; // LC problem number
    private const int XPosition = 0;
    private const int YPosition = 1;

    [Fact]
    public void BuildDarts_DartCount_ReturnsThatManyDistinctDarts()
    {
        var darts = Build();

        Assert.Equal(DartCount, darts.Length);
        Assert.Equal(DartCount, darts.Select(dart => (dart[XPosition], dart[YPosition])).Distinct().Count());
    }

    [Fact]
    public void BuildDarts_EveryCoordinate_StaysInsideTheSquare() =>
        Assert.All(
            Build(),
            dart => Assert.All(dart, coordinate => Assert.InRange(coordinate, -CoordinateRange, CoordinateRange - 1)));

    [Fact]
    public void BuildDarts_SameSeed_ReturnsTheSameDarts() =>
        Assert.Equal(Build(), Build());

    private static int[][] Build() => MaximumNumberOfDartsWorkloads.BuildDarts(DartCount, CoordinateRange, Seed);
}
