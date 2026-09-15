using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.MiddleOfTheLinkedList;

// LeetCode 876. Middle of the Linked List: return the middle node of a singly
// linked list, the second middle one when the length is even.
//
// Both strategies answer with the node itself - LeetCode's actual answer shape,
// which is what the walk has to land on; reading a value off it afterwards is the
// caller's business.
internal static class MiddleOfTheLinkedListSolution
{

    // Floyd's slow/fast two-pointer walk over this repo's own
    // SinglyLinkedListNode<TValue>.Next - the same node representation
    // CycleDetection.FindMeetingPoint advances two-steps-per-one, applied here to
    // an (always acyclic) list so the slow pointer lands on the midpoint rather
    // than a cycle's meeting point. One pass, each node touched once.
    public static SinglyLinkedListNode<int>? MiddleNodeBySlowFastTwoPointer(SinglyLinkedListNode<int>? head)
    {
        var slow = head;
        var fast = head;

        while (fast?.Next is not null)
        {
            slow = slow!.Next;
            fast = fast.Next.Next;
        }

        return slow;
    }

    // The textbook baseline: count the list, then walk length/2 steps from the
    // head. Also O(n), but it touches every node twice. Deliberately nothing but
    // the input list's own Next links - this is what you would write without this
    // repo.
    public static SinglyLinkedListNode<int>? MiddleNodeByCountThenWalk(SinglyLinkedListNode<int>? head)
    {
        var count = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            count++;
        }

        var target = count / AlgorithmConstants.HalvingFactor;
        var current = head;

        for (var step = 0; step < target; step++)
        {
            current = current!.Next;
        }

        return current;
    }
}
