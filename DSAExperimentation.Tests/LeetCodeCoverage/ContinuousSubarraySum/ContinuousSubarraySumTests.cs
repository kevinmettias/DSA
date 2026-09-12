using DSAExperimentation.LeetCode.ContinuousSubarraySum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContinuousSubarraySum;

// Harness only: both strategies live in ContinuousSubarraySumSolution and are asserted
// against the same examples, including the case that fails only because the qualifying
// remainder repeat is one index too close together.
public sealed class ContinuousSubarraySumTests
{
    public static TheoryData<int[], int, bool> Examples =>
        new()
        {
            { [23, 2, 4, 6, 7], 6, true },
            { [23, 2, 6, 4, 7], 6, true },
            { [23, 2, 6, 4, 7], 13, false },
            { [1, 2, 3], 5, true },
            { [1, 2, 12], 6, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasSubarraySumMultipleOfKByBruteForce_LeetCodeExamples_ReturnsExpected(int[] nums, int k, bool expected) =>
        Assert.Equal(expected, ContinuousSubarraySumSolution.HasSubarraySumMultipleOfKByBruteForce(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasSubarraySumMultipleOfKByHashMapPrefixRemainder_LeetCodeExamples_ReturnsExpected(int[] nums, int k, bool expected) =>
        Assert.Equal(expected, ContinuousSubarraySumSolution.HasSubarraySumMultipleOfKByHashMapPrefixRemainder(nums, k));
}
