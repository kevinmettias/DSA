using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedList;

// LeetCode 83. Remove Duplicates from Sorted List: collapse every run of equal
// values in a sorted list down to one node each, keeping the first of each run.
//
// Both strategies land on the same distinct-value sequence; they differ in
// whether the collapse is BCL LINQ's Distinct() over the list's values, or a
// single pass that splices repeated nodes out of the existing list in place.
internal static class RemoveDuplicatesFromSortedListSolution
{
    // The textbook baseline: copy the list's values into an array and let BCL
    // LINQ's Distinct() do the collapsing, then rebuild a fresh list from the
    // result. Deliberately written without this repo's own list-walking idioms -
    // the arm the in-place scan below has to justify itself against.
    public static SinglyLinkedListNode<int>? DeleteDuplicatesByDistinctFilter(SinglyLinkedListNode<int>? head)
        => BuildList(ToArray(head).Distinct());

    // One pass, one pointer: skip forward past every node whose value repeats the
    // current one, splicing it out of the list in place. LeetCode's own idiomatic
    // answer.
    public static SinglyLinkedListNode<int>? DeleteDuplicatesByInPlaceScan(SinglyLinkedListNode<int>? head)
    {
        for (var node = head; node is not null; node = node.Next)
        {
            while (node.Next is not null && node.Value == node.Next.Value)
            {
                node.Next = node.Next.Next;
            }
        }

        return head;
    }

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values.ToArray();
    }

    private static SinglyLinkedListNode<int>? BuildList(IEnumerable<int> values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }
}
