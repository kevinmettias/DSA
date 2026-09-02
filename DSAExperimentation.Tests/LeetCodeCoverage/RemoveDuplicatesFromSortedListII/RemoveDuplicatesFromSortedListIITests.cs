using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedListII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicatesFromSortedListII;

// Harness only. Both strategies are RemoveDuplicatesFromSortedListIISolution's -
// this file builds LeetCode's published examples as linked lists and checks the
// resulting list's values.
public sealed class RemoveDuplicatesFromSortedListIITests
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
            ToArray(RemoveDuplicatesFromSortedListIISolution.DeleteDuplicatesByArrayGroupFilter(BuildList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteDuplicatesByTwoPointerScan_LeetCodeExamples_RemovesAllDuplicateRuns(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(RemoveDuplicatesFromSortedListIISolution.DeleteDuplicatesByTwoPointerScan(BuildList(values))));

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
