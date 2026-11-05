using DSAExperimentation.LeetCode.Solutions.CountOfRangeSum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountOfRangeSum;

// Harness only. Both strategies are CountOfRangeSumSolution's - this file pins them
// to cases the catalog lacks: a range no subarray sum reaches, and a large-magnitude
// case that proves the algorithm's long prefix-sum accumulator is load-bearing, not
// incidental. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CountOfRangeSumSolutionTests
{
    public static TheoryData<int[], int, int, int> Cases =>
        new()
        {
            { [1, 2, 3], 100, 200, 0 },
            { [int.MaxValue, int.MaxValue, -1, -1], 0, int.MaxValue, 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByPairwisePrefixScan_Cases_ReturnsRangeSumCount(
        int[] nums, int lower, int upper, int expected)
    {
        var actual = CountOfRangeSumSolution.CountByPairwisePrefixScan(nums, lower, upper);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByFenwickSweep_Cases_ReturnsRangeSumCount(
        int[] nums, int lower, int upper, int expected)
    {
        var actual = CountOfRangeSumSolution.CountByFenwickSweep(nums, lower, upper);
        Assert.Equal(expected, actual);
    }
}
