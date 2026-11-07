using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Solutions.IntersectionOfTwoLinkedLists;

// LC 160's judge, which the function never sees: it reads intersectVal, listA, listB, skipA and skipB, and
// builds the two lists so that listA's node at index skipA is listB's node at index skipB, one tail shared
// from there on - or no shared node at all when intersectVal is 0. It prints the node the function returns,
// "Intersected at '8'" or "No intersection", which reads here as the node's value, or 0 for none: node values
// are at least 1, so 0 names no node.
internal static class IntersectionJudge
{
    // LeetCode's intersectVal for two lists that never meet, and the answer when the function finds no node.
    public const int NoIntersection = 0;

    // The head of listB once its node at skipB is listA's node at skipA: listB's first skipB nodes lead into
    // listA's tail, and listB's own nodes from skipB on are dropped. listB is returned as it is when the two
    // lists never meet. listA is left as it is, being the other head.
    public static SinglyLinkedListNode<int>? HeadBJoinedToA(
        int intersectVal, SinglyLinkedListNode<int>? listA, SinglyLinkedListNode<int>? listB, (int SkipA, int SkipB) skips)
    {
        if (intersectVal == NoIntersection)
        {
            return listB;
        }

        var shared = NodeAt(listA, skips.SkipA);

        if (skips.SkipB == 0)
        {
            return shared;
        }

        var lastOwn = NodeAt(listB, skips.SkipB - 1);
        lastOwn.Next = shared;

        return listB;
    }

    // The node at index skip, which the judge's skips always reach when the lists meet.
    private static SinglyLinkedListNode<int> NodeAt(SinglyLinkedListNode<int>? head, int skip)
    {
        var node = head;

        for (var index = 0; index < skip; index++)
        {
            node = node?.Next;
        }

        return node ?? throw new ArgumentOutOfRangeException(nameof(skip), skip, "The skip runs past the list.");
    }

    // What the judge prints for the node returned, as its value: 0 for none.
    public static int Printed(SinglyLinkedListNode<int>? intersection) =>
        intersection?.Value ?? NoIntersection;
}
