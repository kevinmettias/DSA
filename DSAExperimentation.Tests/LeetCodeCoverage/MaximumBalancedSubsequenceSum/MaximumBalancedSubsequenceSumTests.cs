using DSAExperimentation.LeetCode.MaximumBalancedSubsequenceSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumBalancedSubsequenceSum;

// Harness only: the algorithms live in MaximumBalancedSubsequenceSumSolution. One
// test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke.
public sealed partial class MaximumBalancedSubsequenceSumTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [3, 3, 5, 6], 14 },
            { [5, -1, -3, 8], 13 },
            { [-2, -1], -1 },
            { [1], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxBalancedSumByBruteForce_LeetCodeExamples_ReturnsMaximumBalancedSum(int[] nums, long expected)
    {
        var actual = MaximumBalancedSubsequenceSumSolution.MaxBalancedSumByBruteForce(nums);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxBalancedSumBySegmentTree_LeetCodeExamples_ReturnsMaximumBalancedSum(int[] nums, long expected)
    {
        var actual = MaximumBalancedSubsequenceSumSolution.MaxBalancedSumBySegmentTree(nums);

        Assert.Equal(expected, actual);
    }
}
