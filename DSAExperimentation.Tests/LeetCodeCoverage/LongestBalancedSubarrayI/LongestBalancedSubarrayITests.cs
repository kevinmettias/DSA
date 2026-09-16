using DSAExperimentation.LeetCode.LongestBalancedSubarrayI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestBalancedSubarrayI;

// Harness only. Both strategies are LongestBalancedSubarrayISolution's - this
// file pins them to LeetCode's published examples.
public sealed partial class LongestBalancedSubarrayITests
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
        Assert.Equal(expected, LongestBalancedSubarrayISolution.FindLongestBalancedLengthByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLongestBalancedLengthByDistinctSetScan_LeetCodeExamples_ReturnsLongestBalancedLength(
        int[] nums, int expected) =>
        Assert.Equal(expected, LongestBalancedSubarrayISolution.FindLongestBalancedLengthByDistinctSetScan(nums));
}
