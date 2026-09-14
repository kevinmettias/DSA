using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.DeleteTheMiddleNodeOfALinkedList;

// LeetCode 2095. Delete the Middle Node of a Linked List: remove the node at
// index n/2 (0-indexed) and return the head of what is left - null when the list
// held a single node, because deleting its only node empties it.
//
// Both strategies answer the same question; they differ only in how many passes
// it takes to reach that index, and whether the answer is spliced out of the
// list handed in or copied into a fresh one.
internal static class DeleteTheMiddleNodeOfALinkedListSolution
{
    // LeetCode's middle for an even-length list is the second of the two centre
    // nodes, which is exactly integer division by two.
    private const int MiddleDivisor = 2;

    // The textbook answer: count the list, then walk it again copying every node
    // except the one at the middle index. Two full traversals and n-1 fresh node
    // allocations, deliberately written without this repo's primitives - it is
    // the arm the single-pass splice below has to justify itself against.
    public static SinglyLinkedListNode<int>? DeleteMiddleByCountThenRebuild(SinglyLinkedListNode<int>? head)
    {
        var length = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            length++;
        }

        var middleIndex = length / MiddleDivisor;

        return RebuildWithoutIndex(head, middleIndex);
    }

    // A dummy head keeps the "is this the first kept node" case out of the loop;
    // a one-node list copies nothing and so comes back empty, which is what LC
    // asks for.
    private static SinglyLinkedListNode<int>? RebuildWithoutIndex(SinglyLinkedListNode<int>? head, int skippedIndex)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        var index = 0;

        for (var node = head; node is not null; node = node.Next, index++)
        {
            if (index != skippedIndex)
            {
                tail.Next = new SinglyLinkedListNode<int>(node.Value);
                tail = tail.Next;
            }
        }

        return dummy.Next;
    }

    // The same slow/fast walk over SinglyLinkedListNode<T>.Next that
    // CycleDetection.cs already uses, aimed at the middle instead of a cycle's
    // meeting point: slow advances one step per two of fast's, so slow lands on
    // the middle exactly when fast runs off the end, and the trailing prev
    // pointer makes the unlink an O(1) splice once it gets there. One traversal,
    // no new nodes at all.
    public static SinglyLinkedListNode<int>? DeleteMiddleBySlowFastPointers(SinglyLinkedListNode<int>? head)
    {
        if (head?.Next is null)
        {
            return null;
        }

        var prev = head;
        var slow = head;
        var fast = head;

        while (fast is not null && fast.Next is not null)
        {
            prev = slow;
            slow = slow!.Next;
            fast = fast.Next.Next;
        }

        prev!.Next = slow!.Next;
        return head;
    }
}
