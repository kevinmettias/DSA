using DSAExperimentation.LeetCode.CountOfRangeSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountOfRangeSum;

// Harness only. Both strategies are CountOfRangeSumSolution's - this file just pins
// them to LeetCode's published examples, including a large-magnitude case that
// proves the algorithm's long prefix-sum accumulator is load-bearing, not
// incidental.
public sealed class CountOfRangeSumTests
{
    public static TheoryData<int[], int, int, int> Examples =>
        new()
        {
            { [-2, 5, -1], -2, 2, 3 },
            { [0], 0, 0, 1 },
            { [1, 2, 3], 100, 200, 0 },
            { [int.MaxValue, int.MaxValue, -1, -1], 0, int.MaxValue, 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByPairwisePrefixScan_LeetCodeExamples_ReturnsRangeSumCount(
        int[] nums, int lower, int upper, int expected) =>
        Assert.Equal(expected, CountOfRangeSumSolution.CountByPairwisePrefixScan(nums, lower, upper));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByFenwickSweep_LeetCodeExamples_ReturnsRangeSumCount(
        int[] nums, int lower, int upper, int expected) =>
        Assert.Equal(expected, CountOfRangeSumSolution.CountByFenwickSweep(nums, lower, upper));
}
