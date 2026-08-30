namespace DSAExperimentation.Tests.LeetCodeCoverage.CircleAndRectangleOverlapping;

// LeetCode 1401. Circle and Rectangle Overlapping: reduces to O(1) scalar
// arithmetic - clamp the circle's center to the rectangle's bounds to find
// the nearest point in the rectangle, then compare squared distance to
// radius^2. Same "lighter repo-primitive fit" case RectangleOverlapTests
// (LC 836) and RectangleAreaTests (LC 223) already document; no repo
// container or algorithm primitive applies to a handful of coordinate
// comparisons. See CircleAndRectangleOverlappingBenchmarks.cs for a
// comparison against a brute-force lattice-point scan that DOES compose a
// repo primitive (DynamicArray<bool>).
public sealed partial class CircleAndRectangleOverlappingTests
{
    [Fact]
    public void CheckOverlap_LeetCodeExampleOne_ReturnsTrue()
        => Assert.True(CheckOverlap(radius: 1, xCenter: 0, yCenter: 0, x1: 1, y1: -1, x2: 3, y2: 1));

    [Fact]
    public void CheckOverlap_CenterFullyInsideRectangle_ReturnsTrue()
        => Assert.True(CheckOverlap(radius: 1, xCenter: 1, yCenter: 1, x1: -3, y1: -3, x2: 3, y2: 3));

    [Fact]
    public void CheckOverlap_NearestCornerFartherThanRadius_ReturnsFalse()
        => Assert.False(CheckOverlap(radius: 1, xCenter: 1, yCenter: 1, x1: 1, y1: -3, x2: 2, y2: -1));

    [Fact]
    public void CheckOverlap_CircleTangentToRectangleEdge_ReturnsTrue()
        => Assert.True(CheckOverlap(radius: 1, xCenter: 0, yCenter: 0, x1: 1, y1: -3, x2: 3, y2: 3));

    private static bool CheckOverlap(int radius, int xCenter, int yCenter, int x1, int y1, int x2, int y2)
    {
        var closestX = Math.Clamp(xCenter, x1, x2);
        var closestY = Math.Clamp(yCenter, y1, y2);
        var dx = (long)(xCenter - closestX);
        var dy = (long)(yCenter - closestY);
        return (dx * dx) + (dy * dy) <= (long)radius * radius;
    }
}
