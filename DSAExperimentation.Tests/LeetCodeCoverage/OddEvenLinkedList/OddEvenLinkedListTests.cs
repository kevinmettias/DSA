using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OddEvenLinkedList;

// LeetCode 328. Odd Even Linked List: in-place pointer rewiring over this repo's own
// SinglyLinkedListNode<TValue> - two running cursors (odd/even) splice the list into
// its odd-indexed nodes followed by its even-indexed nodes, preserving each half's
// relative order, in O(n) time and O(1) extra space.
public sealed partial class OddEvenLinkedListTests
{
    [Fact]
    public void GroupOddEven_FiveNodes_InterleavesOddThenEvenIndices()
    {
        var head = Build([1, 2, 3, 4, 5]);

        var reordered = GroupOddEven(head);

        Assert.Equal([1, 3, 5, 2, 4], ToArray(reordered));
    }

    [Fact]
    public void GroupOddEven_SevenNodes_InterleavesOddThenEvenIndices()
    {
        var head = Build([2, 1, 3, 5, 6, 4, 7]);

        var reordered = GroupOddEven(head);

        Assert.Equal([2, 3, 6, 7, 1, 5, 4], ToArray(reordered));
    }

    [Fact]
    public void GroupOddEven_EmptyList_ReturnsNull()
    {
        var reordered = GroupOddEven(null);

        Assert.Null(reordered);
    }

    [Fact]
    public void GroupOddEven_SingleNode_ReturnsSameNode()
    {
        var head = Build([42]);

        var reordered = GroupOddEven(head);

        Assert.Equal([42], ToArray(reordered));
    }

    private static SinglyLinkedListNode<int>? GroupOddEven(SinglyLinkedListNode<int>? head)
    {
        if (head?.Next is null)
        {
            return head;
        }

        var odd = head;
        var even = head.Next;
        var evenHead = even;

        while (even?.Next is not null)
        {
            odd.Next = even.Next;
            odd = odd.Next;
            even.Next = odd.Next;
            even = even.Next;
        }

        odd.Next = evenHead;
        return head;
    }

    private static SinglyLinkedListNode<int>? Build(int[] values)
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

        return [.. values];
    }
}
