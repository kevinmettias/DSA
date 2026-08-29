using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeKSortedLists;

// LeetCode 23. Merge k Sorted Lists: use the repo's Heap<T,TOrder> as the
// frontier over current list heads, splicing SinglyLinkedListNode<T> values.
public sealed partial class MergeKSortedListsTests
{
    [Fact]
    public void MergeKLists_ThreeSortedLists_ReturnsOneSortedList()
    {
        SinglyLinkedListNode<int>?[] lists = [BuildList([1, 4, 5]), BuildList([1, 3, 4]), BuildList([2, 6])];

        var merged = MergeKLists(lists);

        Assert.Equal([1, 1, 2, 3, 4, 4, 5, 6], ToArray(merged));
    }

    private static SinglyLinkedListNode<int>? MergeKLists(SinglyLinkedListNode<int>?[] lists)
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

    private readonly struct NodeOrder : IHeapOrder<SinglyLinkedListNode<int>>
    {
        public static bool HasPriority(SinglyLinkedListNode<int> candidate, SinglyLinkedListNode<int> incumbent)
            => candidate.Value < incumbent.Value;
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
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
