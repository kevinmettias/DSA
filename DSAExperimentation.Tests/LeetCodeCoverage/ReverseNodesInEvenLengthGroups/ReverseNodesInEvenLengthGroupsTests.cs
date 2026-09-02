using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReverseNodesInEvenLengthGroups;

// LeetCode 2074. Reverse Nodes in Even Length Groups: pointer reversal over
// SinglyLinkedListNode<T>, the same ReverseNodesInKGroup shape except the group
// size grows by one after every group (1, 2, 3, ...) instead of staying fixed, the
// last group may be shorter than its expected size, and whether a group reverses
// depends on its actual (possibly truncated) length's parity, not a fixed k.
public sealed partial class ReverseNodesInEvenLengthGroupsTests
{
    [Theory]
    [InlineData(new[] { 5, 2, 6, 3, 9, 1, 7, 3, 8, 4 }, new[] { 5, 6, 2, 3, 9, 1, 4, 8, 3, 7 })]
    [InlineData(new[] { 1, 1, 0, 6 }, new[] { 1, 0, 1, 6 })]
    [InlineData(new[] { 1, 1, 0, 6, 5 }, new[] { 1, 0, 1, 5, 6 })]
    public void ReverseEvenLengthGroups_LeetCodeExamples_ReversesOnlyEvenActualLengthGroups(
        int[] values, int[] expected)
    {
        var list = BuildList(values);
        var reversed = ReverseEvenLengthGroups(list);
        var actual = ToArray(reversed);
        Assert.Equal(expected, actual);
    }

    // Groups start at expected size 1 (always odd, so the head node is never
    // reversed against anything) and grow by one each time; groupPrevious walks
    // forward one group at a time the same way ReverseNodesInKGroup's does.
    private static SinglyLinkedListNode<int>? ReverseEvenLengthGroups(SinglyLinkedListNode<int>? head)
    {
        var groupPrevious = head;
        var groupSize = 2;

        while (groupPrevious?.Next is not null)
        {
            var (tail, length) = MeasureGroup(groupPrevious, groupSize);

            groupPrevious = length % 2 == 0 ? ReverseGroup(groupPrevious, tail) : tail;
            groupSize++;
        }

        return head;
    }

    // Walks up to groupSize nodes past groupPrevious, stopping early at the list's
    // end - this truncated actual length, not the expected groupSize, is what
    // decides whether the group reverses.
    private static (SinglyLinkedListNode<int> Tail, int Length) MeasureGroup(
        SinglyLinkedListNode<int> groupPrevious, int groupSize)
    {
        var node = groupPrevious;
        var length = 0;

        while (length < groupSize && node.Next is not null)
        {
            node = node.Next;
            length++;
        }

        return (node, length);
    }

    private static SinglyLinkedListNode<int> ReverseGroup(
        SinglyLinkedListNode<int> groupPrevious, SinglyLinkedListNode<int> tail)
    {
        var groupNext = tail.Next;
        var oldGroupHead = groupPrevious.Next!;
        SinglyLinkedListNode<int>? previous = groupNext;
        var current = oldGroupHead;

        while (current != groupNext)
        {
            var next = current!.Next;
            current.Next = previous;
            previous = current;
            current = next;
        }

        groupPrevious.Next = tail;
        return oldGroupHead;
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
