using DSAExperimentation.LeetCode.XORAfterRangeMultiplicationQueriesI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.XORAfterRangeMultiplicationQueriesI;

// Harness only. Both query-walk strategies are
// XORAfterRangeMultiplicationQueriesISolution's - this file just pins them to
// LeetCode's published examples.
public sealed class XORAfterRangeMultiplicationQueriesITests
{
    public static TheoryData<int[], int[][], int> Examples =>
        new()
        {
            { [1, 1, 1], [[0, 2, 1, 4]], 4 },
            { [2, 3, 1, 5, 4], [[1, 4, 2, 3], [0, 2, 1, 2]], 31 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void XorAfterQueriesByRangeScan_LeetCodeExamples_ReturnsXorOfFinalArray(
        int[] nums, int[][] queries, int expected) =>
        Assert.Equal(expected, XORAfterRangeMultiplicationQueriesISolution.XorAfterQueriesByRangeScan(nums, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void XorAfterQueriesByStridedWalk_LeetCodeExamples_ReturnsXorOfFinalArray(
        int[] nums, int[][] queries, int expected) =>
        Assert.Equal(
            expected, XORAfterRangeMultiplicationQueriesISolution.XorAfterQueriesByStridedWalk(nums, queries));
}
