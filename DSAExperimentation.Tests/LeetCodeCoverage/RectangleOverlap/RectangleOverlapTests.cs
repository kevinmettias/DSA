namespace DSAExperimentation.Tests.LeetCodeCoverage.RectangleOverlap;

// LeetCode 836. Rectangle Overlap: two axis-aligned rectangles overlap (with
// positive area) exactly when both axes' intervals overlap strictly - the same
// "lighter repo-primitive fit" O(1) scalar-arithmetic case RectangleAreaTests
// already documents for LC 223; no repo container or algorithm primitive applies
// to a handful of coordinate comparisons. See RectangleOverlapBenchmarks.cs for a
// comparison against a brute-force unit-grid intersection scan that DOES compose
// a repo primitive (DynamicArray<bool>).
public sealed partial class RectangleOverlapTests
{
    [Fact]
    public void IsRectangleOverlap_OverlappingSquares_ReturnsTrue()
    {
        int[] rec1 = [0, 0, 2, 2];
        int[] rec2 = [1, 1, 3, 3];

        Assert.True(IsRectangleOverlap(rec1, rec2));
    }

    [Fact]
    public void IsRectangleOverlap_TouchingAtAnEdgeOnly_ReturnsFalse()
    {
        int[] rec1 = [0, 0, 1, 1];
        int[] rec2 = [1, 0, 2, 1];

        Assert.False(IsRectangleOverlap(rec1, rec2));
    }

    [Fact]
    public void IsRectangleOverlap_Disjoint_ReturnsFalse()
    {
        int[] rec1 = [0, 0, 1, 1];
        int[] rec2 = [2, 2, 3, 3];

        Assert.False(IsRectangleOverlap(rec1, rec2));
    }

    [Fact]
    public void IsRectangleOverlap_OneRectangleFullyInsideAnother_ReturnsTrue()
    {
        int[] rec1 = [0, 0, 10, 10];
        int[] rec2 = [3, 3, 5, 5];

        Assert.True(IsRectangleOverlap(rec1, rec2));
    }

    private static bool IsRectangleOverlap(int[] rec1, int[] rec2)
        => rec1[0] < rec2[2] && rec2[0] < rec1[2] && rec1[1] < rec2[3] && rec2[1] < rec1[3];
}
