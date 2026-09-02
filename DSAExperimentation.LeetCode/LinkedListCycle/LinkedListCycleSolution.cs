using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.LinkedListCycle;

// LeetCode 141. Linked List Cycle: given a singly linked list's head, report
// whether it contains a cycle.
//
// The two strategies differ only in how much space they spend to answer: the
// baseline remembers every node reference it has already walked past, while the
// composed strategy delegates to this repo's own Floyd tortoise-and-hare
// detector, which needs none.
internal static class LinkedListCycleSolution
{
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
