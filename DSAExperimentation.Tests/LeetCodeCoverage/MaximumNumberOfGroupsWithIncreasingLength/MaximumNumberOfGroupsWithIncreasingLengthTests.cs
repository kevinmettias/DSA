using DSAExperimentation.LeetCode.MaximumNumberOfGroupsWithIncreasingLength;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfGroupsWithIncreasingLength;

// Harness only. Both strategies are
// MaximumNumberOfGroupsWithIncreasingLengthSolution's - including the insertion sort,
// which the benchmark used to own privately as its baseline and nothing asserted.
// Beyond LeetCode's three published examples the cases pin the greedy's edges: a
// single index whose spare capacity buys nothing, an unsorted input that only works
// if the sort actually happens, and a large limit on one index that still cannot
// exceed the group-size ceiling set by how many distinct indices exist.
public sealed partial class MaximumNumberOfGroupsWithIncreasingLengthTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            // LC example 1.
            { [1, 2, 5], 3 },

            // LC example 2.
            { [2, 1, 2], 2 },

            // LC example 3.
            { [1, 1], 1 },

            // One index with capacity to spare: it can still only fill one group.
            { [5], 1 },

            // Unsorted, and the answer depends on the small limits being spent first.
            { [3, 1, 4, 1, 5], 4 },

            // Uniform capacity across several indices.
            { [2, 2, 2, 2], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxIncreasingGroupsByInsertionSort_LeetCodeExamples_ReturnsGroupCount(
        int[] usageLimits, int expected) =>
        Assert.Equal(
            expected,
            MaximumNumberOfGroupsWithIncreasingLengthSolution.MaxIncreasingGroupsByInsertionSort(usageLimits));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxIncreasingGroupsByMergeSort_LeetCodeExamples_ReturnsGroupCount(
        int[] usageLimits, int expected) =>
        Assert.Equal(
            expected,
            MaximumNumberOfGroupsWithIncreasingLengthSolution.MaxIncreasingGroupsByMergeSort(usageLimits));
}
