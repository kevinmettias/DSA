using DSAExperimentation.LeetCode.MinimumAreaRectangle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAreaRectangle;

// Harness only. Both strategies are MinimumAreaRectangleSolution's; this file
// states LeetCode's published examples once and asserts every strategy against
// them - the linear-rescan baseline included, which the benchmark previously
// measured without anything checking its answer.
public sealed class MinimumAreaRectangleTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LC example 1: [[1,1],[1,3],[3,1],[3,3],[2,2]] -> 4
            { [[1, 1], [1, 3], [3, 1], [3, 3], [2, 2]], 4 },

            // LC example 2: [[1,1],[1,3],[3,1],[3,3],[4,1],[4,3]] -> 2
            { [[1, 1], [1, 3], [3, 1], [3, 3], [4, 1], [4, 3]], 2 },

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
    [MemberData(nameof(Examples))]
    public void MinAreaRectByLinearScan_LeetCodeExamples_ReturnsSmallestRectangleArea(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinimumAreaRectangleSolution.MinAreaRectByLinearScan(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinAreaRectBySetLookup_LeetCodeExamples_ReturnsSmallestRectangleArea(
        int[][] points, int expected) =>
        Assert.Equal(expected, MinimumAreaRectangleSolution.MinAreaRectBySetLookup(points));
}
