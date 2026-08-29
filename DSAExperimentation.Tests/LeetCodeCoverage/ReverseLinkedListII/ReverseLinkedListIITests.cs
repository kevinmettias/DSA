using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseLinkedListII;

public sealed partial class ReverseLinkedListIITests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 2, 4, new[] { 1, 4, 3, 2, 5 })]
    [InlineData(new[] { 5 }, 1, 1, new[] { 5 })]
    public void ReverseBetween_LeetCodeExamples_ReversesClosedRange(int[] values, int left, int right, int[] expected)
        => Assert.Equal(expected, ToArray(ReverseBetween(Build(values), left, right)));

    private static SinglyLinkedListNode<int>? ReverseBetween(SinglyLinkedListNode<int>? head, int left, int right)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var before = dummy;
        for (var i = 1; i < left; i++) before = before.Next!;
        var current = before.Next;
        for (var i = 0; i < right - left; i++) { var moved = current!.Next!; current.Next = moved.Next; moved.Next = before.Next; before.Next = moved; }
        return dummy.Next;
    }

    private static SinglyLinkedListNode<int>? Build(int[] values) { var d = new SinglyLinkedListNode<int>(0); var t = d; foreach (var v in values) { t.Next = new SinglyLinkedListNode<int>(v); t = t.Next; } return d.Next; }
    private static int[] ToArray(SinglyLinkedListNode<int>? head) { var values = new List<int>(); for (var n = head; n is not null; n = n.Next) values.Add(n.Value); return values.ToArray(); }
}
