using DSAExperimentation.LeetCode.Solutions.MaximumBalancedSubsequenceSum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumBalancedSubsequenceSum;

// Harness only: the algorithms live in MaximumBalancedSubsequenceSumSolution. One
// test method per strategy over one shared set of cases, so a failure names the
// strategy that broke. The remaining case is a single element, whose value is the
// answer. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MaximumBalancedSubsequenceSumSolutionTests
{
    public static TheoryData<int[], long> Cases =>
        new()
        {
            { [1], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxBalancedSumByBruteForce_Cases_ReturnsMaximumBalancedSum(int[] nums, long expected)
    {
        var actual = MaximumBalancedSubsequenceSumSolution.MaxBalancedSumByBruteForce(nums);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxBalancedSumBySegmentTree_Cases_ReturnsMaximumBalancedSum(int[] nums, long expected)
    {
        var actual = MaximumBalancedSubsequenceSumSolution.MaxBalancedSumBySegmentTree(nums);

        Assert.Equal(expected, actual);
    }
}
