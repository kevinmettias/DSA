using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeTwoSortedLists;

// LeetCode 21. Merge Two Sorted Lists: the standard dummy-head merge walk over this
// repo's SinglyLinkedListNode<T>.Next, splicing existing nodes rather than copying
// values.
public sealed partial class MergeTwoSortedListsTests
{
    [Fact]
    public void Merge_TwoSortedLists_InterleavesIntoOneSortedList()
    {
        var first = BuildList([1, 2, 4]);
        var second = BuildList([1, 3, 4]);

        var merged = MergeSortedLists(first, second);

        Assert.Equal([1, 1, 2, 3, 4, 4], ToArray(merged));
    }

    [Fact]
    public void Merge_OneListEmpty_ReturnsTheOtherListUnchanged()
    {
        var first = BuildList([]);
        var second = BuildList([5, 6]);

        var merged = MergeSortedLists(first, second);

        Assert.Equal([5, 6], ToArray(merged));
    }

    private static SinglyLinkedListNode<int>? MergeSortedLists(
        SinglyLinkedListNode<int>? first, SinglyLinkedListNode<int>? second)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        while (first is not null && second is not null)
        {
            if (first.Value <= second.Value)
            {
                tail.Next = first;
                first = first.Next;
            }
            else
            {
                tail.Next = second;
                second = second.Next;
            }

            tail = tail.Next;
        }

        tail.Next = first ?? second;
        return dummy.Next;
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        SinglyLinkedListNode<int>? head = null;
        SinglyLinkedListNode<int>? tail = null;

        foreach (var value in values)
        {
            var node = new SinglyLinkedListNode<int>(value);
            head ??= node;
            AppendAfter(tail, node);
            tail = node;
        }

        return head;
    }

    // No previous node to link on the very first iteration (tail is still null) -
    // head itself becomes that first node instead, back in BuildList.
    private static void AppendAfter(SinglyLinkedListNode<int>? tail, SinglyLinkedListNode<int> node)
    {
        if (tail is not null)
        {
            tail.Next = node;
        }
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
