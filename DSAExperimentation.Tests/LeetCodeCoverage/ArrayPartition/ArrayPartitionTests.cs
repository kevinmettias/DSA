using DSAExperimentation.LeetCode.ArrayPartition;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ArrayPartition;

// LeetCode 561. Array Partition: both strategies must maximize the sum of
// pair-minimums for the same input, so each is asserted here under its own
// name - see ArrayPartitionSolution for the strategies themselves.
public sealed class ArrayPartitionTests
{
    public static TheoryData<int[], int> Examples => new()
    {
        { [1, 4, 3, 2], 4 },
        { [6, 2, 6, 5, 1, 2], 9 },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumByRepeatedSmallestPairScan_Example_ReturnsMaximizedMinPairSum(int[] nums, int expected) => Assert.Equal(expected, ArrayPartitionSolution.MaxSumByRepeatedSmallestPairScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumByMergeSort_Example_ReturnsMaximizedMinPairSum(int[] nums, int expected) => Assert.Equal(expected, ArrayPartitionSolution.MaxSumByMergeSort(nums));
}
