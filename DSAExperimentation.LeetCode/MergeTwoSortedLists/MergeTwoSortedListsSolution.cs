using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.MergeTwoSortedLists;

// LeetCode 21. Merge Two Sorted Lists: merge two already-sorted singly linked lists
// into one sorted list by splicing their existing nodes together, never allocating a
// new node for a value that already has one.
internal static class MergeTwoSortedListsSolution
{
    // The textbook arm the iterative splice is measured against: merge the first two nodes and
    // recurse on the rest. It reuses the same nodes and reaches the same list, but spends a stack
    // frame per merged node where the loop below spends none.
    public static SinglyLinkedListNode<int>? MergeByRecursiveSelection(
        SinglyLinkedListNode<int>? first, SinglyLinkedListNode<int>? second)
    {
        if (first is null)
        {
            return second;
        }

        if (second is null)
        {
            return first;
        }

        if (first.Value <= second.Value)
        {
            first.Next = MergeByRecursiveSelection(first.Next, second);
            return first;
        }

        second.Next = MergeByRecursiveSelection(first, second.Next);
        return second;
    }

    // Walk both lists once behind a dummy head, relinking whichever current node is
    // smaller onto the tail being built; whichever list still has nodes left over
    // is already sorted, so it can be spliced on whole once the other runs out.
    public static SinglyLinkedListNode<int>? MergeByDummyHeadSplice(
        SinglyLinkedListNode<int>? first, SinglyLinkedListNode<int>? second)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        while (first is not null && second is not null)
        {
            if (first.Value <= second.Value)
            {
                tail.Next = first;
                first = first.Next;
            }
            else
            {
                tail.Next = second;
                second = second.Next;
            }

            tail = tail.Next;
        }

        tail.Next = first ?? second;
        return dummy.Next;
    }
}
