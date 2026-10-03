using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedList;

namespace DSAExperimentation.LeetCode.Tests.RemoveDuplicatesFromSortedList;

// Harness only. Both strategies are RemoveDuplicatesFromSortedListSolution's -
// this file builds LeetCode's published examples as linked lists and checks the
// resulting list's values.
public sealed partial class RemoveDuplicatesFromSortedListSolutionTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 1, 2], [1, 2] },
            { [1, 1, 2, 3, 3], [1, 2, 3] },
            { [], [] },
            { [1, 1, 1], [1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteDuplicatesByDistinctFilter_LeetCodeExamples_CollapsesDuplicateRuns(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                RemoveDuplicatesFromSortedListSolution.DeleteDuplicatesByDistinctFilter(
                    LeetCodeWireFormat.ToLinkedList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteDuplicatesByInPlaceScan_LeetCodeExamples_CollapsesDuplicateRuns(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                RemoveDuplicatesFromSortedListSolution.DeleteDuplicatesByInPlaceScan(
                    LeetCodeWireFormat.ToLinkedList(values))));
}
