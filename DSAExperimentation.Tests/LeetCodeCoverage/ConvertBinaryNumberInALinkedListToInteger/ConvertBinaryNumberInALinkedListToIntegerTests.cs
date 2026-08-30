using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConvertBinaryNumberInALinkedListToInteger;

// LeetCode 1290. Convert Binary Number in a Linked List to Integer: a single
// left-to-right walk over this repo's own SinglyLinkedListNode<int>.Next,
// accumulating value = (value << 1) | bit as it goes - no algorithm primitive
// needed beyond the node representation itself, the same "just the
// representation" shape MiddleOfTheLinkedListTests' walk already uses.
public sealed partial class ConvertBinaryNumberInALinkedListToIntegerTests
{
    [Fact]
    public void GetDecimalValue_ClassicExample_ReturnsFive()
        => Assert.Equal(5, GetDecimalValue(Build([1, 0, 1])));

    [Fact]
    public void GetDecimalValue_SingleZeroNode_ReturnsZero()
        => Assert.Equal(0, GetDecimalValue(Build([0])));

    [Fact]
    public void GetDecimalValue_AllOnes_ReturnsFullBitPattern()
        => Assert.Equal(31, GetDecimalValue(Build([1, 1, 1, 1, 1])));

    private static int GetDecimalValue(SinglyLinkedListNode<int>? head)
    {
        var value = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            value = (value << 1) | node.Value;
        }

        return value;
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
