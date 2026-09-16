using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.SwappingNodesInALinkedList;

// LeetCode 1721. Swapping Nodes in a Linked List: swap the value of the kth node
// from the beginning with the value of the kth node from the end, and return the
// head. LeetCode guarantees 1 <= kthPosition <= n, so both positions always exist.
//
// Locating the kth node from the *front* is a straight walk either way; the two
// strategies differ only in how they reach the kth from the *end*. Materializing
// the values into a BCL list turns it into index n - kthPosition at the cost of
// O(n) extra space; a fast/slow pair of node references finds it with no extra space
// at all, because a runner started kthPosition - 1 nodes ahead runs out exactly when
// the trailing reference is standing on it.
internal static class SwappingNodesInALinkedListSolution
{
    // The textbook answer: copy every value out to a BCL list, where the kth node
    // from the end is just index n - kthPosition, swap the two entries there and
    // build a fresh list from the result. Deliberately BCL-only beyond the
    // input/output list shape itself - it is the arm the two-pointer walk below has
    // to justify itself against.
    public static SinglyLinkedListNode<int>? SwapNodesByArrayMaterialize(SinglyLinkedListNode<int>? head, int kthPosition)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        (values[kthPosition - 1], values[^kthPosition]) = (values[^kthPosition], values[kthPosition - 1]);

        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    // One pass, no extra space: walk `front` to the kth node from the beginning,
    // then walk a runner from there to the tail while `end` walks from the head, so
    // `end` lands on the kth node from the end the moment the runner stops. The two
    // nodes' Values are then exchanged in place - the same "rewrite Value, not the
    // pointers" shape SinglyLinkedListNode's own doc comment describes, and the same
    // one SwapNodesInPairs' pointer arm deliberately does not use.
    public static SinglyLinkedListNode<int>? SwapNodesByTwoPointerWalk(SinglyLinkedListNode<int>? head, int kthPosition)
    {
        var front = head;

        for (var i = 1; i < kthPosition; i++)
        {
            front = front!.Next;
        }

        var end = head;
        var runner = front;

        while (runner!.Next is not null)
        {
            runner = runner.Next;
            end = end!.Next;
        }

        (front!.Value, end!.Value) = (end.Value, front.Value);
        return head;
    }
}
