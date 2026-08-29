using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicatesFromSortedListII;

public sealed partial class RemoveDuplicatesFromSortedListIITests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 3, 4, 4, 5 }, new[] { 1, 2, 5 })]
    [InlineData(new[] { 1, 1, 1, 2, 3 }, new[] { 2, 3 })]
    public void DeleteDuplicates_RemovesAllDuplicateRuns(int[] values, int[] expected)
        => Assert.Equal(expected, ToArray(DeleteDuplicates(Build(values))));

    private static SinglyLinkedListNode<int>? DeleteDuplicates(SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var previous = dummy;
        while (previous.Next is not null)
        {
            var current = previous.Next;
            var duplicated = false;
            while (current.Next is not null && current.Value == current.Next.Value)
            {
                duplicated = true;
                current = current.Next;
            }
            if (duplicated) previous.Next = current.Next; else previous = previous.Next;
        }
        return dummy.Next;
    }

    private static SinglyLinkedListNode<int>? Build(int[] values) { var d = new SinglyLinkedListNode<int>(0); var t = d; foreach (var v in values) { t.Next = new SinglyLinkedListNode<int>(v); t = t.Next; } return d.Next; }
    private static int[] ToArray(SinglyLinkedListNode<int>? head) { var values = new List<int>(); for (var n = head; n is not null; n = n.Next) values.Add(n.Value); return values.ToArray(); }
}
