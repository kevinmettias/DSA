using DSAExperimentation.LeetCode.MinimumPairRemovalToSortArrayII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumPairRemovalToSortArrayII;

// Harness only. Both strategies are MinimumPairRemovalToSortArrayIISolution's -
// this file just pins them to LeetCode's published examples.
public sealed partial class MinimumPairRemovalToSortArrayIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [5, 2, 3, 1], 2 },
            { [1, 2, 2], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByBruteForceScan_LeetCodeExamples_ReturnsMinimumMergeCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, MinimumPairRemovalToSortArrayIISolution.MinOperationsByBruteForceScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByLazyHeap_LeetCodeExamples_ReturnsMinimumMergeCount(
        int[] nums, int expected) =>
        Assert.Equal(expected, MinimumPairRemovalToSortArrayIISolution.MinOperationsByLazyHeap(nums));
}
