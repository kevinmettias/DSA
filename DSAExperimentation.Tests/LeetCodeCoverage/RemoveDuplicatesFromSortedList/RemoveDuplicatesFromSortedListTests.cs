using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveDuplicatesFromSortedList;

public sealed partial class RemoveDuplicatesFromSortedListTests
{
    [Theory]
    [InlineData(new[] { 1, 1, 2 }, new[] { 1, 2 })]
    [InlineData(new[] { 1, 1, 2, 3, 3 }, new[] { 1, 2, 3 })]
    public void DeleteDuplicates_CollapsesDuplicateRuns(int[] values, int[] expected)
        => Assert.Equal(expected, ToArray(DeleteDuplicates(Build(values))));

    private static SinglyLinkedListNode<int>? DeleteDuplicates(SinglyLinkedListNode<int>? head)
    {
        for (var node = head; node is not null; node = node.Next)
            while (node.Next is not null && node.Value == node.Next.Value) node.Next = node.Next.Next;
        return head;
    }

    private static SinglyLinkedListNode<int>? Build(int[] values) { var d = new SinglyLinkedListNode<int>(0); var t = d; foreach (var v in values) { t.Next = new SinglyLinkedListNode<int>(v); t = t.Next; } return d.Next; }
    private static int[] ToArray(SinglyLinkedListNode<int>? head) { var values = new List<int>(); for (var n = head; n is not null; n = n.Next) values.Add(n.Value); return values.ToArray(); }
}
