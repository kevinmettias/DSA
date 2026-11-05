using DSAExperimentation.LeetCode.Solutions.FinalElementAfterSubarrayDeletions;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FinalElementAfterSubarrayDeletions;

// Harness only: all three strategies live in
// FinalElementAfterSubarrayDeletionsSolution and are asserted against the
// same cases, so a failure names the strategy that broke. The two full
// game-tree strategies run on the same inputs as the O(1) closed form, here
// and over every catalog case in CatalogCaseTests, which is what checks the
// shortcut against them. The case here is a single-element array, already its
// own final element. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class FinalElementAfterSubarrayDeletionsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [9], 9 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FinalElementByDictionaryMinimax_Cases_ReturnsOptimalPlayResult(int[] nums, int expected) =>
        Assert.Equal(expected, FinalElementAfterSubarrayDeletionsSolution.FinalElementByDictionaryMinimax(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FinalElementByMemoizedMinimax_Cases_ReturnsOptimalPlayResult(int[] nums, int expected) =>
        Assert.Equal(expected, FinalElementAfterSubarrayDeletionsSolution.FinalElementByMemoizedMinimax(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FinalElementByEndpointComparison_Cases_ReturnsOptimalPlayResult(int[] nums, int expected) =>
        Assert.Equal(expected, FinalElementAfterSubarrayDeletionsSolution.FinalElementByEndpointComparison(nums));
}
