using DSAExperimentation.DataStructures.DoublyLinkedList;

namespace DSAExperimentation.Tests.DataStructures.DoublyLinkedList;

public sealed partial class DoublyLinkedListTests
{
    [Fact]
    public void AddFront_MultipleValues_PopBackReturnsValuesInFifoOrder()
    {
        var list = new DoublyLinkedList<int>();
        list.AddFront(new DoublyLinkedListNode<int> { Value = 1 });
        list.AddFront(new DoublyLinkedListNode<int> { Value = 2 });
        list.AddFront(new DoublyLinkedListNode<int> { Value = 3 });

        Assert.Equal(1, list.PopBack().Value);
        Assert.Equal(2, list.PopBack().Value);
        Assert.Equal(3, list.PopBack().Value);
    }

    [Fact]
    public void AddFront_IncreasesCount()
    {
        var list = new DoublyLinkedList<int>();

        list.AddFront(new DoublyLinkedListNode<int> { Value = 1 });
        list.AddFront(new DoublyLinkedListNode<int> { Value = 2 });

        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void PopBack_DecreasesCountAndReturnsRemovedNode()
    {
        var list = new DoublyLinkedList<int>();
        list.AddFront(new DoublyLinkedListNode<int> { Value = 1 });
        list.AddFront(new DoublyLinkedListNode<int> { Value = 2 });

        var popped = list.PopBack();

        Assert.Equal(1, popped.Value);
        Assert.Equal(1, list.Count);
    }

    [Fact]
    public void Remove_MiddleNode_SplicesRemainingNodesCorrectly()
    {
        var list = new DoublyLinkedList<int>();
        var first = new DoublyLinkedListNode<int> { Value = 1 };
        var middle = new DoublyLinkedListNode<int> { Value = 2 };
        var last = new DoublyLinkedListNode<int> { Value = 3 };
        list.AddFront(first);
        list.AddFront(middle);
        list.AddFront(last);

        list.Remove(middle);

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list.PopBack().Value);
        Assert.Equal(3, list.PopBack().Value);
    }

    [Fact]
    public void Remove_HeadNode_SplicesRemainingNodesCorrectly()
    {
        var list = new DoublyLinkedList<int>();
        var first = new DoublyLinkedListNode<int> { Value = 1 };
        var head = new DoublyLinkedListNode<int> { Value = 2 };
        list.AddFront(first);
        list.AddFront(head);

        list.Remove(head);

        Assert.Equal(1, list.Count);
        Assert.Equal(1, list.PopBack().Value);
    }

    [Fact]
    public void Remove_TailNode_SplicesRemainingNodesCorrectly()
    {
        var list = new DoublyLinkedList<int>();
        var tail = new DoublyLinkedListNode<int> { Value = 1 };
        var front = new DoublyLinkedListNode<int> { Value = 2 };
        list.AddFront(tail);
        list.AddFront(front);

        list.Remove(tail);

        Assert.Equal(1, list.Count);
        Assert.Equal(2, list.PopBack().Value);
    }
}
