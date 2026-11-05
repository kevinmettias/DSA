using DSAExperimentation.LeetCode.Solutions.MaximumNumberOfGroupsWithIncreasingLength;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNumberOfGroupsWithIncreasingLength;

// Harness only. Both strategies are
// MaximumNumberOfGroupsWithIncreasingLengthSolution's - including the insertion sort,
// which the benchmark used to own privately as its baseline and nothing asserted.
// The cases pin the greedy's edges: a single index whose spare capacity buys
// nothing, an unsorted input that only works if the sort actually happens, and the
// same capacity on every index. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MaximumNumberOfGroupsWithIncreasingLengthSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            // One index with capacity to spare: it can still only fill one group.
            { [5], 1 },

            // Unsorted, and the answer depends on the small limits being spent first.
            { [3, 1, 4, 1, 5], 4 },

            // Uniform capacity across several indices.
            { [2, 2, 2, 2], 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxIncreasingGroupsByInsertionSort_Cases_ReturnsGroupCount(
        int[] usageLimits, int expected) =>
        Assert.Equal(
            expected,
            MaximumNumberOfGroupsWithIncreasingLengthSolution.MaxIncreasingGroupsByInsertionSort(usageLimits));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxIncreasingGroupsByMergeSort_Cases_ReturnsGroupCount(
        int[] usageLimits, int expected) =>
        Assert.Equal(
            expected,
            MaximumNumberOfGroupsWithIncreasingLengthSolution.MaxIncreasingGroupsByMergeSort(usageLimits));
}
