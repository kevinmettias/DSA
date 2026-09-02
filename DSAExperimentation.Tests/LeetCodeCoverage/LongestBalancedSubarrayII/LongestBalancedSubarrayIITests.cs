using DSAExperimentation.LeetCode.LongestBalancedSubarrayII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestBalancedSubarrayII;

// Harness only. Both strategies are LongestBalancedSubarrayIISolution's - this
// file pins them to LeetCode's published examples (the same examples as
// LongestBalancedSubarrayI, whose only difference from this problem is n's
// upper bound).
public sealed class LongestBalancedSubarrayIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 5, 4, 3], 4 },
            { [3, 2, 2, 5, 4], 5 },
            { [1, 2, 3, 2], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLongestBalancedLengthByBruteForce_LeetCodeExamples_ReturnsLongestBalancedLength(
        int[] nums, int expected) =>
        Assert.Equal(expected, LongestBalancedSubarrayIISolution.FindLongestBalancedLengthByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLongestBalancedLengthByPrefixBalanceSegmentTree_LeetCodeExamples_ReturnsLongestBalancedLength(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            LongestBalancedSubarrayIISolution.FindLongestBalancedLengthByPrefixBalanceSegmentTree(nums));
}
