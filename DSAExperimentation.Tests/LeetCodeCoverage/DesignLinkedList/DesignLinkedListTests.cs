using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignLinkedList;

// LeetCode 707. Design Linked List: a singly-linked MyLinkedList built directly over
// this repo's own SinglyLinkedListNode<TValue> chain. A dummy head sentinel - the same
// "dummy-head list building" precedent AddTwoNumbers already uses - makes AddAtHead/
// AddAtIndex(0, _)/DeleteAtIndex(0) branch-free special cases instead of needing their
// own "is this the first node" checks; every other index just walks .Next from the
// sentinel to the node just before it.
public sealed partial class DesignLinkedListTests
{
    [Fact]
    public void Operations_LeetCodeExampleSequence_MatchesExpectedResults()
    {
        var list = new MyLinkedList();

        list.AddAtHead(1);
        list.AddAtTail(3);
        list.AddAtIndex(1, 2); // list: 1 -> 2 -> 3
        Assert.Equal(2, list.Get(1));

        list.DeleteAtIndex(1); // list: 1 -> 3
        Assert.Equal(3, list.Get(1));
    }

    [Fact]
    public void Get_IndexOutOfBounds_ReturnsNegativeOne()
    {
        var list = new MyLinkedList();
        list.AddAtHead(7);

        Assert.Equal(-1, list.Get(5));
        Assert.Equal(-1, list.Get(-1));
    }

    [Fact]
    public void AddAtIndex_EqualToLength_AppendsAtTail()
    {
        var list = new MyLinkedList();
        list.AddAtHead(1);

        list.AddAtIndex(1, 2);

        Assert.Equal(2, list.Get(1));
    }

    [Fact]
    public void AddAtIndex_GreaterThanLength_IsANoOp()
    {
        var list = new MyLinkedList();
        list.AddAtHead(1);

        list.AddAtIndex(5, 99);

        Assert.Equal(-1, list.Get(1));
    }

    [Fact]
    public void DeleteAtIndex_IndexOutOfBounds_IsANoOp()
    {
        var list = new MyLinkedList();
        list.AddAtHead(1);

        list.DeleteAtIndex(5);

        Assert.Equal(1, list.Get(0));
    }

    private sealed class MyLinkedList
    {
        private readonly SinglyLinkedListNode<int> _dummyHead = new(default);
        private int _count;

        public int Get(int index)
        {
            if (index < 0 || index >= _count)
            {
                return -1;
            }

            return NodeBefore(index).Next!.Value;
        }

        public void AddAtHead(int value) => AddAtIndex(0, value);

        public void AddAtTail(int value) => AddAtIndex(_count, value);

        public void AddAtIndex(int index, int value)
        {
            if (index > _count)
            {
                return;
            }

            var previous = NodeBefore(Math.Max(index, 0));
            var node = new SinglyLinkedListNode<int>(value) { Next = previous.Next };
            previous.Next = node;
            _count++;
        }

        public void DeleteAtIndex(int index)
        {
            if (index < 0 || index >= _count)
            {
                return;
            }

            var previous = NodeBefore(index);
            previous.Next = previous.Next!.Next;
            _count--;
        }

        private SinglyLinkedListNode<int> NodeBefore(int index)
        {
            var node = _dummyHead;
            for (var i = 0; i < index; i++)
            {
                node = node.Next!;
            }

            return node;
        }
    }
}
