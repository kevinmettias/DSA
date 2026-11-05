using DSAExperimentation.LeetCode.Solutions.MaximumStrictlyIncreasingCellsInAMatrix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumStrictlyIncreasingCellsInAMatrix;

// Harness only. Both strategies are MaximumStrictlyIncreasingCellsInAMatrixSolution's -
// this file just pins them to the tie-heavy cases
// that separate "strictly greater" from "greater or equal", which is where the
// equal-value batching in the sorted arm and the strict comparison in the memoized arm
// both have to agree.
public sealed partial class MaximumStrictlyIncreasingCellsInAMatrixSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[3, 3, 3], [3, 2, 3], [3, 3, 3]], 2 },
            { [[1, 2, 3, 4]], 4 },
            { [[5, 5], [5, 5]], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxIncreasingCellsByMemoizedRowColumnScan_Cases_ReturnsLongestStrictlyIncreasingWalk(
        int[][] mat, int expected) =>
        Assert.Equal(
            expected,
            MaximumStrictlyIncreasingCellsInAMatrixSolution.MaxIncreasingCellsByMemoizedRowColumnScan(mat));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxIncreasingCellsBySortedBatchDp_Cases_ReturnsLongestStrictlyIncreasingWalk(
        int[][] mat, int expected) =>
        Assert.Equal(
            expected,
            MaximumStrictlyIncreasingCellsInAMatrixSolution.MaxIncreasingCellsBySortedBatchDp(mat));
}
