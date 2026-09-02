using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.ReverseLinkedListII;

// LeetCode 92. Reverse Linked List II: given a singly linked list's head and two
// 1-indexed positions left <= right, reverse the nodes from position left to
// position right and return the (possibly new) head.
//
// Both strategies land on the same resulting sequence of values but reach it
// differently: the baseline materializes the list into an array, reverses the
// [left, right] sub-range with Array.Reverse, and rebuilds a fresh list from the
// result; the head-insertion walk finds the same sub-range on the existing nodes
// and rewires them in place, with no extra storage. Mirrors RotateListSolution's
// array-rebuild-vs-pointer-walk shape exactly.
internal static class ReverseLinkedListIISolution
{
    // Textbook baseline: copy the list's values into an array, reverse the
    // [left, right] sub-range with Array.Reverse, and rebuild a fresh list from
    // the result. O(Length) extra space, where the head-insertion walk below
    // needs none.
    public static SinglyLinkedListNode<int>? ReverseBetweenByArrayRebuild(
        SinglyLinkedListNode<int>? head, int left, int right)
    {
        var values = ToArray(head);
        Array.Reverse(values, left - 1, right - left + 1);

        return BuildList(values);
    }

    // The standard walk: a dummy node ahead of the list lets position 1 be
    // handled the same as any other position. Once positioned at the node just
    // before the sub-range, each subsequent node is unlinked and reinserted
    // immediately after that fixed point - "head insertion" - so the sub-range
    // ends up reversed after right - left splices, with no extra storage.
    public static SinglyLinkedListNode<int>? ReverseBetweenByHeadInsertion(
        SinglyLinkedListNode<int>? head, int left, int right)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var before = dummy;

        for (var i = 1; i < left; i++)
        {
            before = before.Next!;
        }

        var current = before.Next;

        for (var i = 0; i < right - left; i++)
        {
            var moved = current!.Next!;
            current.Next = moved.Next;
            moved.Next = before.Next;
            before.Next = moved;
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
}
