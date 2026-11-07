using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.DataStructures.SinglyLinkedList;

// What the random-pointer configuration adds to the one linked node: its Random link. Value and
// Next are the node template's, run over this configuration by LinkedNodeTests.
public sealed partial class RandomLinkedListNodeTests
{
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

    [Fact]
    public void Random_SetAfterNext_LeavesNextAlone()
    {
        var node = new RandomLinkedListNode<int>(1);
        var (next, target) = (new RandomLinkedListNode<int>(2), new RandomLinkedListNode<int>(3));

        node.Next = next;
        node.Random = target;

        Assert.Equal((next, target), (node.Next, node.Random));
    }
}
