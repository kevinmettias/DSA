using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.RemoveZeroSumConsecutiveNodesFromLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveZeroSumConsecutiveNodesFromLinkedList;

// Harness only. Both strategies are
// RemoveZeroSumConsecutiveNodesFromLinkedListSolution's - this file builds
// LeetCode's published examples as linked lists and checks the surviving values,
// including the cases the nested rescan used to get wrong: a run that starts at a
// node rather than at the head, and a list that cancels away entirely.
public sealed partial class RemoveZeroSumConsecutiveNodesFromLinkedListTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [1, 2, -3, 3, 1], [3, 1] }, // LC's example 1
            { [1, 2, 3, -3, 4], [1, 2, 4] }, // LC's example 2
            { [1, 2, 3, -3, -2], [1] }, // LC's example 3
            { [1, 2, 3], [1, 2, 3] }, // nothing cancels
            { [1, -1], [] }, // the whole list cancels
            { [3, -3, 1, -1], [] }, // two adjacent runs, both cancel
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveZeroSumSublistsByNestedRescan_LeetCodeExamples_RemovesEveryZeroSumRun(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(RemoveZeroSumConsecutiveNodesFromLinkedListSolution
                .RemoveZeroSumSublistsByNestedRescan(BuildList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveZeroSumSublistsByPrefixSumMap_LeetCodeExamples_RemovesEveryZeroSumRun(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(RemoveZeroSumConsecutiveNodesFromLinkedListSolution
                .RemoveZeroSumSublistsByPrefixSumMap(BuildList(values))));

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
