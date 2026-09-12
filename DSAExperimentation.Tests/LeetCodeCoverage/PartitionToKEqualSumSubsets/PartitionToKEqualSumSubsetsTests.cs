using DSAExperimentation.LeetCode.PartitionToKEqualSumSubsets;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionToKEqualSumSubsets;

// Harness only. Both strategies are PartitionToKEqualSumSubsetsSolution's - this
// file just pins them to LeetCode's published examples.
public sealed class PartitionToKEqualSumSubsetsTests
{
    public static TheoryData<int[], int, bool> Examples =>
        new()
        {
            { [4, 3, 2, 3, 5, 2, 1], 4, true },
            { [1, 2, 3, 4], 3, false },
            { [2, 2, 2, 2, 3, 4, 5], 4, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPartitionKSubsetsByNaiveBacktracking_LeetCodeExamples_ReturnsWhetherKEqualBucketsExist(
        int[] nums, int k, bool expected) =>
        Assert.Equal(expected, PartitionToKEqualSumSubsetsSolution.CanPartitionKSubsetsByNaiveBacktracking(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPartitionKSubsetsByGenericBacktrack_LeetCodeExamples_ReturnsWhetherKEqualBucketsExist(
        int[] nums, int k, bool expected) =>
        Assert.Equal(expected, PartitionToKEqualSumSubsetsSolution.CanPartitionKSubsetsByGenericBacktrack(nums, k));
}
