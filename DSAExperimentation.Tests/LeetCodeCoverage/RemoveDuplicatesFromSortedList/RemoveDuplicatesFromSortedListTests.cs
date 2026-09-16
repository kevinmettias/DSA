using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicatesFromSortedList;

// Harness only. Both strategies are RemoveDuplicatesFromSortedListSolution's -
// this file builds LeetCode's published examples as linked lists and checks the
// resulting list's values.
public sealed partial class RemoveDuplicatesFromSortedListTests
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
            ToArray(RemoveDuplicatesFromSortedListSolution.DeleteDuplicatesByDistinctFilter(BuildList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteDuplicatesByInPlaceScan_LeetCodeExamples_CollapsesDuplicateRuns(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(RemoveDuplicatesFromSortedListSolution.DeleteDuplicatesByInPlaceScan(BuildList(values))));

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();
        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values.ToArray();
    }
}
