using DSAExperimentation.DataStructures.Deque;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumTwinSumOfALinkedList;

// LeetCode 2130. Maximum Twin Sum of a Linked List: this repo's own Deque<int>
// (CircularBuffer-backed, ARCHITECTURE.md Sec.4.1) collects every value with one
// forward walk over the list, then drains matched front/back pairs together - node
// i and node (n-1-i) always arrive at the deque's two ends in lockstep, so no index
// arithmetic or list reversal is needed to find twin i's partner.
public sealed partial class MaximumTwinSumOfALinkedListTests
{
    [Theory]
    [InlineData(new[] { 5, 4, 2, 1 }, 6)]
    [InlineData(new[] { 4, 2, 2, 3 }, 7)]
    public void PairSum_LeetCodeExamples_ReturnsMaximumTwinSum(int[] values, int expected) =>
        Assert.Equal(expected, PairSum(Build(values)));

    private static int PairSum(SinglyLinkedListNode<int>? head)
    {
        var values = new Deque<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.PushBack(node.Value);
        }

        var best = 0;

        while (values.TryPopFront(out var front) && values.TryPopBack(out var back))
        {
            best = Math.Max(best, front + back);
        }

        return best;
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
}
