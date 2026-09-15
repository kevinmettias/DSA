using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.OddEvenLinkedList;

// LeetCode 328. Odd Even Linked List: regroup a singly linked list so every
// odd-indexed node (1-indexed) precedes every even-indexed node, each half keeping
// its original relative order.
//
// The two strategies differ only in how they get there: materialize each half's
// values into a fresh chain, or rewire the existing nodes' Next pointers in place.
internal static class OddEvenLinkedListSolution
{
    private const int ParityDivisor = 2;

    // The textbook baseline: walk the list once, bucketing values into two BCL
    // List<int> buffers by index parity, then build a brand-new chain from their
    // concatenation. O(n) extra space. Deliberately written without this repo's
    // rewiring trick - it is the arm the composed solution below has to justify
    // itself against.
    public static SinglyLinkedListNode<int>? GroupOddEvenByTwoListRebuild(SinglyLinkedListNode<int>? head)
    {
        if (head is null)
        {
            return null;
        }

        var odds = new List<int>();
        var evens = new List<int>();
        var index = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            var isOddIndex = index % ParityDivisor == 0;
            (isOddIndex ? odds : evens).Add(node.Value);
            index++;
        }

        odds.AddRange(evens);
        return Rebuild(odds);
    }

    private static SinglyLinkedListNode<int>? Rebuild(List<int> values)
    {
        if (values.Count == 0)
        {
            return null;
        }

        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    // Two running cursors splice the list into its odd-indexed nodes followed by
    // its even-indexed nodes in place, preserving each half's relative order, in
    // O(n) time and O(1) extra space.
    public static SinglyLinkedListNode<int>? GroupOddEvenByInPlaceRewire(SinglyLinkedListNode<int>? head)
    {
        if (head?.Next is null)
        {
            return head;
        }

        var odd = head;
        var even = head.Next;
        var evenHead = even;

        while (even?.Next is not null)
        {
            odd.Next = even.Next;
            odd = odd.Next;
            even.Next = odd.Next;
            even = even.Next;
        }

        odd.Next = evenHead;
        return head;
    }
}
