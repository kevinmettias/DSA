using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseNodesInKGroup;

// LeetCode 25. Reverse Nodes in k-Group: bounded pointer reversal over
// SinglyLinkedListNode<T>, leaving the final short group untouched.
public sealed partial class ReverseNodesInKGroupTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, new[] { 2, 1, 4, 3, 5 })]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 3, new[] { 3, 2, 1, 4, 5 })]
    public void ReverseKGroup_LeetCodeExamples_ReversesOnlyCompleteGroups(int[] values, int k, int[] expected)
        => Assert.Equal(expected, ToArray(ReverseKGroup(BuildList(values), k)));

    private static SinglyLinkedListNode<int>? ReverseKGroup(SinglyLinkedListNode<int>? head, int k)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var groupPrevious = dummy;

        while (TryGetKth(groupPrevious, k, out var kth))
        {
            var groupNext = kth.Next;
            SinglyLinkedListNode<int>? previous = groupNext;
            var current = groupPrevious.Next;

            while (current != groupNext)
            {
                var next = current!.Next;
                current.Next = previous;
                previous = current;
                current = next;
            }

            var oldGroupHead = groupPrevious.Next!;
            groupPrevious.Next = kth;
            groupPrevious = oldGroupHead;
        }

        return dummy.Next;
    }

    private static bool TryGetKth(SinglyLinkedListNode<int> groupPrevious, int k, out SinglyLinkedListNode<int> kth)
    {
        SinglyLinkedListNode<int>? node = groupPrevious;
        for (var i = 0; i < k && node is not null; i++)
        {
            node = node.Next;
        }

        kth = node!;
        return node is not null;
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
