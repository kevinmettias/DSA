using DSAExperimentation.DataStructures.Graph.Adjacency;

namespace DSAExperimentation.Tests.DataStructures.Graph.Adjacency;

public sealed partial class AdjacencyTopologyTests
{
    [Fact]
    public void GetChildren_ExposesTheNodesOwnNeighborsInOrder()
    {
        var (source, first, second) = (new AdjacencyNode(0), new AdjacencyNode(1), new AdjacencyNode(2));
        source.Neighbors.Add(first);
        source.Neighbors.Add(second);

        var children = AdjacencyTopology.GetChildren(source);

        Assert.Equal([first, second], Enumerable.Range(0, children.Count).Select(children.Get));
    }

    [Fact]
    public void GetChildren_NodeWithNoOutEdges_ReturnsNoChildren() =>
        Assert.Equal(0, AdjacencyTopology.GetChildren(new AdjacencyNode(0)).Count);

    // The graph tier promises nothing about cycles, so an edge back to the node itself is a child
    // like any other: the topology reports it and leaves the defence to the engine's visit guard.
    [Fact]
    public void GetChildren_EdgeBackToItself_ReportsTheNodeAsItsOwnChild()
    {
        var node = new AdjacencyNode(0);
        node.Neighbors.Add(node);

        Assert.Same(node, AdjacencyTopology.GetChildren(node).Get(0));
    }
}
