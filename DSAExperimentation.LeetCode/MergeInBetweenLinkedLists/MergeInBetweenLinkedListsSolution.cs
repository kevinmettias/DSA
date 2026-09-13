using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.MergeInBetweenLinkedLists;

// LeetCode 1669. Merge In Between Linked Lists: remove list1's nodes from index a
// through index b inclusive and put the whole of list2 in their place, returning
// list1's head.
//
// Both strategies produce the same list, but pay differently for it. The baseline
// materializes list1's values into a List<int>, splices the window out and list2's
// values in - O(n) shifting plus a full rebuild - where the pointer walk finds the
// two cut nodes directly on the existing chain and rewires two Next pointers,
// touching nothing outside the removed range. The same "traverse-then-rewire-.Next"
// shape RotateList uses for LC 61, just two cut points instead of one.
internal static class MergeInBetweenLinkedListsSolution
{
    // The node just after `before` is index a, so the walk to `before` is a - 1
    // steps; `after` is index b + 1, which is b - a + 2 steps further on.
    private const int WindowBoundaryOffset = 2;

    // Textbook baseline: copy both lists' values out, RemoveRange the [a, b]
    // window, InsertRange list2's values at a, and rebuild a fresh list from the
    // result. Deliberately all BCL inside - it is the arm the pointer splice below
    // has to justify itself against - and it leaves the input chains untouched,
    // where the splice consumes them.
    public static SinglyLinkedListNode<int> MergeInBetweenByArrayRebuild(
        SinglyLinkedListNode<int> list1, int a, int b, SinglyLinkedListNode<int> list2)
    {
        var merged = new List<int>(ValuesOf(list1));
        merged.RemoveRange(a, b - a + 1);
        merged.InsertRange(a, ValuesOf(list2));

        return BuildList(merged);
    }

    // The standard walk: step to the node just before index a and the node just
    // after index b, hang list2 off the first, and hang the tail of the list back
    // off list2's last node. Two pointer rewrites, no extra storage, and the cost
    // is the walk to the window rather than the size of the list.
    public static SinglyLinkedListNode<int> MergeInBetweenByPointerSplice(
        SinglyLinkedListNode<int> list1, int a, int b, SinglyLinkedListNode<int> list2)
    {
        var before = list1;

        for (var i = 0; i < a - 1; i++)
        {
            before = before.Next!;
        }

        var after = before;

        for (var i = 0; i < b - a + WindowBoundaryOffset; i++)
        {
            after = after.Next!;
        }

        before.Next = list2;

        var list2Tail = list2;

        while (list2Tail.Next is not null)
        {
            list2Tail = list2Tail.Next;
        }

        list2Tail.Next = after;

        return list1;
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
}
