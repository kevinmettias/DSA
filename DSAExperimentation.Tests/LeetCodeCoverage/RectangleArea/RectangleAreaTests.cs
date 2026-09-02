namespace DSAExperimentation.Tests.LeetCodeCoverage.RectangleArea;

// LeetCode 223. Rectangle Area: total area covered by two axis-aligned rectangles
// is area1 + area2 - overlap, where overlap is the product of each axis's clamped
// intersection length. This reduces to O(1) scalar arithmetic (min/max clamps) -
// no repo container or algorithm primitive applies to a handful of long scalars,
// the same "lighter repo-primitive fit" case this repo already accepted for
// Pow(x, n) / Power of Two. See RectangleAreaBenchmarks.cs for a comparison
// against a brute-force unit-grid coverage count that DOES compose a repo
// primitive (DynamicArray<bool>).
public sealed partial class RectangleAreaTests
{
    [Fact]
    public void TotalArea_OverlappingSquares_SubtractsSharedRegionOnce()
    {
        // LeetCode's own example: (-3,0,3,4) and (0,-1,9,2) -> 45
        var area = TotalArea(new Rectangle(-3, 0, 3, 4), new Rectangle(0, -1, 9, 2));

        Assert.Equal(45, area);
    }

    [Fact]
    public void TotalArea_TouchingButNotOverlapping_SumsBothAreasWithNoSubtraction()
    {
        // Second example: (-2,-2,2,2) and (-2,-2,2,2) fully coincide -> single area
        var area = TotalArea(new Rectangle(-2, -2, 2, 2), new Rectangle(-2, -2, 2, 2));

        Assert.Equal(16, area);
    }

    [Fact]
    public void TotalArea_DisjointRectangles_SumsBothAreasWithZeroOverlap()
    {
        var area = TotalArea(new Rectangle(0, 0, 2, 2), new Rectangle(10, 10, 12, 12));

        Assert.Equal(8, area);
    }

    private readonly record struct Rectangle(int X1, int Y1, int X2, int Y2);

    private static long TotalArea(Rectangle a, Rectangle b)
    {
        var area1 = (long)(a.X2 - a.X1) * (a.Y2 - a.Y1);
        var area2 = (long)(b.X2 - b.X1) * (b.Y2 - b.Y1);

        var overlapWidth = Math.Max(0, Math.Min(a.X2, b.X2) - Math.Max(a.X1, b.X1));
        var overlapHeight = Math.Max(0, Math.Min(a.Y2, b.Y2) - Math.Max(a.Y1, b.Y1));

        return area1 + area2 - (long)overlapWidth * overlapHeight;
    }
}
