using DSAExperimentation.LeetCode.FallingSquares;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FallingSquares;

// Harness only. Both strategies are FallingSquaresSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class FallingSquaresTests
{
    public static TheoryData<int[][], List<int>> Examples =>
        new()
        {
            { [[1, 2], [2, 3], [6, 1]], [2, 5, 5] },
            { [[100, 100], [100, 100]], [100, 200] },
            { [[1, 2], [5, 3], [10, 1]], [2, 3, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HeightsByBruteForceOverlapScan_LeetCodeExamples_ReturnsRunningMaxHeights(
        int[][] positions, List<int> expected) =>
        Assert.Equal(expected, FallingSquaresSolution.HeightsByBruteForceOverlapScan(positions));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HeightsByLazySegmentTree_LeetCodeExamples_ReturnsRunningMaxHeights(
        int[][] positions, List<int> expected) =>
        Assert.Equal(expected, FallingSquaresSolution.HeightsByLazySegmentTree(positions));
}
