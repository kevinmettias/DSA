using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Solutions.LinkedListCycle;

// LeetCode 141. Linked List Cycle: given a singly linked list's head, report
// whether it contains a cycle.
//
// The two strategies differ only in how much space they spend to answer: the
// baseline remembers every node reference it has already walked past, while the
// composed strategy delegates to this repo's own Floyd tortoise-and-hare
// detector, which needs none.
[LeetCodeQuestion("linked-list-cycle", LeetCodeAnswerOrder.AsWritten, RewritesInput = true)]
internal static class LinkedListCycleSolution
{
    // LeetCode's shape (ARCHITECTURE 17.4): its signature lists pos, the node its judge links the tail back
    // to, which the function itself is never handed. The cycle is built into the list read for the case, and
    // then the strategy is asked, so a harness hands each call a list of its own.
    public static bool HasCycleByVisitedSet(SinglyLinkedListNode<int>? head, int pos)
    {
        var linked = LeetCodeWireFormat.WithCycleAt(head, pos);

        return HasCycleByVisitedSet(linked);
    }

    public static bool HasCycleByFloydCycleDetection(SinglyLinkedListNode<int>? head, int pos)
    {
        var linked = LeetCodeWireFormat.WithCycleAt(head, pos);

        return HasCycleByFloydCycleDetection(linked);
    }

    // Textbook baseline: record every visited node reference in a BCL HashSet
    // and stop the first time a node repeats. O(n) extra space, written without
    // this repo's primitives - the arm the composed strategy has to beat.
    public static bool HasCycleByVisitedSet(SinglyLinkedListNode<int>? head)
    {
        var visited = new HashSet<SinglyLinkedListNode<int>>();

        for (var node = head; node is not null; node = node.Next)
        {
            if (!visited.Add(node))
            {
                return true;
            }
        }

        return false;
    }

    // This repo's own Floyd tortoise-and-hare cycle detector - O(1) extra space.
    public static bool HasCycleByFloydCycleDetection(SinglyLinkedListNode<int>? head) =>
        CycleDetection.HasCycle(head);
}
