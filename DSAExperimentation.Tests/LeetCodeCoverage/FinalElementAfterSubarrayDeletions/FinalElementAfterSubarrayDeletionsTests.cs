using DSAExperimentation.LeetCode.FinalElementAfterSubarrayDeletions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FinalElementAfterSubarrayDeletions;

// Harness only: all three strategies live in
// FinalElementAfterSubarrayDeletionsSolution and are asserted against the
// same examples, so a failure names the strategy that broke. The two full
// game-tree strategies are exercised on the same small inputs as the O(1)
// closed form - proving the shortcut right rather than merely asserting it.
public sealed partial class FinalElementAfterSubarrayDeletionsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 5, 2], 2 },
            { [3, 7], 7 },
            { [9], 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FinalElementByDictionaryMinimax_LeetCodeExamples_ReturnsOptimalPlayResult(int[] nums, int expected) =>
        Assert.Equal(expected, FinalElementAfterSubarrayDeletionsSolution.FinalElementByDictionaryMinimax(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FinalElementByMemoizedMinimax_LeetCodeExamples_ReturnsOptimalPlayResult(int[] nums, int expected) =>
        Assert.Equal(expected, FinalElementAfterSubarrayDeletionsSolution.FinalElementByMemoizedMinimax(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FinalElementByEndpointComparison_LeetCodeExamples_ReturnsOptimalPlayResult(int[] nums, int expected) =>
        Assert.Equal(expected, FinalElementAfterSubarrayDeletionsSolution.FinalElementByEndpointComparison(nums));
}
