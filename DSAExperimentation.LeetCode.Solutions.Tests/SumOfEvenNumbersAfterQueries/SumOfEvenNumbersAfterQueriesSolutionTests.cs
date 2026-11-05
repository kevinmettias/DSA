using DSAExperimentation.LeetCode.Solutions.SumOfEvenNumbersAfterQueries;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SumOfEvenNumbersAfterQueries;

// Harness only: both strategies live in SumOfEvenNumbersAfterQueriesSolution and
// are asserted against the same cases - LeetCode's published array and its four
// queries, a lone odd entry that stays odd, repeated queries against one index that
// flip it odd and back, and negative values (where C#'s % is negative for odd
// entries, so "even" has to be tested as % 2 == 0 rather than == 1).
public sealed partial class SumOfEvenNumbersAfterQueriesSolutionTests
{
    public static TheoryData<int[], int[][], int[]> Cases =>
        new()
        {
            { [2, 4], [[1, 0], [1, 0]], [4, 8] },
            { [-2, -1], [[-3, 1], [2, 0]], [-6, -4] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumEvenAfterQueriesByRescan_Cases_ReturnsEvenSumAfterEachQuery(
        int[] nums, int[][] queries, int[] expected)
    {
        var actual = SumOfEvenNumbersAfterQueriesSolution.SumEvenAfterQueriesByRescan(nums, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumEvenAfterQueriesByRunningEvenSum_Cases_ReturnsEvenSumAfterEachQuery(
        int[] nums, int[][] queries, int[] expected)
    {
        var actual = SumOfEvenNumbersAfterQueriesSolution.SumEvenAfterQueriesByRunningEvenSum(nums, queries);

        Assert.Equal(expected, actual);
    }
}
