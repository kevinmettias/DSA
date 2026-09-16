using DSAExperimentation.LeetCode.MinimumCostToDivideArrayIntoSubarrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToDivideArrayIntoSubarrays;

// Harness only. Both strategies are
// MinimumCostToDivideArraysIntoSubarraysSolution's - this file just pins them to
// LeetCode's published examples.
public sealed class MinimumCostToDivideArrayIntoSubarraysTests
{
    public static TheoryData<int[], int[], int, long> Examples =>
        new()
        {
            { [3, 1, 4], [4, 6, 6], 1, 110 },
            { [4, 8, 5, 1, 14, 2, 2, 12, 1], [7, 2, 8, 4, 2, 2, 1, 1, 2], 7, 985 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByDictionaryMemo_LeetCodeExamples_ReturnsMinimumTotalCost(
        int[] nums, int[] cost, int indexMultiplier, long expected)
    {
        var actual = MinimumCostToDivideArrayIntoSubarraysSolution.MinimumCostByDictionaryMemo(
            nums, cost, indexMultiplier);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByMemoizedPartition_LeetCodeExamples_ReturnsMinimumTotalCost(
        int[] nums, int[] cost, int indexMultiplier, long expected)
    {
        var actual = MinimumCostToDivideArrayIntoSubarraysSolution.MinimumCostByMemoizedPartition(
            nums, cost, indexMultiplier);

        Assert.Equal(expected, actual);
    }
}
