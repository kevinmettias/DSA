using DSAExperimentation.LeetCode.GoodSubsequenceQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GoodSubsequenceQueries;

// Harness only. Both strategies are GoodSubsequenceQueriesSolution's - this file
// just pins them to LeetCode's published examples, including Example 2's run of
// two consecutive "yes" answers after the first update settles as "no".
public sealed class GoodSubsequenceQueriesTests
{
    public static TheoryData<int[], int, int[][], int> Examples =>
        new()
        {
            { [4, 8, 12, 16], 2, [[0, 3], [2, 6]], 1 },
            { [4, 5, 7, 8], 3, [[0, 6], [1, 9], [2, 3]], 2 },
            { [5, 7, 9], 2, [[1, 4], [2, 8]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodSubseqByBruteForce_LeetCodeExamples_ReturnsQueriesWithAGoodSubsequence(
        int[] nums, int p, int[][] queries, int expected) =>
        Assert.Equal(expected, GoodSubsequenceQueriesSolution.CountGoodSubseqByBruteForce(nums, p, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodSubseqBySegmentTreeGcd_LeetCodeExamples_ReturnsQueriesWithAGoodSubsequence(
        int[] nums, int p, int[][] queries, int expected) =>
        Assert.Equal(expected, GoodSubsequenceQueriesSolution.CountGoodSubseqBySegmentTreeGcd(nums, p, queries));
}
