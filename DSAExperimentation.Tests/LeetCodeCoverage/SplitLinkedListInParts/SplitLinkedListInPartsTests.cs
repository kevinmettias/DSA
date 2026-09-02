using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SplitLinkedListInParts;

// LeetCode 725. Split Linked List in Parts: splits the list into k consecutive
// parts as evenly as possible (earlier parts absorb the length % k remainder,
// trailing parts are null once nodes run out), composing only this repo's own
// SinglyLinkedListNode<TValue> - no new primitive needed.
public sealed partial class SplitLinkedListInPartsTests
{
    [Fact]
    public void SplitListToParts_FewerNodesThanParts_TrailingPartsAreNull()
    {
        var head = Build([1, 2, 3]);

        var parts = SplitListToParts(head, 5);

        Assert.Equal(5, parts.Length);
        Assert.Equal([1], ToArray(parts[0]));
        Assert.Equal([2], ToArray(parts[1]));
        Assert.Equal([3], ToArray(parts[2]));
        Assert.Null(parts[3]);
        Assert.Null(parts[4]);
    }

    [Fact]
    public void SplitListToParts_MoreNodesThanParts_EarlierPartsAbsorbRemainder()
    {
        var head = Build([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);

        var parts = SplitListToParts(head, 3);

        Assert.Equal(3, parts.Length);
        Assert.Equal([1, 2, 3, 4], ToArray(parts[0]));
        Assert.Equal([5, 6, 7], ToArray(parts[1]));
        Assert.Equal([8, 9, 10], ToArray(parts[2]));
    }

    private static SinglyLinkedListNode<int>?[] SplitListToParts(SinglyLinkedListNode<int>? head, int k)
    {
        var length = 0;
        for (var node = head; node is not null; node = node.Next)
        {
            length++;
        }

        var partSize = length / k;
        var extra = length % k;
        var parts = new SinglyLinkedListNode<int>?[k];
        var current = head;

        for (var i = 0; i < k && current is not null; i++)
        {
            var currentSize = partSize + (i < extra ? 1 : 0);
            current = AssignPart(parts, i, currentSize, current);
        }

        return parts;
    }

    private static SinglyLinkedListNode<int>? AssignPart(
        SinglyLinkedListNode<int>?[] parts, int i, int currentSize, SinglyLinkedListNode<int>? current)
    {
        parts[i] = current;

        for (var j = 1; j < currentSize; j++)
        {
            current = current!.Next;
        }

        var next = current!.Next;
        current.Next = null;
        return next;
    }

    private static SinglyLinkedListNode<int> Build(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next!;
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
