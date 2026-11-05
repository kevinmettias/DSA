using DSAExperimentation.LeetCode.Solutions.PartitionEqualSubsetSum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PartitionEqualSubsetSum;

// Harness only: both strategies live in PartitionEqualSubsetSumSolution and are
// asserted against the same cases, among them the odd-total case that makes an
// equal split impossible before any subset-sum search runs.
public sealed partial class PartitionEqualSubsetSumSolutionTests
{
    public static TheoryData<SubsetSumCase> Cases =>
        new()
        {
            { new SubsetSumCase(Nums: [1, 2, 5], Expected: false) },
            { new SubsetSumCase(Nums: [1], Expected: false) },
            { new SubsetSumCase(Nums: [2, 2], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanPartitionByTabulation_Cases_ReturnsWhetherEqualSplitExists(
        SubsetSumCase testCase) =>
        Assert.Equal(
            testCase.Expected, PartitionEqualSubsetSumSolution.CanPartitionByTabulation(testCase.Nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanPartitionByMemoization_Cases_ReturnsWhetherEqualSplitExists(
        SubsetSumCase testCase) =>
        Assert.Equal(
            testCase.Expected, PartitionEqualSubsetSumSolution.CanPartitionByMemoization(testCase.Nums));

    // One case: the numbers and whether they split into two subsets of
    // equal sum. The answer is the datum under test, so the row names it rather than
    // leaving a bare `bool` beside the array - `CanPartition(nums, true)` does not say
    // what `true` is.
    public readonly record struct SubsetSumCase(int[] Nums, bool Expected);
}
