using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionList;

public sealed partial class PartitionListTests
{
    [Fact]
    public void Partition_ClassicExample_PreservesRelativeOrderWithinSides()
        => Assert.Equal([1, 2, 2, 4, 3, 5], ToArray(Partition(Build([1, 4, 3, 2, 5, 2]), 3)));

    private static SinglyLinkedListNode<int>? Partition(SinglyLinkedListNode<int>? head, int x)
    {
        var before = new SinglyLinkedListNode<int>(0); var beforeTail = before;
        var after = new SinglyLinkedListNode<int>(0); var afterTail = after;
        for (var node = head; node is not null;)
        {
            var next = node.Next; node.Next = null;
            if (node.Value < x) { beforeTail.Next = node; beforeTail = node; } else { afterTail.Next = node; afterTail = node; }
            node = next;
        }
        beforeTail.Next = after.Next;
        return before.Next;
    }

    private static SinglyLinkedListNode<int>? Build(int[] values) { var d = new SinglyLinkedListNode<int>(0); var t = d; foreach (var v in values) { t.Next = new SinglyLinkedListNode<int>(v); t = t.Next; } return d.Next; }
    private static int[] ToArray(SinglyLinkedListNode<int>? head) { var values = new List<int>(); for (var n = head; n is not null; n = n.Next) values.Add(n.Value); return values.ToArray(); }
}
