using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.SwapNodesInPairs;

// LeetCode 24. Swap Nodes in Pairs: swap every adjacent pair of nodes in a singly
// linked list and return the new head.
//
// The two strategies differ in how they get the swapped order: rewire each pair's
// Next pointers in place behind a dummy head, or copy every value out to a BCL
// array, swap adjacent entries there, and build a fresh list from the result.
internal static class SwapNodesInPairsSolution
{
    private const int PairStride = 2;

    // The composed solution: relink each adjacent pair's Next pointers behind a
    // dummy head, reusing the existing nodes rather than allocating new ones.
    public static SinglyLinkedListNode<int>? SwapPairsByPointerRewiring(SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var previous = dummy;

        while (previous.Next?.Next is not null)
        {
            var first = previous.Next;
            var second = first.Next!;
            first.Next = second.Next;
            second.Next = first;
            previous.Next = second;
            previous = first;
        }

        return dummy.Next;
    }

    // The textbook approach many first reach for: copy every value into a BCL
    // array, swap adjacent entries there, and build a brand-new list from the
    // result. Deliberately written without this repo's pointer-rewiring beyond
    // the input/output list shape itself - it is the arm PointerRewiring has to
    // justify itself against.
    public static SinglyLinkedListNode<int>? SwapPairsByArrayRoundTrip(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        var array = values.ToArray();

        for (var i = 0; i + 1 < array.Length; i += PairStride)
        {
            (array[i], array[i + 1]) = (array[i + 1], array[i]);
        }

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
