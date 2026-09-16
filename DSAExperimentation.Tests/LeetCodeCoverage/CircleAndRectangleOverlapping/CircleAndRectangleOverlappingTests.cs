using DSAExperimentation.LeetCode.CircleAndRectangleOverlapping;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CircleAndRectangleOverlapping;

// Harness only. Both strategies are CircleAndRectangleOverlappingSolution's - the O(1)
// clamp-and-distance check and the lattice-point scan that used to live untested as the
// benchmark's baseline - pinned to LeetCode's published examples plus a centre inside
// the rectangle, a tangent circle, and a corner just out of reach.
public sealed partial class CircleAndRectangleOverlappingTests
{
    public static TheoryData<OverlapCase> Examples =>
        new()
        {
            { new OverlapCase(Radius: 1, XCenter: 0, YCenter: 0, X1: 1, Y1: -1, X2: 3, Y2: 1, Expected: true) },
            { new OverlapCase(Radius: 1, XCenter: 1, YCenter: 1, X1: 1, Y1: -3, X2: 2, Y2: -1, Expected: false) },
            { new OverlapCase(Radius: 1, XCenter: 0, YCenter: 0, X1: -1, Y1: 0, X2: 0, Y2: 1, Expected: true) },
            { new OverlapCase(Radius: 1, XCenter: 1, YCenter: 1, X1: -3, Y1: -3, X2: 3, Y2: 3, Expected: true) },
            { new OverlapCase(Radius: 1, XCenter: 0, YCenter: 0, X1: 1, Y1: -3, X2: 3, Y2: 3, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasOverlapByClampedDistance_LeetCodeExamples_ReturnsWhetherShapesShareAPoint(
        OverlapCase example)
    {
        var overlaps = CircleAndRectangleOverlappingSolution.HasOverlapByClampedDistance(
            new Circle(example.Radius, example.XCenter, example.YCenter),
            new Rectangle(example.X1, example.Y1, example.X2, example.Y2));

        Assert.Equal(example.Expected, overlaps);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasOverlapByLatticePointScan_LeetCodeExamples_ReturnsWhetherShapesShareAPoint(
        OverlapCase example)
    {
        var overlaps = CircleAndRectangleOverlappingSolution.HasOverlapByLatticePointScan(
            new Circle(example.Radius, example.XCenter, example.YCenter),
            new Rectangle(example.X1, example.Y1, example.X2, example.Y2));

        Assert.Equal(example.Expected, overlaps);
    }

    // One LeetCode example: the circle and the rectangle that either do or do not share a
    // point, with every number named where it is passed rather than left in a run of eight
    // that only position separates.
    //
    // The row holds the two shapes' numbers rather than the solution's own Circle and
    // Rectangle, for one reason: both are internal to CircleAndRectangleOverlappingSolution,
    // and a public row cannot name an internal type (CS0053). The row is the only place the
    // two shapes are written out in the raw, and it labels each number as it goes.
    public readonly record struct OverlapCase(
        int Radius, int XCenter, int YCenter, int X1, int Y1, int X2, int Y2, bool Expected);
}
