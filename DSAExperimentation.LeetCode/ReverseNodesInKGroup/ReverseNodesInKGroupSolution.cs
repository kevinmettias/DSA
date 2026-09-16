using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.ReverseNodesInKGroup;

// LeetCode 25. Reverse Nodes in k-Group: reverse every run of groupSize nodes in
// order, leaving a final short group (fewer than groupSize nodes remaining) untouched.
//
// The two strategies differ in where the reversal happens - in place on the
// linked list's own pointers, or by copying every value out to a plain array,
// reversing each complete group there, and writing the values back.
internal static class ReverseNodesInKGroupSolution
{
    // The composed solution: a dummy head lets the very first group be spliced
    // back the same way as every other one. TryGetKth finds the boundary of the
    // next complete group; ReverseOneGroup rewires just that group's pointers and
    // returns the node the previous group should now point at.
    public static SinglyLinkedListNode<int>? ReverseKGroupByPointerReversal(
        SinglyLinkedListNode<int>? head, int groupSize)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var groupPrevious = dummy;

        while (TryGetKth(groupPrevious, groupSize, out var kth))
        {
            groupPrevious = ReverseOneGroup(groupPrevious, kth);
        }

        return dummy.Next;
    }

    private static bool TryGetKth(
        SinglyLinkedListNode<int> groupPrevious, int groupSize, out SinglyLinkedListNode<int> kth)
    {
        SinglyLinkedListNode<int>? node = groupPrevious;

        for (var i = 0; i < groupSize && node is not null; i++)
        {
            node = node.Next;
        }

        kth = node!;
        return node is not null;
    }

    private static SinglyLinkedListNode<int> ReverseOneGroup(
        SinglyLinkedListNode<int> groupPrevious, SinglyLinkedListNode<int> kth)
    {
        var groupNext = kth.Next;
        SinglyLinkedListNode<int>? previous = groupNext;
        var current = groupPrevious.Next;

        while (current != groupNext)
        {
            var next = current!.Next;
            current.Next = previous;
            previous = current;
            current = next;
        }

        var oldGroupHead = groupPrevious.Next!;
        groupPrevious.Next = kth;

        return oldGroupHead;
    }

    // The textbook alternative: read every value off the list into a plain array,
    // reverse each complete run of groupSize with the BCL's own Array.Reverse, then
    // rebuild the list from the reordered values. Deliberately written without
    // this repo's list-walking helpers beyond the input/output list shape itself -
    // it is the arm the pointer-reversal strategy above has to justify itself
    // against.
    public static SinglyLinkedListNode<int>? ReverseKGroupByArrayReverse(
        SinglyLinkedListNode<int>? head, int groupSize)
    {
        var array = ReadValues(head);
        var completeGroups = array.Length / groupSize * groupSize;

        for (var i = 0; i < completeGroups; i += groupSize)
        {
            Array.Reverse(array, i, groupSize);
        }

        return RebuildFromArray(array);
    }

    // Every value on the list, in order, as a plain array.
    private static int[] ReadValues(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values.ToArray();
    }

    // A fresh list holding the array's values in order.
    private static SinglyLinkedListNode<int>? RebuildFromArray(int[] array)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in array)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }
}
