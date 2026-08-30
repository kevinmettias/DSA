using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LinkedListRandomNode;

// LeetCode 382. Linked List Random Node: this repo's own SinglyLinkedListNode<int>
// walked once into a DynamicArray<int> cache, then GetRandom is a single O(1) indexed
// read - the same value -> DynamicArray<int> "cache once, GetRandom in O(1)" shape
// InsertDeleteGetRandomO1/InsertDeleteGetRandomO1DuplicatesAllowed already use for
// their own GetRandom().
public sealed partial class LinkedListRandomNodeTests
{
    [Fact]
    public void GetRandom_SingleNodeList_AlwaysReturnsThatValue()
    {
        var solution = new Solution(new SinglyLinkedListNode<int>(42));

        for (var i = 0; i < 20; i++)
        {
            Assert.Equal(42, solution.GetRandom());
        }
    }

    [Fact]
    public void GetRandom_MultiNodeList_EventuallyReturnsEveryValue()
    {
        var third = new SinglyLinkedListNode<int>(3);
        var second = new SinglyLinkedListNode<int>(2) { Next = third };
        var head = new SinglyLinkedListNode<int>(1) { Next = second };
        var solution = new Solution(head);

        var seen = new HashSet<int>();
        for (var i = 0; i < 200; i++)
        {
            var value = solution.GetRandom();
            Assert.True(value is 1 or 2 or 3);
            seen.Add(value);
        }

        Assert.Equal(3, seen.Count);
    }

    private sealed class Solution
    {
        private readonly DynamicArray<int> _values = new();
        private readonly Random _random = new(1);

        public Solution(SinglyLinkedListNode<int> head)
        {
            for (var node = head; node is not null; node = node.Next)
            {
                _values.Add(node.Value);
            }
        }

        public int GetRandom() => _values.Get(_random.Next(_values.Count));
    }
}
