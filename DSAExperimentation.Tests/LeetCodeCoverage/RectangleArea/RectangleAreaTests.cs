using DSAExperimentation.LeetCode.RectangleArea;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RectangleArea;

// Harness only: both strategies are RectangleAreaSolution's - the closed-form O(1)
// overlap arithmetic and the unit-grid brute force it has to beat, now under test
// itself instead of sitting unasserted as a benchmark-only baseline.
public sealed class RectangleAreaTests
{
    public static TheoryData<int, int, int, int, int, int, int, int, long> Examples =>
        new()
        {
            // LeetCode's own example: (-3,0,3,4) and (0,-1,9,2) -> 45
            { -3, 0, 3, 4, 0, -1, 9, 2, 45 },
            // Second example: (-2,-2,2,2) and (-2,-2,2,2) fully coincide -> single area
            { -2, -2, 2, 2, -2, -2, 2, 2, 16 },
            // Disjoint rectangles: no overlap to subtract
            { 0, 0, 2, 2, 10, 10, 12, 12, 8 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalAreaByClosedFormOverlap_LeetCodeExamples_ReturnsExpectedArea(
        int ax1, int ay1, int ax2, int ay2, int bx1, int by1, int bx2, int by2, long expected) =>
        Assert.Equal(
            expected,
            RectangleAreaSolution.TotalAreaByClosedFormOverlap(ax1, ay1, ax2, ay2, bx1, by1, bx2, by2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalAreaByUnitGridCoverageCount_LeetCodeExamples_ReturnsExpectedArea(
        int ax1, int ay1, int ax2, int ay2, int bx1, int by1, int bx2, int by2, long expected) =>
        Assert.Equal(
            expected,
            RectangleAreaSolution.TotalAreaByUnitGridCoverageCount(ax1, ay1, ax2, ay2, bx1, by1, bx2, by2));
}
