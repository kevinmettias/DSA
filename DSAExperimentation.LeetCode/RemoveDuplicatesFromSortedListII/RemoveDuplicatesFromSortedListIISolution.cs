using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedListII;

// LeetCode 82. Remove Duplicates from Sorted List II: given a sorted list, delete
// every node whose value occurs more than once, keeping only the values that
// occur exactly once, in their original order.
//
// Both strategies land on the same surviving values; they differ in whether the
// duplicate check is a BCL LINQ grouping over the list's values, or a single pass
// with a dummy head that splices duplicate runs out of the existing nodes.
internal static class RemoveDuplicatesFromSortedListIISolution
{
    // The textbook baseline: copy the list's values into an array and let BCL
    // LINQ's GroupBy find each value's total run length, keeping only the
    // singletons and rebuilding a fresh list from them. Deliberately written
    // without this repo's own list-walking idioms - the arm the in-place scan
    // below has to justify itself against.
    public static SinglyLinkedListNode<int>? DeleteDuplicatesByArrayGroupFilter(SinglyLinkedListNode<int>? head)
    {
        var survivors = ToArray(head)
            .GroupBy(value => value)
            .Where(group => group.Count() == 1)
            .Select(group => group.Key);

        return BuildList(survivors);
    }

    // One pass with a dummy head and a previous/current pair: walk past every run
    // of two or more equal values and splice the whole run out, in place.
    // LeetCode's own idiomatic answer.
    public static SinglyLinkedListNode<int>? DeleteDuplicatesByTwoPointerScan(SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var previous = dummy;

        while (previous.Next is not null)
        {
            var current = previous.Next;
            var duplicated = false;

            while (current.Next is not null && current.Value == current.Next.Value)
            {
                duplicated = true;
                current = current.Next;
            }

            if (duplicated)
            {
                previous.Next = current.Next;
            }
            else
            {
                previous = previous.Next;
            }
        }

        return dummy.Next;
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
