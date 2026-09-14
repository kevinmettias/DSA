using DSAExperimentation.DataStructures.SinglyLinkedList;
using RepoNodeStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.DataStructures.SinglyLinkedList.SinglyLinkedListNode<int>>;

namespace DSAExperimentation.LeetCode.RemoveNodesFromLinkedList;

// LeetCode 2487. Remove Nodes From Linked List: a node survives only if its value
// is >= every value to its right, so the surviving nodes form a non-increasing
// sequence left to right.
//
// Both strategies answer that same question and both return LeetCode's own answer -
// the head of the spliced list - they differ only in how "is there a greater value
// to my right?" is decided: rescan the tail per node, or let a monotonic stack
// remember it.
//
// Both splice .Next in place, reusing the original nodes rather than allocating a
// second list, which is what LeetCode's own signature invites; a caller that needs
// the input list afterwards has to build it per call.
internal static class RemoveNodesFromLinkedListSolution
{
    // The textbook answer: for every node, walk the rest of the list looking for a
    // strictly greater value, and keep the node only when there is none - O(n^2)
    // with no auxiliary structure at all. Deliberately written without this repo's
    // primitives; it is the arm the monotonic sweep has to justify itself against.
    //
    // Relinking as it goes is safe for the rescan: the only pointer this ever
    // rewrites is the last kept node's, which is always behind the node currently
    // being scanned, so the tail each scan walks is still the original one.
    public static SinglyLinkedListNode<int>? RemoveNodesByBruteForceScan(SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        for (var node = head; node is not null; node = node.Next)
        {
            if (!HasGreaterValueToTheRight(node))
            {
                tail.Next = node;
                tail = node;
            }
        }

        tail.Next = null;

        return dummy.Next;
    }

    private static bool HasGreaterValueToTheRight(SinglyLinkedListNode<int> node)
    {
        for (var later = node.Next; later is not null; later = later.Next)
        {
            if (later.Value > node.Value)
            {
                return true;
            }
        }

        return false;
    }

    // One left-to-right sweep through this repo's own Stack<T>, holding node
    // references instead of values - the same monotonic-stack shape AsteroidCollision
    // and DailyTemperatures use. Any stacked node smaller than the incoming one has
    // just been proven to have a greater node to its right, so it is popped before
    // the incoming node is pushed. What remains, bottom to top, is exactly the
    // answer in order: popped into an array back-to-front, then relinked in one
    // more pass.
    public static SinglyLinkedListNode<int>? RemoveNodesByMonotonicStack(SinglyLinkedListNode<int>? head)
    {
        var keep = new RepoNodeStack();

        for (var node = head; node is not null; node = node.Next)
        {
            while (keep.TryPeek(out var top) && top.Value < node.Value)
            {
                keep.TryPop(out _);
            }

            keep.Push(node);
        }

        return RelinkInOrder(keep);
    }

    // The stack hands survivors back top-down, i.e. last-to-first, so the array is
    // filled back-to-front before the .Next chain is rewritten front-to-back.
    private static SinglyLinkedListNode<int>? RelinkInOrder(RepoNodeStack keep)
    {
        var survivors = new SinglyLinkedListNode<int>[keep.Count];

        for (var i = survivors.Length - 1; i >= 0; i--)
        {
            keep.TryPop(out var survivor);
            survivors[i] = survivor;
        }

        for (var i = 0; i < survivors.Length - 1; i++)
        {
            survivors[i].Next = survivors[i + 1];
        }

        if (survivors.Length == 0)
        {
            return null;
        }

        survivors[^1].Next = null;

        return survivors[0];
    }
}
