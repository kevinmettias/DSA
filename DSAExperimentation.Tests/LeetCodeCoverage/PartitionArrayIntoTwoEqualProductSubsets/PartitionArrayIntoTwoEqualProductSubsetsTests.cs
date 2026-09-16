using DSAExperimentation.LeetCode.PartitionArrayIntoTwoEqualProductSubsets;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionArrayIntoTwoEqualProductSubsets;

// Harness only. Both partition searches are
// PartitionArrayIntoTwoEqualProductSubsetsSolution's - this file just pins them to
// LeetCode's published examples.
public sealed class PartitionArrayIntoTwoEqualProductSubsetsTests
{
    public static TheoryData<ProductSubsetExample> Examples =>
        new()
        {
            { new ProductSubsetExample(Nums: [3, 1, 6, 8, 4], Target: 24, Expected: true) },
            { new ProductSubsetExample(Nums: [2, 5, 3, 7], Target: 15, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPartitionEquallyByBitmaskEnumeration_LeetCodeExamples_ReturnsWhetherAnEqualSplitExists(
        ProductSubsetExample example)
    {
        var actual = PartitionArrayIntoTwoEqualProductSubsetsSolution.CanPartitionEquallyByBitmaskEnumeration(
            example.Nums, example.Target);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPartitionEquallyByPrunedBacktracking_LeetCodeExamples_ReturnsWhetherAnEqualSplitExists(
        ProductSubsetExample example)
    {
        var actual = PartitionArrayIntoTwoEqualProductSubsetsSolution.CanPartitionEquallyByPrunedBacktracking(
            example.Nums, example.Target);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the numbers, the product each of the two subsets has to
    // reach, and whether such a split exists. The answer is the datum under test, so
    // the row names it rather than leaving a bare `bool` after the target.
    public readonly record struct ProductSubsetExample(int[] Nums, long Target, bool Expected);
}
