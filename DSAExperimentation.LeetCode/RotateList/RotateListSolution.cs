using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.RotateList;

// LeetCode 61. Rotate List: given a singly linked list's head, rotate the list to
// the right by rotationCount places and return the new head.
//
// Both strategies land on the same rotation point - length - (rotationCount %
// length) nodes in from the head - but reach it differently. The baseline
// materializes the list into an array, rotates that array by slicing, and rebuilds
// a fresh list from the result; the pointer walk finds the same cut point directly
// on the existing nodes and rewires three pointers, with no extra storage.
internal static class RotateListSolution
{
    // Textbook baseline: copy the list's values into an array, rotate the array by
    // slicing, and rebuild a fresh list from the rotated array. O(Length) extra
    // space, where the pointer walk below needs none.
    public static SinglyLinkedListNode<int>? RotateRightByArrayRebuild(
        SinglyLinkedListNode<int>? head, int rotationCount)
    {
        var values = ToArray(head);

        if (values.Length == 0)
        {
            return null;
        }

        var shift = rotationCount % values.Length;

        if (shift == 0)
        {
            return BuildList(values);
        }

        var rotated = new int[values.Length];
        Array.Copy(values, values.Length - shift, rotated, 0, shift);
        Array.Copy(values, 0, rotated, shift, values.Length - shift);

        return BuildList(rotated);
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

    // The standard walk: find the length and current tail in one pass, then cut
    // the list at the new tail and reattach the old tail to the old head - three
    // pointer rewrites, no extra storage.
    public static SinglyLinkedListNode<int>? RotateRightByPointerRewire(
        SinglyLinkedListNode<int>? head, int rotationCount)
    {
        if (IsNoRotationNeeded(head, rotationCount))
        {
            return head;
        }

        var (length, tail) = MeasureList(head);
        var shift = rotationCount % length;

        if (shift == 0)
        {
            return head;
        }

        return RewireAtNewTail(head, tail, length - shift - 1);
    }

    // A list of fewer than two nodes, or a rotation by nothing, is already the
    // answer.
    private static bool IsNoRotationNeeded(SinglyLinkedListNode<int>? head, int rotationCount)
        => head is null || head.Next is null || rotationCount == 0;

    // The list's node count and its current tail, found in a single walk from the
    // head - the head is not null here, so there is always at least one node.
    private static (int Length, SinglyLinkedListNode<int>? Tail) MeasureList(SinglyLinkedListNode<int>? head)
    {
        var length = 1;
        var tail = head;

        while (tail.Next is not null)
        {
            tail = tail.Next;
            length++;
        }

        return (length, tail);
    }

    // The cut point is the node `stepsToNewTail` in from the head; rewiring it
    // takes three pointer writes - detach the new head, terminate the new tail,
    // and splice the old tail back onto the old head.
    private static SinglyLinkedListNode<int>? RewireAtNewTail(
        SinglyLinkedListNode<int>? head, SinglyLinkedListNode<int>? tail, int stepsToNewTail)
    {
        var newTail = AdvanceToNewTail(head, stepsToNewTail);
        return CutAndSplice(newTail, tail, head);
    }

    private static SinglyLinkedListNode<int>? AdvanceToNewTail(SinglyLinkedListNode<int>? head, int steps)
    {
        var newTail = head;

        for (var i = 0; i < steps; i++)
        {
            newTail = newTail.Next!;
        }

        return newTail;
    }

    private static SinglyLinkedListNode<int>? CutAndSplice(
        SinglyLinkedListNode<int>? newTail, SinglyLinkedListNode<int>? tail, SinglyLinkedListNode<int>? head)
    {
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
}
