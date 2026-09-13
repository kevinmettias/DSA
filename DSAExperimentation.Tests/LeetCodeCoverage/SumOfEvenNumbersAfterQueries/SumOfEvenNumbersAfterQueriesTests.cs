using DSAExperimentation.LeetCode.SumOfEvenNumbersAfterQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfEvenNumbersAfterQueries;

// Harness only: both strategies live in SumOfEvenNumbersAfterQueriesSolution and
// are asserted against the same examples - LeetCode's published array and its four
// queries, a lone odd entry that stays odd, repeated queries against one index that
// flip it odd and back, and negative values (where C#'s % is negative for odd
// entries, so "even" has to be tested as % 2 == 0 rather than == 1).
public sealed class SumOfEvenNumbersAfterQueriesTests
{
    public static TheoryData<int[], int[][], int[]> Examples =>
        new()
        {
            { [1, 2, 3, 4], [[1, 0], [-3, 1], [-4, 0], [2, 3]], [8, 6, 2, 4] },
            { [1], [[4, 0]], [0] },
            { [2, 4], [[1, 0], [1, 0]], [4, 8] },
            { [-2, -1], [[-3, 1], [2, 0]], [-6, -4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumEvenAfterQueriesByRescan_LeetCodeExamples_ReturnsEvenSumAfterEachQuery(
        int[] nums, int[][] queries, int[] expected) =>
        Assert.Equal(expected, SumOfEvenNumbersAfterQueriesSolution.SumEvenAfterQueriesByRescan(nums, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumEvenAfterQueriesByRunningEvenSum_LeetCodeExamples_ReturnsEvenSumAfterEachQuery(
        int[] nums, int[][] queries, int[] expected) =>
        Assert.Equal(
            expected,
            SumOfEvenNumbersAfterQueriesSolution.SumEvenAfterQueriesByRunningEvenSum(nums, queries));
}
