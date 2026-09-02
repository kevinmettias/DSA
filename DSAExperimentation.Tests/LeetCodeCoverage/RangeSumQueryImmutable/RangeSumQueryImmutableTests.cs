using DSAExperimentation.LeetCode.RangeSumQueryImmutable;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeSumQueryImmutable;

// Harness only. Both strategies are RangeSumQueryImmutableSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class RangeSumQueryImmutableTests
{
    public static TheoryData<int[], int, int, int> Examples =>
        new()
        {
            { new[] { -2, 0, 3, -5, 2, -1 }, 0, 2, 1 },
            { new[] { -2, 0, 3, -5, 2, -1 }, 2, 5, -1 },
            { new[] { -2, 0, 3, -5, 2, -1 }, 0, 5, -3 },
            { new[] { 7, -3, 4 }, 1, 1, -3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumRangeByFenwickTree_LeetCodeExamples_ReturnsRangeSum(int[] nums, int left, int right, int expected) =>
        Assert.Equal(expected, RangeSumQueryImmutableSolution.SumRangeByFenwickTree(nums, left, right));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumRangeByBruteForceRescan_LeetCodeExamples_ReturnsRangeSum(int[] nums, int left, int right, int expected) =>
        Assert.Equal(expected, RangeSumQueryImmutableSolution.SumRangeByBruteForceRescan(nums, left, right));
}
