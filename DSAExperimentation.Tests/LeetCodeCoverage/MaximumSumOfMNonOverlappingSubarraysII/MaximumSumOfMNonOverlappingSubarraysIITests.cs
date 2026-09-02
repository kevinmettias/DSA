using DSAExperimentation.LeetCode.MaximumSumOfMNonOverlappingSubarraysII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumOfMNonOverlappingSubarraysII;

// Harness only. Both strategies are MaximumSumOfMNonOverlappingSubarraysIISolution's -
// this file just pins them to LeetCode's published examples, the same four Part I
// (LC 3956) uses, since Part II states the identical rules at a larger n.
public sealed class MaximumSumOfMNonOverlappingSubarraysIITests
{
    public static TheoryData<int[], int, int, int, long> Examples =>
        new()
        {
            { [4, 1, -5, 2], 2, 1, 3, 7 },
            { [1, 0, 3, 4], 2, 1, 2, 8 },
            { [-1, 7, -4], 1, 2, 3, 6 },
            { [-3, -4, -1], 2, 1, 2, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumByDynamicProgramming_LeetCodeExamples_ReturnsBestAtMostMSubarraySum(
        int[] nums, int m, int l, int r, long expected) =>
        Assert.Equal(expected, MaximumSumOfMNonOverlappingSubarraysIISolution.MaximumSumByDynamicProgramming(nums, m, l, r));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumByLagrangianRelaxation_LeetCodeExamples_ReturnsBestAtMostMSubarraySum(
        int[] nums, int m, int l, int r, long expected) =>
        Assert.Equal(expected, MaximumSumOfMNonOverlappingSubarraysIISolution.MaximumSumByLagrangianRelaxation(nums, m, l, r));
}
