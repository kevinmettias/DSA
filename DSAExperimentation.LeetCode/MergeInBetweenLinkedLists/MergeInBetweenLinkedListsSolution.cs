using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.MergeInBetweenLinkedLists;

// LeetCode 1669. Merge In Between Linked Lists: remove list1's nodes between
// indices fromIndex and toIndex inclusive and put the whole of list2 in their place,
// returning list1's head.
//
// Both strategies produce the same list, but pay differently for it. The baseline
// materializes list1's values into a List<int>, splices the window out and list2's
// values in - O(n) shifting plus a full rebuild - where the pointer walk finds the
// two cut nodes directly on the existing chain and rewires two Next pointers,
// touching nothing outside the removed range. The same "traverse-then-rewire-.Next"
// shape RotateList uses for LC 61, just two cut points instead of one.
internal static class MergeInBetweenLinkedListsSolution
{
    // The node just after `before` is index fromIndex, so the walk to `before` is
    // fromIndex - 1 steps; `after` is index toIndex + 1, which is toIndex - fromIndex
    // + 2 steps further on.
    private const int WindowBoundaryOffset = 2;

    // Textbook baseline: copy both lists' values out, RemoveRange the
    // [fromIndex, toIndex] window, InsertRange list2's values at fromIndex, and
    // rebuild a fresh list from the result. Deliberately all BCL inside - it is the
    // arm the pointer splice below has to justify itself against - and it leaves the
    // input chains untouched, where the splice consumes them.
    public static SinglyLinkedListNode<int> MergeInBetweenByArrayRebuild(
        SinglyLinkedListNode<int> list1, int fromIndex, int toIndex, SinglyLinkedListNode<int> list2)
    {
        var merged = new List<int>(ValuesOf(list1));
        merged.RemoveRange(fromIndex, toIndex - fromIndex + 1);
        merged.InsertRange(fromIndex, ValuesOf(list2));

        return BuildList(merged);
    }

    private static List<int> ValuesOf(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }

    private static SinglyLinkedListNode<int> BuildList(List<int> values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next!;
    }

    // The standard walk: step to the node just before index fromIndex and the node
    // just after index toIndex, hang list2 off the first, and hang the tail of the
    // list back off list2's last node. Two pointer rewrites, no extra storage, and
    // the cost is the walk to the window rather than the size of the list.
    public static SinglyLinkedListNode<int> MergeInBetweenByPointerSplice(
        SinglyLinkedListNode<int> list1, int fromIndex, int toIndex, SinglyLinkedListNode<int> list2)
    {
        var before = Advance(list1, fromIndex - 1);
        var after = Advance(before, toIndex - fromIndex + WindowBoundaryOffset);

        before.Next = list2;
        TailOf(list2).Next = after;

        return list1;
    }

    // The node `steps` links past `node`: what both cut points - the one just before
    // index fromIndex and the one just past index toIndex - are found with.
    private static SinglyLinkedListNode<int> Advance(SinglyLinkedListNode<int> node, int steps)
    {
        for (var i = 0; i < steps; i++)
        {
            node = node.Next!;
        }

        return node;
    }

    // list2's own last node, which list1's surviving tail is reattached to.
    private static SinglyLinkedListNode<int> TailOf(SinglyLinkedListNode<int> head)
    {
        var tail = head;

        while (tail.Next is not null)
        {
            tail = tail.Next;
        }

        return tail;
    }
}
