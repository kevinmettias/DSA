using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.RemoveNthNodeFromEndOfList;

// LeetCode 19. Remove Nth Node From End of List: given a singly linked list's head,
// remove the node that sits `positionsFromEnd` places from the end (1-indexed) and
// return the new head.
//
// Both strategies land on the same target - index `Length - positionsFromEnd`,
// 0-indexed from the head - but reach it differently. The baseline materializes the
// list into an array and rebuilds one around the gap; the two-runner walk finds the
// same spot in a single pass with no extra storage, by running a fast pointer
// `positionsFromEnd` nodes ahead of a slow one so the slow pointer stops just before
// the target the instant the fast pointer runs off the end.
internal static class RemoveNthNodeFromEndOfListSolution
{
    // Textbook baseline: copy the list's values into an array, drop the target
    // index with two Array.Copy calls, and rebuild a fresh list from what
    // remains. O(Length) extra space, where the two-runner walk below needs none.
    public static SinglyLinkedListNode<int>? RemoveByArrayRebuild(SinglyLinkedListNode<int>? head, int positionsFromEnd)
    {
        var values = ToArray(head);
        var index = values.Length - positionsFromEnd;
        var remaining = new int[values.Length - 1];
        Array.Copy(values, 0, remaining, 0, index);
        Array.Copy(values, index + 1, remaining, index, values.Length - index - 1);

        return BuildList(remaining);
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

    // The standard two-runner walk: advance fast `positionsFromEnd` nodes ahead of slow,
    // then step both together until fast falls off the end - slow now sits just before
    // the node to remove. Dummy-headed so removing the true head needs no special case.
    public static SinglyLinkedListNode<int>? RemoveByTwoRunner(SinglyLinkedListNode<int>? head, int positionsFromEnd)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var fast = dummy;
        var slow = dummy;

        for (var i = 0; i < positionsFromEnd; i++)
        {
            fast = fast.Next!;
        }

        while (fast.Next is not null)
        {
            fast = fast.Next;
            slow = slow.Next!;
        }

        slow.Next = slow.Next?.Next;
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
