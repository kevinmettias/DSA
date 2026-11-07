using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Solutions.IntersectionOfTwoLinkedLists;

// LeetCode 160. Intersection of Two Linked Lists: find the node at which two
// singly linked lists converge (by reference identity), or null if they never
// do.
[LeetCodeQuestion("intersection-of-two-linked-lists", LeetCodeAnswerOrder.AsWritten, RewritesInput = true)]
internal static class IntersectionOfTwoLinkedListsSolution
{
    // LeetCode's shape (ARCHITECTURE 17.4): its judge reads intersectVal, listA, listB, skipA and skipB, which
    // the function itself is never handed, joins listB into listA's tail (IntersectionJudge), and prints the
    // node returned, read here as its value, 0 for none. The two skips, LeetCode's last two inputs, are read
    // together into one pair. Joining relinks listB, so a harness hands each call lists of their own.
    public static int GetIntersectionNodeByTwoPointerWalk(
        int intersectVal, SinglyLinkedListNode<int>? listA, SinglyLinkedListNode<int>? listB, (int SkipA, int SkipB) skips)
    {
        var headB = IntersectionJudge.HeadBJoinedToA(intersectVal, listA, listB, skips);
        var intersection = GetIntersectionNodeByTwoPointerWalk(listA, headB);

        return IntersectionJudge.Printed(intersection);
    }

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
