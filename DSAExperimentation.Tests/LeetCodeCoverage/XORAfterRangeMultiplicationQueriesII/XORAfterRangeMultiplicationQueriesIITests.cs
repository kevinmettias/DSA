using DSAExperimentation.LeetCode.XORAfterRangeMultiplicationQueriesII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.XORAfterRangeMultiplicationQueriesII;

// Harness only. Both query-application strategies are
// XORAfterRangeMultiplicationQueriesIISolution's - this file just pins them to
// LeetCode's published examples, the same two Part I uses.
public sealed class XORAfterRangeMultiplicationQueriesIITests
{
    public static TheoryData<int[], int[][], int> Examples =>
        new()
        {
            { [1, 1, 1], [[0, 2, 1, 4]], 4 },
            { [2, 3, 1, 5, 4], [[1, 4, 2, 3], [0, 2, 1, 2]], 31 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void XorAfterQueriesByStridedWalk_LeetCodeExamples_ReturnsXorOfFinalArray(
        int[] nums, int[][] queries, int expected)
    {
        var actual = XORAfterRangeMultiplicationQueriesIISolution.XorAfterQueriesByStridedWalk(nums, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void XorAfterQueriesBySqrtDecomposition_LeetCodeExamples_ReturnsXorOfFinalArray(
        int[] nums, int[][] queries, int expected)
    {
        var actual = XORAfterRangeMultiplicationQueriesIISolution.XorAfterQueriesBySqrtDecomposition(
            nums, queries);

        Assert.Equal(expected, actual);
    }
}
