using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.LinkedListCycleII;

// LeetCode 142. Linked List Cycle II: given a singly linked list's head, return
// the node where the cycle begins, or null if the list has no cycle.
//
// Same shape as LC 141 (LinkedListCycle) one tier down - a BCL HashSet baseline
// against this repo's own Floyd detector - except both strategies here must also
// report *which* node starts the cycle, not just whether one exists.
internal static class LinkedListCycleIISolution
{
    // Textbook baseline: record every visited node reference in a BCL HashSet;
    // the first node seen twice is the cycle's entry point. O(n) extra space,
    // written without this repo's primitives - the arm the composed strategy has
    // to beat.
    public static SinglyLinkedListNode<int>? DetectCycleByVisitedSet(SinglyLinkedListNode<int>? head)
    {
        var visited = new HashSet<SinglyLinkedListNode<int>>();

        for (var node = head; node is not null; node = node.Next)
        {
            if (!visited.Add(node))
            {
                return node;
            }
        }

        return null;
    }

    // This repo's own Floyd meeting-point-plus-entry-walk detector - O(1) extra
    // space.
    public static SinglyLinkedListNode<int>? DetectCycleByFloydCycleDetection(SinglyLinkedListNode<int>? head) =>
        CycleDetection.FindCycleStart(head);
}
