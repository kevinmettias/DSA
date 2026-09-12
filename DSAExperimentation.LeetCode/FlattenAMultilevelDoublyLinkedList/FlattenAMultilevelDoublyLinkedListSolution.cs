using PendingStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.LeetCode.FlattenAMultilevelDoublyLinkedList.Node>;

namespace DSAExperimentation.LeetCode.FlattenAMultilevelDoublyLinkedList;

// LeetCode 430. Flatten a Multilevel Doubly Linked List: splice every Child sublist
// into the main list in depth-first order, in place, leaving no Child pointers behind.
//
// FlattenByStack is this repo's own LIFO Stack<T> holding each level's not-yet-resumed
// Next pointer - the same "push where DFS should resume, descend now" shape
// FlattenNestedListIteratorSolution already uses for LC 341's nested lists, here
// descending into Child instead of a NestedInteger's own sublist. FlattenByBruteForceRescan
// is the textbook baseline it has to justify itself against: rescan from head after every
// single splice (the classic "no bookkeeping" naive flatten, O(n^2) once every node has a
// child), using nothing from this repo.
internal static class FlattenAMultilevelDoublyLinkedListSolution
{
    public static Node? FlattenByBruteForceRescan(Node? head)
    {
        while (TrySpliceNextChild(head))
        {
        }

        return head;
    }

    private static bool TrySpliceNextChild(Node? head)
    {
        var splicePoint = FindNodeWithChild(head);
        if (splicePoint is null)
        {
            return false;
        }

        var after = splicePoint.Next;
        var childHead = AttachChildAsNext(splicePoint);
        var tail = FindTail(childHead);
        ReattachRemainder(tail, after);

        return true;
    }

    private static Node AttachChildAsNext(Node splicePoint)
    {
        var childHead = splicePoint.Child!;
        splicePoint.Next = childHead;
        childHead.Previous = splicePoint;
        splicePoint.Child = null;
        return childHead;
    }

    private static Node FindTail(Node node)
    {
        var tail = node;
        while (tail.Next is not null)
        {
            tail = tail.Next;
        }

        return tail;
    }

    private static void ReattachRemainder(Node tail, Node? after)
    {
        tail.Next = after;
        if (after is not null)
        {
            after.Previous = tail;
        }
    }

    private static Node? FindNodeWithChild(Node? head)
    {
        for (var node = head; node is not null; node = node.Next)
        {
            if (node.Child is not null)
            {
                return node;
            }
        }

        return null;
    }

    public static Node? FlattenByStack(Node? head)
    {
        if (head is null)
        {
            return null;
        }

        var pending = new PendingStack();
        var current = head;

        while (current is not null)
        {
            AdvanceNode(current, pending);
            current = current.Next;
        }

        return head;
    }

    private static void AdvanceNode(Node current, PendingStack pending)
    {
        if (current.Child is not null)
        {
            DescendIntoChild(current, current.Child, pending);
        }

        if (current.Next is null && pending.TryPop(out var resumed))
        {
            current.Next = resumed;
            resumed.Previous = current;
        }
    }

    private static void DescendIntoChild(Node current, Node child, PendingStack pending)
    {
        if (current.Next is not null)
        {
            pending.Push(current.Next);
        }

        current.Next = child;
        child.Previous = current;
        current.Child = null;
    }
}
