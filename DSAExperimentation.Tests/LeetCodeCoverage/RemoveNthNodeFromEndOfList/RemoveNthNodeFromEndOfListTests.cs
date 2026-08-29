using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveNthNodeFromEndOfList;

// LeetCode 19. Remove Nth Node From End of List: compose the repo's mutable
// SinglyLinkedListNode<T> with the standard two-runner pointer walk.
public sealed partial class RemoveNthNodeFromEndOfListTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, new[] { 1, 2, 3, 5 })]
    [InlineData(new[] { 1 }, 1, new int[] { })]
    [InlineData(new[] { 1, 2 }, 2, new[] { 2 })]
    public void RemoveNthFromEnd_VariedInputs_RemovesExpectedNode(int[] values, int n, int[] expected)
        => Assert.Equal(expected, ToArray(RemoveNthFromEnd(BuildList(values), n)));

    private static SinglyLinkedListNode<int>? RemoveNthFromEnd(SinglyLinkedListNode<int>? head, int n)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var fast = dummy;
        var slow = dummy;

        for (var i = 0; i < n; i++)
        {
            fast = fast.Next!;
        }

        while (fast.Next is not null)
        {
            fast = fast.Next;
            slow = slow.Next!;
        }

        slow.Next = slow.Next?.Next;
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
