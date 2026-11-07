using DSAExperimentation.DataStructures.DoublyLinkedList;

namespace DSAExperimentation.Tests.DataStructures.DoublyLinkedList;

// What the doubly linked configuration adds to the one linked node: its Previous link, which must
// keep its own value when Next is written and the other way round. Value and Next are the node
// template's, run over this configuration by LinkedNodeTests.
public sealed partial class DoublyLinkedListNodeTests
{
    [Fact]
    public void Previous_NewNode_IsNull() =>
        Assert.Null(new DoublyLinkedListNode<int>().Previous);

    [Fact]
    public void Previous_SetToAnotherNode_ReturnsThatNode()
    {
        var node = new DoublyLinkedListNode<int>();
        var previous = new DoublyLinkedListNode<int>();

        node.Previous = previous;

        Assert.Same(previous, node.Previous);
    }

    [Fact]
    public void Previous_SetAfterNext_LeavesNextAlone()
    {
        var node = new DoublyLinkedListNode<int>();
        var (previous, next) = (new DoublyLinkedListNode<int>(), new DoublyLinkedListNode<int>());

        node.Next = next;
        node.Previous = previous;

        Assert.Equal((previous, next), (node.Previous, node.Next));
    }

    [Fact]
    public void Previous_NextSetAfterIt_KeepsItsNode()
    {
        var node = new DoublyLinkedListNode<int>();
        var (previous, next) = (new DoublyLinkedListNode<int>(), new DoublyLinkedListNode<int>());

        node.Previous = previous;
        node.Next = next;

        Assert.Equal((previous, next), (node.Previous, node.Next));
    }
}
