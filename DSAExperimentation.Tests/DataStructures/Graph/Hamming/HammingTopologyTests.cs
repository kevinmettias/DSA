using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Tests.DataStructures.Graph.Hamming;

public sealed class HammingTopologyTests
{
    [Fact]
    public void GetChildren_ExposesTheNodesOwnNeighbours()
    {
        var graph = HammingGraph.Build("aa", ["ab", "ba"]);

        Assert.True(graph.TryGetNode("aa", out var node));

        var children = HammingTopology.GetChildren(node);

        Assert.Equal(node.Neighbors.Count, children.Count);
        Assert.Equal(["ab", "ba"], Enumerable.Range(0, children.Count)
            .Select(i => children.Get(i).Value)
            .OrderBy(v => v, StringComparer.Ordinal));
    }

    [Fact]
    public void GetChildren_IsolatedNode_ReturnsNoChildren()
    {
        var graph = HammingGraph.Build("aa", ["zz"]);

        Assert.True(graph.TryGetNode("aa", out var node));
        Assert.Equal(0, HammingTopology.GetChildren(node).Count);
    }
}
