using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.IntersectionOfTwoLinkedLists;

// LeetCode 160. Intersection of Two Linked Lists: find the node at which two
// singly linked lists converge (by reference identity), or null if they never
// do.
internal static class IntersectionOfTwoLinkedListsSolution
{
    // Two pointers walk their own list, then swap onto the other list's head once
    // they run off their own list's end. Both pointers therefore travel
    // lengthA + lengthB steps in total, so they land on the intersection node - or
    // both hit null together - without ever measuring either list's length first.
    public static SinglyLinkedListNode<int>? GetIntersectionNodeByTwoPointerWalk(
        SinglyLinkedListNode<int>? headA, SinglyLinkedListNode<int>? headB)
    {
        var pointerA = headA;
        var pointerB = headB;

        while (!ReferenceEquals(pointerA, pointerB))
        {
            pointerA = pointerA is null ? headB : pointerA.Next;
            pointerB = pointerB is null ? headA : pointerB.Next;
        }

        return pointerA;
    }
}
