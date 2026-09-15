using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.PartitionList;

// LeetCode 86. Partition List: given a linked list and a value x, partition it so
// that every node less than x comes before every node greater than or equal to x,
// preserving each side's original relative order.
//
// Both strategies land on the same partitioned order. The baseline materializes the
// list's values into an array, stably partitions it with a filter/concat pass, and
// rebuilds a fresh list from the result; the splice walk reuses the existing nodes,
// threading each one onto a "before" or "after" chain as it is visited, with no
// extra storage beyond the two chain heads.
internal static class PartitionListSolution
{
    // Textbook baseline: copy the list's values into an array, stably partition the
    // array with LINQ, and rebuild a fresh list from the result. O(Length) extra
    // space, where the splice walk below needs none.
    public static SinglyLinkedListNode<int>? PartitionByArrayRebuild(SinglyLinkedListNode<int>? head, int x)
    {
        var values = ToArray(head);
        var partitioned = values.Where(value => value < x).Concat(values.Where(value => value >= x));

        return BuildList(partitioned);
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

    // The standard walk: thread each node onto a "before" or "after" chain as it is
    // visited, cutting it loose from its old neighbor first, then splice the two
    // chains together. No extra storage; the existing nodes are reused.
    public static SinglyLinkedListNode<int>? PartitionByPointerSplice(SinglyLinkedListNode<int>? head, int x)
    {
        var before = new SinglyLinkedListNode<int>(0);
        var after = new SinglyLinkedListNode<int>(0);

        var (beforeTail, afterTail) = ThreadOntoChains(head, x, (before, after));

        beforeTail.Next = after.Next;
        return before.Next;
    }

    // The walk itself: cut each visited node loose from its old neighbor, append it
    // to the chain for its side, and hand back the two chain tails - the dummy heads
    // passed in are those tails before any node has been threaded.
    private static (SinglyLinkedListNode<int> BeforeTail, SinglyLinkedListNode<int> AfterTail) ThreadOntoChains(
        SinglyLinkedListNode<int>? head,
        int x,
        (SinglyLinkedListNode<int> BeforeTail, SinglyLinkedListNode<int> AfterTail) tails)
    {
        for (var node = head; node is not null;)
        {
            var next = node.Next;
            node.Next = null;

            if (node.Value < x)
            {
                tails.BeforeTail.Next = node;
                tails.BeforeTail = node;
            }
            else
            {
                tails.AfterTail.Next = node;
                tails.AfterTail = node;
            }

            node = next;
        }

        return tails;
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
