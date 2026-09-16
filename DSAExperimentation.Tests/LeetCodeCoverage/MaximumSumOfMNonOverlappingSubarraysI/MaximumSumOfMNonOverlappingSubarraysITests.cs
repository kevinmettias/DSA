using DSAExperimentation.LeetCode.MaximumSumOfMNonOverlappingSubarraysI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumOfMNonOverlappingSubarraysI;

// Harness only. Both strategies are MaximumSumOfMNonOverlappingSubarraysISolution's -
// this file just pins them to LeetCode's published examples, including example 4's
// all-negative array, the case that catches an "at most j" DP into wrongly
// returning 0 by skipping every subarray.
public sealed partial class MaximumSumOfMNonOverlappingSubarraysITests
{
    public static TheoryData<SubarraySumExample> Examples =>
        new()
        {
            { new SubarraySumExample(Nums: [4, 1, -5, 2], M: 2, L: 1, R: 3, Expected: 7) },
            { new SubarraySumExample(Nums: [1, 0, 3, 4], M: 2, L: 1, R: 2, Expected: 8) },
            { new SubarraySumExample(Nums: [-1, 7, -4], M: 1, L: 2, R: 3, Expected: 6) },
            { new SubarraySumExample(Nums: [-3, -4, -1], M: 2, L: 1, R: 2, Expected: -1) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumByDynamicProgramming_LeetCodeExamples_ReturnsBestAtMostMSubarraySum(
        SubarraySumExample example)
    {
        var actual = MaximumSumOfMNonOverlappingSubarraysISolution.MaximumSumByDynamicProgramming(
            example.Nums, example.M, example.L, example.R);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumBySlidingWindowMaximum_LeetCodeExamples_ReturnsBestAtMostMSubarraySum(
        SubarraySumExample example)
    {
        var actual = MaximumSumOfMNonOverlappingSubarraysISolution.MaximumSumBySlidingWindowMaximum(
            example.Nums, example.M, example.L, example.R);

        Assert.Equal(example.Expected, actual);
    }

    // One example as one argument: the five values that describe a single case. They
    // travel together - a row IS one case - and passed separately they made a
    // five-parameter signature that could only be read by counting commas.
    public readonly record struct SubarraySumExample(int[] Nums, int M, int L, int R, long Expected);
}
