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
    {
        var overlaps = CheckOverlap(radius: 1, xCenter: 0, yCenter: 0, rectangle: new Rectangle(1, -1, 3, 1));
        Assert.True(overlaps);
    }

    [Fact]
    public void CheckOverlap_CenterFullyInsideRectangle_ReturnsTrue()
    {
        var overlaps = CheckOverlap(radius: 1, xCenter: 1, yCenter: 1, rectangle: new Rectangle(-3, -3, 3, 3));
        Assert.True(overlaps);
    }

    [Fact]
    public void CheckOverlap_NearestCornerFartherThanRadius_ReturnsFalse()
    {
        var overlaps = CheckOverlap(radius: 1, xCenter: 1, yCenter: 1, rectangle: new Rectangle(1, -3, 2, -1));
        Assert.False(overlaps);
    }

    [Fact]
    public void CheckOverlap_CircleTangentToRectangleEdge_ReturnsTrue()
    {
        var overlaps = CheckOverlap(radius: 1, xCenter: 0, yCenter: 0, rectangle: new Rectangle(1, -3, 3, 3));
        Assert.True(overlaps);
    }

    private readonly record struct Rectangle(int X1, int Y1, int X2, int Y2);

    private static bool CheckOverlap(int radius, int xCenter, int yCenter, Rectangle rectangle)
    {
        var closestX = Math.Clamp(xCenter, rectangle.X1, rectangle.X2);
        var closestY = Math.Clamp(yCenter, rectangle.Y1, rectangle.Y2);
        var dx = (long)(xCenter - closestX);
        var dy = (long)(yCenter - closestY);
        return (dx * dx) + (dy * dy) <= (long)radius * radius;
    }
}
