using DSAExperimentation.LeetCode.PartitionEqualSubsetSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionEqualSubsetSum;

// Harness only: both strategies live in PartitionEqualSubsetSumSolution and are
// asserted against the same examples, including the odd-total case that makes an
// equal split impossible before any subset-sum search runs.
public sealed class PartitionEqualSubsetSumTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { new[] { 1, 5, 11, 5 }, true },
            { new[] { 1, 2, 3, 5 }, false },
            { new[] { 1, 2, 5 }, false },
            { new[] { 1 }, false },
            { new[] { 2, 2 }, true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPartitionByTabulation_LeetCodeExamples_ReturnsWhetherEqualSplitExists(
        int[] nums, bool expected) =>
        Assert.Equal(expected, PartitionEqualSubsetSumSolution.CanPartitionByTabulation(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPartitionByMemoization_LeetCodeExamples_ReturnsWhetherEqualSplitExists(
        int[] nums, bool expected) =>
        Assert.Equal(expected, PartitionEqualSubsetSumSolution.CanPartitionByMemoization(nums));
}
