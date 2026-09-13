using DSAExperimentation.LeetCode.CircleAndRectangleOverlapping;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CircleAndRectangleOverlapping;

// Harness only. Both strategies are CircleAndRectangleOverlappingSolution's - the O(1)
// clamp-and-distance check and the lattice-point scan that used to live untested as the
// benchmark's baseline - pinned to LeetCode's published examples plus a centre inside
// the rectangle, a tangent circle, and a corner just out of reach.
public sealed class CircleAndRectangleOverlappingTests
{
    public static TheoryData<int, int, int, int, int, int, int, bool> Examples =>
        new()
        {
            { 1, 0, 0, 1, -1, 3, 1, true },
            { 1, 1, 1, 1, -3, 2, -1, false },
            { 1, 0, 0, -1, 0, 0, 1, true },
            { 1, 1, 1, -3, -3, 3, 3, true },
            { 1, 0, 0, 1, -3, 3, 3, true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckOverlapByClampedDistance_LeetCodeExamples_ReturnsWhetherShapesShareAPoint(
        int radius, int xCenter, int yCenter, int x1, int y1, int x2, int y2, bool expected) =>
        Assert.Equal(
            expected,
            CircleAndRectangleOverlappingSolution.CheckOverlapByClampedDistance(
                radius, xCenter, yCenter, x1, y1, x2, y2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckOverlapByLatticePointScan_LeetCodeExamples_ReturnsWhetherShapesShareAPoint(
        int radius, int xCenter, int yCenter, int x1, int y1, int x2, int y2, bool expected) =>
        Assert.Equal(
            expected,
            CircleAndRectangleOverlappingSolution.CheckOverlapByLatticePointScan(
                radius, xCenter, yCenter, x1, y1, x2, y2));
}
