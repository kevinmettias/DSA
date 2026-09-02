using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.ReverseLinkedList;

// LeetCode 206. Reverse Linked List: given a singly linked list's head, reverse
// it in place and return the new head.
//
// Only one strategy exists in this repo's coverage today - the standard
// three-pointer walk that rewires each node's Next to point at its
// predecessor as it advances. There is no second arm to compare it against
// yet; the benchmark this problem previously carried was a compile-smoke
// placeholder that measured nothing (see the manifest note this migration
// replaces), not a second algorithm.
internal static class ReverseLinkedListSolution
{
    public static SinglyLinkedListNode<int>? ReverseListByIterativeRewire(SinglyLinkedListNode<int>? head)
    {
        SinglyLinkedListNode<int>? previous = null;
        var current = head;

        while (current is not null)
        {
            var next = current.Next;
            current.Next = previous;
            previous = current;
            current = next;
        }

        return previous;
    }
}
