using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveZeroSumConsecutiveNodesFromLinkedList;

// LeetCode 1171. Remove Zero Sum Consecutive Nodes from Linked List: the standard
// prefix-sum trick over this repo's own SinglyLinkedListNode<int>, using
// HashMap<int, SinglyLinkedListNode<int>> to remember the LAST node seen at each
// running sum. A zero-sum run between two nodes sharing the same prefix sum means
// everything strictly between them cancels out, so re-walking the list and pointing
// each node at map[sum].Next splices every such run out in one second pass - O(n)
// total instead of the O(n^2) nested-loop brute force.
public sealed partial class RemoveZeroSumConsecutiveNodesFromLinkedListTests
{
    [Fact]
    public void RemoveZeroSumSublists_SingleZeroSumRunInMiddle_RemovesJustThatRun()
        => Assert.Equal([3, 1], ToArray(RemoveZeroSumSublists(BuildList([1, 2, -3, 3, 1]))));

    [Fact]
    public void RemoveZeroSumSublists_OverlappingZeroSumRuns_RemovesBoth()
        => Assert.Equal([1, 2, 4], ToArray(RemoveZeroSumSublists(BuildList([1, 2, 3, -3, 4]))));

    [Fact]
    public void RemoveZeroSumSublists_EntireTailCancelsOut_LeavesOnlyThePrefix()
        => Assert.Equal([1], ToArray(RemoveZeroSumSublists(BuildList([1, 2, 3, -3, -2]))));

    private static SinglyLinkedListNode<int>? RemoveZeroSumSublists(SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var lastNodeAtSum = new HashMap<int, SinglyLinkedListNode<int>>();

        var sum = 0;
        for (var node = dummy; node is not null; node = node.Next)
        {
            sum += node.Value;
            lastNodeAtSum.Set(sum, node);
        }

        sum = 0;
        for (var node = dummy; node is not null; node = node.Next)
        {
            sum += node.Value;
            lastNodeAtSum.TryGetValue(sum, out var lastNodeWithSameSum);
            node.Next = lastNodeWithSameSum!.Next;
        }

        return dummy.Next;
    }

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
