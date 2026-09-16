using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LatticePointWorkloads (ARCHITECTURE 17.7). The reading depends on the point set
// being distinct lattice points drawn from a grid barely larger than the point count, so rectangles
// are plentiful and either minimum-area sibling's arms do real corner-confirmation work.
public sealed partial class LatticePointWorkloadsTests
{
    private const int Count = 64;
    private const int GridPadding = 4;
    private const int Seed = 939; // LC problem number of the axis-aligned sibling
    private const int PointFieldCount = 2; // X, Y

    [Fact]
    public void InGrid_Count_ReturnsOneCoordinatePairPerRequestedPoint()
    {
        var points = LatticePointWorkloads.InGrid(Count, GridPadding, Seed);

        Assert.Equal(Count, points.Length);
        Assert.All(points, point => Assert.Equal(PointFieldCount, point.Length));
    }

    [Fact]
    public void InGrid_EveryPoint_StaysInsideTheGridDerivedFromThePointCount()
    {
        var points = LatticePointWorkloads.InGrid(Count, GridPadding, Seed);
        var grid = (int)Math.Ceiling(Math.Sqrt(Count)) + GridPadding;

        Assert.All(points, point => Assert.InRange(point[0], 0, grid - 1));
        Assert.All(points, point => Assert.InRange(point[1], 0, grid - 1));
    }

    [Fact]
    public void InGrid_EveryPoint_IsDistinct()
    {
        var points = LatticePointWorkloads.InGrid(Count, GridPadding, Seed);

        Assert.Equal(Count, points.Select(point => (point[0], point[1])).Distinct().Count());
    }

    [Fact]
    public void InGrid_SameSeed_ReturnsTheSamePoints() =>
        Assert.Equal(
            AnswerText.Of(LatticePointWorkloads.InGrid(Count, GridPadding, Seed)),
            AnswerText.Of(LatticePointWorkloads.InGrid(Count, GridPadding, Seed)));
}
