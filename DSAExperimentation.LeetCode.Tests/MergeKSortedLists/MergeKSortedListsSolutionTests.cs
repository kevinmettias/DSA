using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.MergeKSortedLists;

namespace DSAExperimentation.LeetCode.Tests.MergeKSortedLists;

// Harness only. Both strategies are MergeKSortedListsSolution's - this file pins
// them to LeetCode's published examples, stated once as the raw values of each
// input list so a fresh SinglyLinkedListNode<int> chain is built per assertion:
// MergeListsByHeap rewires the very nodes it is handed, so reusing one already-
// merged instance across strategies (or across the two theories sharing this
// data) would silently feed the second call an already-consumed structure.
public sealed partial class MergeKSortedListsSolutionTests
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
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(MergeKSortedListsSolution.MergeListsByHeap(BuildLists(lists))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeListsByFlattenSort_LeetCodeExamples_ReturnsOneSortedList(int[][] lists, int[] expected) =>
        Assert.Equal(
            expected,
            LeetCodeWireFormat.FromLinkedList(MergeKSortedListsSolution.MergeListsByFlattenSort(BuildLists(lists))));

    private static SinglyLinkedListNode<int>?[] BuildLists(int[][] lists) =>
        lists.Select(LeetCodeWireFormat.ToLinkedList).ToArray();
}
