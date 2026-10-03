using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedListII;

namespace DSAExperimentation.LeetCode.Tests.RemoveDuplicatesFromSortedListII;

// Harness only. Both strategies are RemoveDuplicatesFromSortedListIISolution's -
// this file builds LeetCode's published examples as linked lists and checks the
// resulting list's values.
public sealed partial class RemoveDuplicatesFromSortedListIISolutionTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, 3, 3, 4, 4, 5], [1, 2, 5] },
            { [1, 1, 1, 2, 3], [2, 3] },
            { [1, 1], [] },
            { [1, 2, 3], [1, 2, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteDuplicatesByArrayGroupFilter_LeetCodeExamples_RemovesAllDuplicateRuns(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                RemoveDuplicatesFromSortedListIISolution.DeleteDuplicatesByArrayGroupFilter(
                    LeetCodeWireFormat.ToLinkedList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteDuplicatesByTwoPointerScan_LeetCodeExamples_RemovesAllDuplicateRuns(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(
                RemoveDuplicatesFromSortedListIISolution.DeleteDuplicatesByTwoPointerScan(
                    LeetCodeWireFormat.ToLinkedList(values))));
}
