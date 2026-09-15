using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.ReverseNodesInEvenLengthGroups;

// LeetCode 2074. Reverse Nodes in Even Length Groups: the list is cut into groups
// of 1, 2, 3, ... nodes, the final group taking however many are left, and every
// group whose ACTUAL length is even is reversed in place.
//
// The same shape as ReverseNodesInKGroup with two twists that both strategies have
// to honour: the group size grows by one after every group instead of staying
// fixed, and it is the truncated final group's real length - not the size it was
// reaching for - that decides whether it reverses.
internal static class ReverseNodesInEvenLengthGroupsSolution
{
    // The first group is a single node. Length 1 is odd, so the head never moves,
    // which is what lets both walks start from it and measure the second group -
    // two nodes - first.
    private const int FirstGroupLength = 1;
    private const int SecondGroupSize = 2;
    private const int EvenLengthDivisor = 2;

    // The composed solution: pointer splicing over SinglyLinkedListNode<T> with no
    // array materialization at all. groupPrevious walks forward one group at a time
    // exactly as ReverseNodesInKGroup's does; MeasureGroup reports both where the
    // group ends and how long it actually turned out to be.
    public static SinglyLinkedListNode<int>? ReverseEvenLengthGroupsByPointerReversal(
        SinglyLinkedListNode<int>? head)
    {
        var groupPrevious = head;
        var groupSize = SecondGroupSize;

        while (groupPrevious?.Next is not null)
        {
            var (tail, length) = MeasureGroup(groupPrevious, groupSize);

            var isGroupLengthEven = length % EvenLengthDivisor == 0;
            groupPrevious = isGroupLengthEven ? ReverseGroup(groupPrevious, tail) : tail;
            groupSize++;
        }

        return head;
    }

    // Walks up to groupSize nodes past groupPrevious, stopping early at the list's
    // end - this truncated actual length, not the expected groupSize, is what
    // decides whether the group reverses.
    private static (SinglyLinkedListNode<int> Tail, int Length) MeasureGroup(
        SinglyLinkedListNode<int> groupPrevious, int groupSize)
    {
        var node = groupPrevious;
        var length = 0;

        while (length < groupSize && node.Next is not null)
        {
            node = node.Next;
            length++;
        }

        return (node, length);
    }

    // Rewires one group's pointers between groupPrevious and tail, returning the
    // node the next group should treat as its predecessor - the group's old head,
    // which reversal has just moved to its end.
    private static SinglyLinkedListNode<int> ReverseGroup(
        SinglyLinkedListNode<int> groupPrevious, SinglyLinkedListNode<int> tail)
    {
        var groupNext = tail.Next;
        var oldGroupHead = groupPrevious.Next!;
        SinglyLinkedListNode<int>? previous = groupNext;
        var current = oldGroupHead;

        while (current != groupNext)
        {
            var next = current!.Next;
            current.Next = previous;
            previous = current;
            current = next;
        }

        groupPrevious.Next = tail;
        return oldGroupHead;
    }

    // The textbook alternative: copy every value out into a plain array, compute
    // the same increasing-then-truncated boundaries there, reverse each even-length
    // run with the BCL's own Array.Reverse, and rebuild the list from the reordered
    // values. Deliberately written without this repo's primitives beyond the
    // input/output list shape itself.
    public static SinglyLinkedListNode<int>? ReverseEvenLengthGroupsByArrayReverse(
        SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        var array = values.ToArray();
        ReverseEvenLengthRuns(array);

        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in array)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    private static void ReverseEvenLengthRuns(int[] array)
    {
        var start = FirstGroupLength;
        var groupSize = SecondGroupSize;

        while (start < array.Length)
        {
            var actualLength = Math.Min(groupSize, array.Length - start);

            if (actualLength % EvenLengthDivisor == 0)
            {
                Array.Reverse(array, start, actualLength);
            }

            start += actualLength;
            groupSize++;
        }
    }
}
