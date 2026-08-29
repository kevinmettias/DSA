using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RotateList;

// LeetCode 61. Rotate List: compose the repo's mutable SinglyLinkedListNode<T>
// with length/tail discovery and one .Next rewiring at the new cut point.
public sealed partial class RotateListTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, new[] { 4, 5, 1, 2, 3 })]
    [InlineData(new[] { 0, 1, 2 }, 4, new[] { 2, 0, 1 })]
    public void RotateRight_LeetCodeExamples_RotatesList(int[] values, int k, int[] expected)
        => Assert.Equal(expected, ToArray(RotateRight(BuildList(values), k)));

    private static SinglyLinkedListNode<int>? RotateRight(SinglyLinkedListNode<int>? head, int k)
    {
        if (head is null || head.Next is null || k == 0) return head;
        var length = 1;
        var tail = head;
        while (tail.Next is not null) { tail = tail.Next; length++; }
        var shift = k % length;
        if (shift == 0) return head;
        var stepsToNewTail = length - shift - 1;
        var newTail = head;
        for (var i = 0; i < stepsToNewTail; i++) newTail = newTail.Next!;
        var newHead = newTail.Next;
        newTail.Next = null;
        tail.Next = head;
        return newHead;
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0); var tail = dummy;
        foreach (var value in values) { tail.Next = new SinglyLinkedListNode<int>(value); tail = tail.Next; }
        return dummy.Next;
    }

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();
        for (var node = head; node is not null; node = node.Next) values.Add(node.Value);
        return values.ToArray();
    }
}
