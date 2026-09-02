using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.MergeKSortedLists;

// LeetCode 23. Merge k Sorted Lists: merge k already-sorted singly linked lists
// into one sorted list.
//
// The two strategies differ in how they find "the next smallest node": a min-heap
// kept over each list's current head, or flatten every value into a BCL array,
// let Array.Sort do the ordering, and rebuild a fresh list from the result.
internal static class MergeKSortedListsSolution
{
    // The composed solution: a k-way merge over a min-heap of each list's current
    // head. Popping the smallest head and pushing its successor keeps the heap at
    // size <= k throughout, splicing the popped nodes themselves onto the output
    // rather than allocating new ones - O(n log k) for n total nodes across k
    // lists.
    public static SinglyLinkedListNode<int>? MergeListsByHeap(SinglyLinkedListNode<int>?[] lists)
    {
        var heap = new Heap<SinglyLinkedListNode<int>, NodeOrder>();

        foreach (var list in lists)
        {
            if (list is not null)
            {
                heap.Push(list);
            }
        }

        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        while (heap.TryPop(out var node))
        {
            if (node.Next is not null)
            {
                heap.Push(node.Next);
            }

            tail.Next = node;
            tail = node;
        }

        tail.Next = null;
        return dummy.Next;
    }

    // The textbook approach many first reach for: read every value out of every
    // list into one BCL array, let Array.Sort order it, then build a brand-new
    // list from the sorted values. Deliberately written without this repo's Heap
    // beyond the input/output list shape itself - it is the arm MergeListsByHeap
    // has to justify itself against.
    public static SinglyLinkedListNode<int>? MergeListsByFlattenSort(SinglyLinkedListNode<int>?[] lists)
    {
        var values = new List<int>();

        foreach (var list in lists)
        {
            for (var node = list; node is not null; node = node.Next)
            {
                values.Add(node.Value);
            }
        }

        var sorted = values.ToArray();
        Array.Sort(sorted);

        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in sorted)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    private readonly struct NodeOrder : IHeapOrder<SinglyLinkedListNode<int>>
    {
        public static bool HasPriority(SinglyLinkedListNode<int> candidate, SinglyLinkedListNode<int> incumbent)
            => candidate.Value < incumbent.Value;
    }
}
