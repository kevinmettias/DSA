using DSAExperimentation.LeetCode.Solutions.FallingSquares;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FallingSquares;

// Harness only. Both strategies are FallingSquaresSolution's - this file just pins
// them to a square landing squarely on an equal one, which stacks to twice its
// height, and three squares that never overlap, so the running maximum holds when
// a lower square lands. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class FallingSquaresSolutionTests
{
    public static TheoryData<int[][], List<int>> Cases =>
        new()
        {
            { [[100, 100], [100, 100]], [100, 200] },
            { [[1, 2], [5, 3], [10, 1]], [2, 3, 3] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HeightsByBruteForceOverlapScan_Cases_ReturnsRunningMaxHeights(
        int[][] positions, List<int> expected) =>
        Assert.Equal(expected, FallingSquaresSolution.HeightsByBruteForceOverlapScan(positions));

    [Theory]
    [MemberData(nameof(Cases))]
    public void HeightsByLazySegmentTree_Cases_ReturnsRunningMaxHeights(
        int[][] positions, List<int> expected) =>
        Assert.Equal(expected, FallingSquaresSolution.HeightsByLazySegmentTree(positions));
}
