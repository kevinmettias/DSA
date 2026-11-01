using DSAExperimentation.LeetCode.MaximumSumOfMNonOverlappingSubarraysII;

namespace DSAExperimentation.LeetCode.Tests.MaximumSumOfMNonOverlappingSubarraysII;

// Harness only. Both strategies are MaximumSumOfMNonOverlappingSubarraysIISolution's -
// this file pins them to LeetCode's published examples, the same four Part I (LC 3956)
// uses, since Part II states the identical rules at a larger n, plus one tie the
// Lagrangian arm depends on and those examples never reach.
public sealed partial class MaximumSumOfMNonOverlappingSubarraysIISolutionTests
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

    // The binary search over the penalty needs the penalized optimum's subarray count to
    // fall as the penalty grows, and that holds only while equal transitions go to the
    // start carrying fewer subarrays. Three equal values make every split of the run offer
    // the same sum, so the tie is reached at once; with ties going the other way, the arm
    // answers 4.
    public static TheoryData<SubarraySumExample> TiedTransitionExamples =>
        new()
        {
            { new SubarraySumExample(Nums: [1, 1, 1], M: 2, L: 1, R: 3, Expected: 3) },
        };

    [Theory]
    [MemberData(nameof(TiedTransitionExamples))]
    public void MaximumSumByLagrangianRelaxation_TiedTransitions_PrefersFewerSubarrays(
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
