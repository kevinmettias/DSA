using DSAExperimentation.LeetCode.PartitionArrayIntoTwoEqualProductSubsets;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionArrayIntoTwoEqualProductSubsets;

// Harness only. Both partition searches are
// PartitionArrayIntoTwoEqualProductSubsetsSolution's - this file just pins them to
// LeetCode's published examples.
public sealed class PartitionArrayIntoTwoEqualProductSubsetsTests
{
    public static TheoryData<int[], long, bool> Examples =>
        new()
        {
            { [3, 1, 6, 8, 4], 24, true },
            { [2, 5, 3, 7], 15, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckEqualPartitionsByBitmaskEnumeration_LeetCodeExamples_ReturnsWhetherAnEqualSplitExists(
        int[] nums, long target, bool expected) =>
        Assert.Equal(
            expected,
            PartitionArrayIntoTwoEqualProductSubsetsSolution.CheckEqualPartitionsByBitmaskEnumeration(nums, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CheckEqualPartitionsByPrunedBacktracking_LeetCodeExamples_ReturnsWhetherAnEqualSplitExists(
        int[] nums, long target, bool expected) =>
        Assert.Equal(
            expected,
            PartitionArrayIntoTwoEqualProductSubsetsSolution.CheckEqualPartitionsByPrunedBacktracking(nums, target));
}
