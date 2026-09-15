using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.ReorderList;

// LeetCode 143. Reorder List: rewrite L0->L1->...->Ln-1->Ln in place into
// L0->Ln->L1->Ln-1->L2->Ln-2->... without allocating a second list.
//
// Split the list at its midpoint with slow/fast pointers, reverse the second
// half, then splice the two halves together one node at a time. This is the
// only strategy the original test and benchmark between them actually
// implemented - the benchmark carried no second algorithm, just two stub
// [Benchmark] methods that both returned a literal 1 - so there is nothing to
// reconcile here, only to extract and put under test for the first time.
internal static class ReorderListSolution
{
    public static void ReorderByReverseAndMergeInPlace(SinglyLinkedListNode<int>? head)
    {
        if (head?.Next is null)
        {
            return;
        }

        var midpoint = FindMidpoint(head);
        var second = Reverse(midpoint.Next);
        midpoint.Next = null;

        Merge(head, second);
    }

    // The node the slow cursor stops on: the last node of the first half, found by
    // running a fast cursor at twice its speed.
    private static SinglyLinkedListNode<int> FindMidpoint(SinglyLinkedListNode<int> head)
    {
        var slow = head;
        var fast = head;

        while (fast.Next?.Next is not null)
        {
            slow = slow.Next!;
            fast = fast.Next.Next;
        }

        return slow;
    }

    private static SinglyLinkedListNode<int>? Reverse(SinglyLinkedListNode<int>? head)
    {
        SinglyLinkedListNode<int>? previous = null;

        for (var current = head; current is not null;)
        {
            var next = current.Next;
            current.Next = previous;
            previous = current;
            current = next;
        }

        return previous;
    }

    private static void Merge(SinglyLinkedListNode<int> first, SinglyLinkedListNode<int>? second)
    {
        while (second is not null)
        {
            var nextFirst = first.Next;
            var nextSecond = second.Next;

            first.Next = second;
            second.Next = nextFirst;

            first = nextFirst!;
            second = nextSecond;
        }
    }
}
