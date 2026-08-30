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
        var area = TotalArea(-3, 0, 3, 4, 0, -1, 9, 2);

        Assert.Equal(45, area);
    }

    [Fact]
    public void TotalArea_TouchingButNotOverlapping_SumsBothAreasWithNoSubtraction()
    {
        // Second example: (-2,-2,2,2) and (-2,-2,2,2) fully coincide -> single area
        var area = TotalArea(-2, -2, 2, 2, -2, -2, 2, 2);

        Assert.Equal(16, area);
    }

    [Fact]
    public void TotalArea_DisjointRectangles_SumsBothAreasWithZeroOverlap()
    {
        var area = TotalArea(0, 0, 2, 2, 10, 10, 12, 12);

        Assert.Equal(8, area);
    }

    private static long TotalArea(int ax1, int ay1, int ax2, int ay2, int bx1, int by1, int bx2, int by2)
    {
        var area1 = (long)(ax2 - ax1) * (ay2 - ay1);
        var area2 = (long)(bx2 - bx1) * (by2 - by1);

        var overlapWidth = Math.Max(0, Math.Min(ax2, bx2) - Math.Max(ax1, bx1));
        var overlapHeight = Math.Max(0, Math.Min(ay2, by2) - Math.Max(ay1, by1));

        return area1 + area2 - (long)overlapWidth * overlapHeight;
    }
}
