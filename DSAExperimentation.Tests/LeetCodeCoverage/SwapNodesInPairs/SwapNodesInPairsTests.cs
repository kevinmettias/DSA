using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SwapNodesInPairs;

// LeetCode 24. Swap Nodes in Pairs: pointer rewiring over this repo's mutable
// SinglyLinkedListNode<T> primitive.
public sealed partial class SwapNodesInPairsTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 4 }, new[] { 2, 1, 4, 3 })]
    [InlineData(new[] { 1, 2, 3 }, new[] { 2, 1, 3 })]
    public void SwapPairs_VariedLengths_SwapsAdjacentPairs(int[] values, int[] expected)
        => Assert.Equal(expected, ToArray(SwapPairs(BuildList(values))));

    private static SinglyLinkedListNode<int>? SwapPairs(SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var previous = dummy;

        while (previous.Next?.Next is not null)
        {
            var first = previous.Next;
            var second = first.Next!;
            first.Next = second.Next;
            second.Next = first;
            previous.Next = second;
            previous = first;
        }

        return dummy.Next;
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
