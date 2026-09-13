using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.RemoveZeroSumConsecutiveNodesFromLinkedList;

// LeetCode 1171. Remove Zero Sum Consecutive Nodes from Linked List: repeatedly
// delete every maximal run of consecutive nodes summing to zero, until none is
// left, and return the resulting head.
//
// Both strategies use the same sentinel trick - a dummy node in front of head, so
// a run starting at the head is spliced out by the same code path as any other -
// and differ only in how they find the runs. The rescan re-sums forward from every
// start node in turn; the prefix-sum map notices that two positions sharing a
// running sum bracket a run that cancels, so one pass to record the LAST node at
// each running sum and one pass to jump each node past everything that cancels
// after it does the whole job in O(n).
internal static class RemoveZeroSumConsecutiveNodesFromLinkedListSolution
{
    // Textbook baseline: for every start node, walk forward re-summing, and every
    // time the running total hits zero splice the whole run out by pointing start
    // past it. O(n^2) and deliberately written without this repo's primitives - it
    // is the arm the prefix-sum map below has to justify itself against.
    //
    // The inner walk begins at start.Next, not at start: the sum being tested is
    // the sum of the nodes strictly after start, which are exactly the nodes
    // start.Next can be made to skip.
    public static SinglyLinkedListNode<int>? RemoveZeroSumSublistsByNestedRescan(
        SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };

        for (var start = dummy; start is not null; start = start.Next)
        {
            var sum = 0;

            for (var end = start.Next; end is not null; end = end.Next)
            {
                sum += end.Value;

                if (sum == 0)
                {
                    start.Next = end.Next;
                }
            }
        }

        return dummy.Next;
    }

    // This repo's own HashMap<int, SinglyLinkedListNode<int>> as a prefix-sum
    // index. Nodes sharing a running sum bracket a run that cancels, so recording
    // the LAST node seen at each sum and then pointing every node at that node's
    // Next removes every zero-sum run - including overlapping and nested ones - in
    // one second pass, O(n) in total.
    public static SinglyLinkedListNode<int>? RemoveZeroSumSublistsByPrefixSumMap(
        SinglyLinkedListNode<int>? head)
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
}
