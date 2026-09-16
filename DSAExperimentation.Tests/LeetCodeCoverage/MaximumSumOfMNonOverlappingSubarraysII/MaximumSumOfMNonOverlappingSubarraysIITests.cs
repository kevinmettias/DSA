using DSAExperimentation.LeetCode.MaximumSumOfMNonOverlappingSubarraysII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumOfMNonOverlappingSubarraysII;

// Harness only. Both strategies are MaximumSumOfMNonOverlappingSubarraysIISolution's -
// this file just pins them to LeetCode's published examples, the same four Part I
// (LC 3956) uses, since Part II states the identical rules at a larger n.
public sealed class MaximumSumOfMNonOverlappingSubarraysIITests
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
        var actual = MaximumSumOfMNonOverlappingSubarraysIISolution.MaximumSumByDynamicProgramming(
            example.Nums, example.M, example.L, example.R);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumByLagrangianRelaxation_LeetCodeExamples_ReturnsBestAtMostMSubarraySum(
        SubarraySumExample example)
    {
        var actual = MaximumSumOfMNonOverlappingSubarraysIISolution.MaximumSumByLagrangianRelaxation(
            example.Nums, example.M, example.L, example.R);

        Assert.Equal(example.Expected, actual);
    }

    // One example as one argument: the five values that describe a single case. They
    // travel together - a row IS one case - and passed separately they made a
    // five-parameter signature that could only be read by counting commas.
    public readonly record struct SubarraySumExample(int[] Nums, int M, int L, int R, long Expected);
}
