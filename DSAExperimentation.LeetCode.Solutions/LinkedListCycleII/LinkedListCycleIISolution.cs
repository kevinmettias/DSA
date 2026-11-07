using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Solutions.LinkedListCycleII;

// LeetCode 142. Linked List Cycle II: given a singly linked list's head, return
// the node where the cycle begins, or null if the list has no cycle.
//
// Same shape as LC 141 (LinkedListCycle) one tier down - a BCL HashSet baseline
// against this repo's own Floyd detector - except both strategies here must also
// report *which* node starts the cycle, not just whether one exists.
[LeetCodeQuestion("linked-list-cycle-ii", LeetCodeAnswerOrder.AsWritten, RewritesInput = true)]
internal static class LinkedListCycleIISolution
{
    // What LeetCode's judge prints, "no cycle", read here as an index no node has.
    public const int NoCycle = -1;

    // LeetCode's shape (ARCHITECTURE 17.4): its judge reads the list and pos, links the tail back to the node at
    // index pos (LeetCodeWireFormat.WithCycleAt), and prints the node returned by its index - "tail connects to
    // node index 1" - or "no cycle". Linking rewrites the list, so a harness hands each call a list of its own.
    public static int DetectCycleByVisitedSet(SinglyLinkedListNode<int>? head, int pos)
    {
        var linked = LeetCodeWireFormat.WithCycleAt(head, pos);
        var entry = DetectCycleByVisitedSet(linked);

        return IndexOf(linked, entry);
    }

    // The index the judge prints for a node of the list: its distance from the head, or NoCycle for none.
    private static int IndexOf(SinglyLinkedListNode<int>? head, SinglyLinkedListNode<int>? entry)
    {
        if (entry is null)
        {
            return NoCycle;
        }

        var index = 0;

        for (var node = head; !ReferenceEquals(node, entry); node = node?.Next ?? throw new InvalidOperationException("The entry lies on the list."))
        {
            index++;
        }

        return index;
    }

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

    // LeetCode's shape, as for the visited-set strategy above.
    public static int DetectCycleByFloydCycleDetection(SinglyLinkedListNode<int>? head, int pos)
    {
        var linked = LeetCodeWireFormat.WithCycleAt(head, pos);
        var entry = DetectCycleByFloydCycleDetection(linked);

        return IndexOf(linked, entry);
    }

    // This repo's own Floyd meeting-point-plus-entry-walk detector - O(1) extra
    // space.
    public static SinglyLinkedListNode<int>? DetectCycleByFloydCycleDetection(SinglyLinkedListNode<int>? head) =>
        CycleDetection.FindCycleStart(head);
}
