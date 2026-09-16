using DSAExperimentation.LeetCode.MaxSumOfRectangleNoLargerThanK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxSumOfRectangleNoLargerThanK;

// Harness only. Both strategies are MaxSumOfRectangleNoLargerThanKSolution's - this
// file pins them to LeetCode's classic example plus the single-row exact-limit
// case, so a failure names the strategy that broke.
public sealed class MaxSumOfRectangleNoLargerThanKTests
{
    public static TheoryData<int[][], int, int> Examples =>
        new()
        {
            { [[1, 0, 1], [0, -2, 3]], 2, 2 },
            { [[2, 2, -1]], 3, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumSubmatrixByBruteForceWindowScan_LeetCodeExamples_ReturnsLargestSumWithinLimit(
        int[][] matrix, int k, int expected)
    {
        var actual = MaxSumOfRectangleNoLargerThanKSolution.MaxSumSubmatrixByBruteForceWindowScan(matrix, k);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumSubmatrixByBstCeilingScan_LeetCodeExamples_ReturnsLargestSumWithinLimit(
        int[][] matrix, int k, int expected)
    {
        var actual = MaxSumOfRectangleNoLargerThanKSolution.MaxSumSubmatrixByBstCeilingScan(matrix, k);
        Assert.Equal(expected, actual);
    }
}
