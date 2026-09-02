using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.MergeTwoSortedLists;

// LeetCode 21. Merge Two Sorted Lists: merge two already-sorted singly linked lists
// into one sorted list by splicing their existing nodes together, never allocating a
// new node for a value that already has one.
internal static class MergeTwoSortedListsSolution
{
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
