using DSAExperimentation.Algorithms.Geometry;

namespace DSAExperimentation.Tests.Algorithms.Geometry;

public sealed partial class ConvexHullTests
{
    private const int RandomPointCount = 200;
    private const int RandomCoordinateBound = 50;

    [Fact]
    public void Corners_SquareWithAnInteriorPoint_ReturnsTheFourCornersCounterClockwise() =>
        Assert.Equal(
            [(0, 0), (2, 0), (2, 2), (0, 2)],
            ConvexHull.Corners([(1, 1), (2, 2), (0, 0), (0, 2), (2, 0)]));

    // A point on an edge between two corners is on the boundary but is not a corner.
    [Fact]
    public void Corners_PointOnAnEdge_IsLeftOut() =>
        Assert.Equal(
            [(0, 0), (2, 0), (2, 2), (0, 2)],
            ConvexHull.Corners([(0, 0), (1, 0), (2, 0), (2, 2), (0, 2)]));

    [Fact]
    public void Corners_CollinearPoints_ReturnsTheTwoEnds() =>
        Assert.Equal([(0, 0), (3, 3)], ConvexHull.Corners([(2, 2), (0, 0), (3, 3), (1, 1)]));

    [Fact]
    public void Corners_RepeatedPoints_AreReportedOnce() =>
        Assert.Equal([(0, 0), (1, 0), (0, 1)], ConvexHull.Corners([(0, 0), (1, 0), (0, 0), (0, 1), (1, 0)]));

    [Fact]
    public void Corners_CoincidentPoints_ReturnsThePointOnce() =>
        Assert.Equal([(3, 4)], ConvexHull.Corners([(3, 4), (3, 4), (3, 4)]));

    [Fact]
    public void Corners_NoPoints_ReturnsNoCorners() => Assert.Empty(ConvexHull.Corners([]));

    // Spans of 2^32 in each direction: a cross product in long would overflow here.
    [Fact]
    public void Corners_ExtremeCoordinates_StayExact() =>
        Assert.Equal(
            [(int.MinValue, int.MinValue), (int.MaxValue, int.MinValue), (int.MaxValue, int.MaxValue), (int.MinValue, int.MaxValue)],
            ConvexHull.Corners([(0, 0), (int.MaxValue, int.MaxValue), (int.MinValue, int.MinValue), (int.MinValue, int.MaxValue), (int.MaxValue, int.MinValue)]));

    [Fact]
    public void Corners_InputPoints_AreLeftAsGiven()
    {
        (int X, int Y)[] points = [(2, 2), (0, 0), (1, 1), (2, 0)];

        _ = ConvexHull.Corners(points);

        Assert.Equal([(2, 2), (0, 0), (1, 1), (2, 0)], points);
    }

    // Seeded points in a small box, so collinear and repeated points are common: every point lies
    // on or to the left of every hull edge, and the hull starts at the smallest point.
    [Fact]
    public void Corners_RandomPoints_EnclosesEveryPointAndStartsAtTheSmallest()
    {
        var random = new Random(7);
        var points = Enumerable.Range(0, RandomPointCount)
            .Select(_ => (X: random.Next(RandomCoordinateBound), Y: random.Next(RandomCoordinateBound)))
            .ToArray();

        var corners = ConvexHull.Corners(points);

        Assert.Equal(points.Min(), corners[0]);
        Assert.All(
            Enumerable.Range(0, corners.Length),
            i => Assert.All(points, point => Assert.True(Cross(corners[i], corners[(i + 1) % corners.Length], point) >= 0)));
    }

    private static Int128 Cross((int X, int Y) origin, (int X, int Y) first, (int X, int Y) second)
        => ((Int128)first.X - origin.X) * ((Int128)second.Y - origin.Y)
            - ((Int128)first.Y - origin.Y) * ((Int128)second.X - origin.X);
}
