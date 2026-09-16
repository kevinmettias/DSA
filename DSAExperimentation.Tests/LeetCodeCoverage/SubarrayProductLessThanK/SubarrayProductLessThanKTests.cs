using DSAExperimentation.LeetCode.SubarrayProductLessThanK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubarrayProductLessThanK;

// Harness only. Both strategies are SubarrayProductLessThanKSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class SubarrayProductLessThanKTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [10, 5, 2, 6], 100, 8 },
            { [1, 2, 3], 0, 0 },
            { [5], 10, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSubarraysWithProductLessThanKByBruteForce_LeetCodeExamples_ReturnsValidSubarrayCount(
        int[] nums, int productLimit, int expected)
    {
        var actual = SubarrayProductLessThanKSolution.CountSubarraysWithProductLessThanKByBruteForce(nums, productLimit);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSubarraysWithProductLessThanKByLogPrefixLowerBound_LeetCodeExamples_ReturnsValidSubarrayCount(
        int[] nums, int productLimit, int expected)
    {
        var actual = SubarrayProductLessThanKSolution.CountSubarraysWithProductLessThanKByLogPrefixLowerBound(nums, productLimit);

        Assert.Equal(expected, actual);
    }
}
