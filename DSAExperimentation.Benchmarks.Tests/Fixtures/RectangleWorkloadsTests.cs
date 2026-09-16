using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RectangleWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3047's bottom
// left corners being spread over a modest coordinate range with a modest side length, so a meaningful
// fraction of pairs actually overlap - LC's own full coordinate spread would make almost every pair
// disjoint and let the pruned strategy break on its first comparison every time.
public sealed partial class RectangleWorkloadsTests
{
    private const int RectangleCount = 50;
    private const int Seed = 3047;
    private const int CoordinateRange = 200;
    private const int MaxSide = 30;
    private const int MinCoordinate = 1;
    private const int MinSide = 1;
    private const int CoordinateFieldCount = 2;

    [Fact]
    public void BuildRectangles_RectangleCount_ReturnsOneCornerPairPerPosition()
    {
        var (bottomLeft, topRight) = RectangleWorkloads.BuildRectangles(RectangleCount, Seed);

        Assert.Equal(RectangleCount, bottomLeft.Length);
        Assert.Equal(RectangleCount, topRight.Length);
        Assert.All(bottomLeft, corner => Assert.Equal(CoordinateFieldCount, corner.Length));
        Assert.All(topRight, corner => Assert.Equal(CoordinateFieldCount, corner.Length));
    }

    [Fact]
    public void BuildRectangles_EveryRectangle_IsAnchoredInsideTheCoordinateRangeWithASideInsideTheCap()
    {
        var (bottomLeft, topRight) = RectangleWorkloads.BuildRectangles(RectangleCount, Seed);

        Assert.All(
            Enumerable.Range(0, RectangleCount),
            index => AssertRectangle(bottomLeft[index], topRight[index]));
    }

    // The two corners are the rectangle's own statement of its extent, so the top-right has to be beyond
    // the bottom-left on both axes - a rectangle that inverted one axis would overlap nothing, and the
    // pruned strategy's whole comparison would be decided by that rather than by the overlap test.
    [Fact]
    public void BuildRectangles_EveryRectangle_HasItsTopRightBeyondItsBottomLeftOnBothAxes()
    {
        var (bottomLeft, topRight) = RectangleWorkloads.BuildRectangles(RectangleCount, Seed);

        Assert.All(
            Enumerable.Range(0, RectangleCount),
            index => Assert.True(
                topRight[index][0] > bottomLeft[index][0] && topRight[index][1] > bottomLeft[index][1]));
    }

    [Fact]
    public void BuildRectangles_SameSeed_ReturnsTheSameRectangles()
    {
        var (bottomLeft, topRight) = RectangleWorkloads.BuildRectangles(RectangleCount, Seed);
        var (repeatBottomLeft, repeatTopRight) = RectangleWorkloads.BuildRectangles(RectangleCount, Seed);

        Assert.Equal(AnswerText.Of(bottomLeft), AnswerText.Of(repeatBottomLeft));
        Assert.Equal(AnswerText.Of(topRight), AnswerText.Of(repeatTopRight));
    }

    private static void AssertRectangle(int[] bottomLeft, int[] topRight)
    {
        Assert.All(bottomLeft, coordinate => Assert.InRange(coordinate, MinCoordinate, CoordinateRange - 1));
        Assert.InRange(topRight[0] - bottomLeft[0], MinSide, MaxSide);
        Assert.InRange(topRight[1] - bottomLeft[1], MinSide, MaxSide);
        Assert.True(topRight[0] > bottomLeft[0]);
        Assert.True(topRight[1] > bottomLeft[1]);
    }
}
