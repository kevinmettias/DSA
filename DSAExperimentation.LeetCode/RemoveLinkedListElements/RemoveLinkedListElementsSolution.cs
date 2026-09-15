using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.RemoveLinkedListElements;

// LeetCode 203. Remove Linked List Elements: delete every node whose value
// equals val, anywhere in the list including the head.
//
// Both strategies land on the same filtered order. The baseline materializes the
// list's values into an array, filters it with BCL LINQ, and rebuilds a fresh
// list from the result; the dummy-head walk reuses the existing nodes, splicing
// each match out in place as it is visited - the sentinel node in front of head
// is what lets the walk remove the head itself with the same code path as any
// other match.
internal static class RemoveLinkedListElementsSolution
{
    // Textbook baseline: copy the list's values into an array, filter out val with
    // LINQ, and rebuild a fresh list from the result. O(Length) extra space, where
    // the dummy-head walk below needs none. Deliberately written without this
    // repo's own list-walking idioms - the arm the splice below has to justify
    // itself against.
    public static SinglyLinkedListNode<int>? RemoveElementsByArrayRebuild(
        SinglyLinkedListNode<int>? head, int val)
        => BuildList(ToArray(head).Where(value => value != val));

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

    // LeetCode's own idiomatic answer: a dummy node in front of head lets the walk
    // splice out a match - head included - by rewiring the previous node's Next,
    // never special-casing "is this the head". No extra storage.
    public static SinglyLinkedListNode<int>? RemoveElementsByDummyHeadSplice(
        SinglyLinkedListNode<int>? head, int val)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var previous = dummy;

        while (previous.Next is not null)
        {
            if (previous.Next.Value == val)
            {
                previous.Next = previous.Next.Next;
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
}
