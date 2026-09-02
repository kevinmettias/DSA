using DSAExperimentation.LeetCode.MinimumIncrementsForTargetMultiplesInAnArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumIncrementsForTargetMultiplesInAnArray;

// Harness only. Both strategies are
// MinimumIncrementsForTargetMultiplesInAnArraySolution's - this file just pins them
// to LeetCode's published examples.
public sealed class MinimumIncrementsForTargetMultiplesInAnArrayTests
{
    public static TheoryData<int[], int[], long> Examples =>
        new()
        {
            { [1, 2, 3], [4], 1L },
            { [8, 4], [10, 5], 2L },
            { [7, 9, 10], [7], 0L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinIncrementsByBottomUpBitmaskDp_LeetCodeExamples_ReturnsMinimumIncrements(
        int[] nums, int[] target, long expected) =>
        Assert.Equal(
            expected,
            MinimumIncrementsForTargetMultiplesInAnArraySolution.MinIncrementsByBottomUpBitmaskDp(nums, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinIncrementsByMemoizedBitmaskDp_LeetCodeExamples_ReturnsMinimumIncrements(
        int[] nums, int[] target, long expected) =>
        Assert.Equal(
            expected,
            MinimumIncrementsForTargetMultiplesInAnArraySolution.MinIncrementsByMemoizedBitmaskDp(nums, target));
}
