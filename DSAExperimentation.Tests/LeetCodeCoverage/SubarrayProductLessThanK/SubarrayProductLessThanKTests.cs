using DSAExperimentation.LeetCode.SubarrayProductLessThanK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubarrayProductLessThanK;

// Harness only. Both strategies are SubarrayProductLessThanKSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class SubarrayProductLessThanKTests
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
    public void NumSubarrayProductLessThanKByBruteForce_LeetCodeExamples_ReturnsValidSubarrayCount(
        int[] nums, int k, int expected) =>
        Assert.Equal(expected, SubarrayProductLessThanKSolution.NumSubarrayProductLessThanKByBruteForce(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumSubarrayProductLessThanKByLogPrefixLowerBound_LeetCodeExamples_ReturnsValidSubarrayCount(
        int[] nums, int k, int expected) =>
        Assert.Equal(
            expected,
            SubarrayProductLessThanKSolution.NumSubarrayProductLessThanKByLogPrefixLowerBound(nums, k));
}
