using DSAExperimentation.LeetCode.Solutions.MinimumAreaRectangle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumAreaRectangle;

// Harness only. Both strategies are MinimumAreaRectangleSolution's; this file
// states its cases once and asserts every strategy against them - the
// linear-rescan baseline included, which the benchmark previously measured without
// anything checking its answer. The cases are point sets that form no rectangle
// and one that forms exactly one. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MinimumAreaRectangleSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // Three corners only - no axis-aligned rectangle exists.
            { [[1, 1], [1, 3], [3, 1]], 0 },

            // A single point cannot form a diagonal at all.
            { [[0, 0]], 0 },

            // All points collinear: every candidate pair shares a coordinate.
            { [[0, 0], [0, 1], [0, 2], [0, 3]], 0 },

            // Exactly one rectangle, with sides of different lengths.
            { [[0, 0], [0, 5], [2, 0], [2, 5]], 10 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinAreaRectByLinearScan_Cases_ReturnsSmallestRectangleArea(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinimumAreaRectangleSolution.MinAreaRectByLinearScan(points));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinAreaRectBySetLookup_Cases_ReturnsSmallestRectangleArea(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinimumAreaRectangleSolution.MinAreaRectBySetLookup(points));
}
