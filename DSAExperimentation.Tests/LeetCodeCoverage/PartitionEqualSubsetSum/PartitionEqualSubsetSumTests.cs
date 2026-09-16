using DSAExperimentation.LeetCode.PartitionEqualSubsetSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionEqualSubsetSum;

// Harness only: both strategies live in PartitionEqualSubsetSumSolution and are
// asserted against the same examples, including the odd-total case that makes an
// equal split impossible before any subset-sum search runs.
public sealed partial class PartitionEqualSubsetSumTests
{
    public static TheoryData<SubsetSumExample> Examples =>
        new()
        {
            { new SubsetSumExample(Nums: [1, 5, 11, 5], Expected: true) },
            { new SubsetSumExample(Nums: [1, 2, 3, 5], Expected: false) },
            { new SubsetSumExample(Nums: [1, 2, 5], Expected: false) },
            { new SubsetSumExample(Nums: [1], Expected: false) },
            { new SubsetSumExample(Nums: [2, 2], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPartitionByTabulation_LeetCodeExamples_ReturnsWhetherEqualSplitExists(
        SubsetSumExample example) =>
        Assert.Equal(
            example.Expected, PartitionEqualSubsetSumSolution.CanPartitionByTabulation(example.Nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPartitionByMemoization_LeetCodeExamples_ReturnsWhetherEqualSplitExists(
        SubsetSumExample example) =>
        Assert.Equal(
            example.Expected, PartitionEqualSubsetSumSolution.CanPartitionByMemoization(example.Nums));

    // One LeetCode example: the numbers and whether they split into two subsets of
    // equal sum. The answer is the datum under test, so the row names it rather than
    // leaving a bare `bool` beside the array - `CanPartition(nums, true)` does not say
    // what `true` is.
    public readonly record struct SubsetSumExample(int[] Nums, bool Expected);
}
