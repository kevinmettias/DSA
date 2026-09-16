using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for LinkedListRandomNodeWorkloads (ARCHITECTURE 17.7). The reading depends on one
// chain of the requested length, nothing more - everything about LC 382's strategies lives in the
// LeetCode project, so the only thing this generator owes is the chain's shape.
public sealed partial class LinkedListRandomNodeWorkloadsTests
{
    private const int Length = 64;
    private const int FirstPosition = 0;

    [Fact]
    public void Build_Length_ReturnsAChainOfExactlyThatManyNodes()
    {
        var nodes = Nodes(LinkedListRandomNodeWorkloads.Build(Length));

        Assert.Equal(Length, nodes.Count);
        Assert.Null(nodes[^1].Next);
    }

    [Fact]
    public void Build_EveryNode_HoldsItsOwnPositionAsItsValue()
    {
        var nodes = Nodes(LinkedListRandomNodeWorkloads.Build(Length));

        Assert.Equal(FirstPosition, nodes[0].Value);
        Assert.Equal(Enumerable.Range(0, Length), nodes.Select(node => node.Value));
    }

    private static List<SinglyLinkedListNode<int>> Nodes(SinglyLinkedListNode<int> head)
    {
        var nodes = new List<SinglyLinkedListNode<int>>();

        for (SinglyLinkedListNode<int>? node = head; node is not null; node = node.Next)
        {
            nodes.Add(node);
        }

        return nodes;
    }
}
