using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.DataStructures.SinglyLinkedList;

public sealed class RandomLinkedListNodeTests
{
    [Fact]
    public void Value_ConstructedWithValue_ReturnsThatValue() =>
        Assert.Equal(7, new RandomLinkedListNode<int>(7).Value);

    [Fact]
    public void Value_SetAfterConstruction_ReturnsTheNewValue()
    {
        var node = new RandomLinkedListNode<int>(1) { Value = 2 };

        Assert.Equal(2, node.Value);
    }

    [Fact]
    public void Next_NewNode_DefaultsToNull() =>
        Assert.Null(new RandomLinkedListNode<int>(1).Next);

    [Fact]
    public void Next_SetToAnotherNode_ReturnsThatNode()
    {
        var node = new RandomLinkedListNode<int>(1);
        var next = new RandomLinkedListNode<int>(2);

        node.Next = next;

        Assert.Same(next, node.Next);
    }

    [Fact]
    public void Random_NewNode_DefaultsToNull() =>
        Assert.Null(new RandomLinkedListNode<int>(1).Random);

    [Fact]
    public void Random_SetToAnotherNode_ReturnsThatNode()
    {
        var node = new RandomLinkedListNode<int>(1);
        var target = new RandomLinkedListNode<int>(2);

        node.Random = target;

        Assert.Same(target, node.Random);
    }

    [Fact]
    public void Random_SetToItself_ReturnsItself()
    {
        var node = new RandomLinkedListNode<int>(1)
        {
            Random = null,
        };

        node.Random = node;

        Assert.Same(node, node.Random);
    }
}
