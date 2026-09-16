using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.MergeKSortedLists;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeKSortedLists;

// Harness only. Both strategies are MergeKSortedListsSolution's - this file pins
// them to LeetCode's published examples, stated once as the raw values of each
// input list so a fresh SinglyLinkedListNode<int> chain is built per assertion:
// MergeListsByHeap rewires the very nodes it is handed, so reusing one already-
// merged instance across strategies (or across the two theories sharing this
// data) would silently feed the second call an already-consumed structure.
public sealed partial class MergeKSortedListsTests
{
    public static TheoryData<int[][], int[]> Examples =>
        new()
        {
            { [[1, 4, 5], [1, 3, 4], [2, 6]], [1, 1, 2, 3, 4, 4, 5, 6] },
            { [], [] },
            { [[]], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeListsByHeap_LeetCodeExamples_ReturnsOneSortedList(int[][] lists, int[] expected) =>
        Assert.Equal(expected, ToArray(MergeKSortedListsSolution.MergeListsByHeap(BuildLists(lists))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeListsByFlattenSort_LeetCodeExamples_ReturnsOneSortedList(int[][] lists, int[] expected) =>
        Assert.Equal(expected, ToArray(MergeKSortedListsSolution.MergeListsByFlattenSort(BuildLists(lists))));

    private static SinglyLinkedListNode<int>?[] BuildLists(int[][] lists) =>
        lists.Select(BuildList).ToArray();

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
