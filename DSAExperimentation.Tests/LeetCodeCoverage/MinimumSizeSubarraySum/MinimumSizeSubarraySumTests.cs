using DSAExperimentation.LeetCode.MinimumSizeSubarraySum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumSizeSubarraySum;

// Harness only. Both strategies are MinimumSizeSubarraySumSolution's - this file
// just pins them to LeetCode's published examples, including the case where no
// subarray reaches target at all.
public sealed class MinimumSizeSubarraySumTests
{
    public static TheoryData<int, int[], int> Examples =>
        new()
        {
            { 7, [2, 3, 1, 2, 4, 3], 2 },
            { 4, [1, 4, 4], 1 },
            { 11, [1, 1, 1, 1, 1, 1, 1, 1], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinLengthByBruteForce_LeetCodeExamples_ReturnsShortestWindowLength(
        int target, int[] nums, int expected) =>
        Assert.Equal(expected, MinimumSizeSubarraySumSolution.MinLengthByBruteForce(target, nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinLengthByBinarySearchPrefixSum_LeetCodeExamples_ReturnsShortestWindowLength(
        int target, int[] nums, int expected) =>
        Assert.Equal(expected, MinimumSizeSubarraySumSolution.MinLengthByBinarySearchPrefixSum(target, nums));
}
