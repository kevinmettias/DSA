using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.RotateList;

// LeetCode 61. Rotate List: given a singly linked list's head, rotate the list to
// the right by k places and return the new head.
//
// Both strategies land on the same rotation point - length - (k % length) nodes in
// from the head - but reach it differently. The baseline materializes the list
// into an array, rotates that array by slicing, and rebuilds a fresh list from the
// result; the pointer walk finds the same cut point directly on the existing nodes
// and rewires three pointers, with no extra storage.
internal static class RotateListSolution
{
    // Textbook baseline: copy the list's values into an array, rotate the array by
    // slicing, and rebuild a fresh list from the rotated array. O(Length) extra
    // space, where the pointer walk below needs none.
    public static SinglyLinkedListNode<int>? RotateRightByArrayRebuild(SinglyLinkedListNode<int>? head, int k)
    {
        var values = ToArray(head);

        if (values.Length == 0)
        {
            return null;
        }

        var shift = k % values.Length;

        if (shift == 0)
        {
            return BuildList(values);
        }

        var rotated = new int[values.Length];
        Array.Copy(values, values.Length - shift, rotated, 0, shift);
        Array.Copy(values, 0, rotated, shift, values.Length - shift);

        return BuildList(rotated);
    }

    // The standard walk: find the length and current tail in one pass, then cut
    // the list at the new tail and reattach the old tail to the old head - three
    // pointer rewrites, no extra storage.
    public static SinglyLinkedListNode<int>? RotateRightByPointerRewire(SinglyLinkedListNode<int>? head, int k)
    {
        if (head is null || head.Next is null || k == 0)
        {
            return head;
        }

        var length = 1;
        var tail = head;

        while (tail.Next is not null)
        {
            tail = tail.Next;
            length++;
        }

        var shift = k % length;

        if (shift == 0)
        {
            return head;
        }

        var stepsToNewTail = length - shift - 1;
        var newTail = head;

        for (var i = 0; i < stepsToNewTail; i++)
        {
            newTail = newTail.Next!;
        }

        var newHead = newTail.Next;
        newTail.Next = null;
        tail.Next = head;

        return newHead;
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
