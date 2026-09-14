using DSAExperimentation.LeetCode.MaximumStrictlyIncreasingCellsInAMatrix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumStrictlyIncreasingCellsInAMatrix;

// Harness only. Both strategies are MaximumStrictlyIncreasingCellsInAMatrixSolution's -
// this file just pins them to LeetCode's published examples plus the tie-heavy cases
// that separate "strictly greater" from "greater or equal", which is where the
// equal-value batching in the sorted arm and the strict comparison in the memoized arm
// both have to agree.
public sealed class MaximumStrictlyIncreasingCellsInAMatrixTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[3, 1], [3, 4]], 2 },
            { [[1, 1], [1, 1]], 1 },
            { [[3, 1, 6], [-9, 5, 7]], 4 },
            { [[3, 3, 3], [3, 2, 3], [3, 3, 3]], 2 },
            { [[1, 2, 3, 4]], 4 },
            { [[5, 5], [5, 5]], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxIncreasingCellsByMemoizedRowColumnScan_LeetCodeExamples_ReturnsLongestStrictlyIncreasingWalk(
        int[][] mat, int expected) =>
        Assert.Equal(
            expected,
            MaximumStrictlyIncreasingCellsInAMatrixSolution.MaxIncreasingCellsByMemoizedRowColumnScan(mat));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxIncreasingCellsBySortedBatchDp_LeetCodeExamples_ReturnsLongestStrictlyIncreasingWalk(
        int[][] mat, int expected) =>
        Assert.Equal(
            expected,
            MaximumStrictlyIncreasingCellsInAMatrixSolution.MaxIncreasingCellsBySortedBatchDp(mat));
}
