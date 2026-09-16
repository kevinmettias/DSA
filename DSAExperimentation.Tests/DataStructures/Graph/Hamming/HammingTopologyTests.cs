using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Tests.DataStructures.Graph.Hamming;

public sealed class HammingTopologyTests
{
    [Fact]
    public void GetChildren_ExposesTheNodesOwnNeighbours()
    {
        var node = NodeIn("aa", ["ab", "ba"]);

        var children = HammingTopology.GetChildren(node);

        var childValues = Enumerable.Range(0, children.Count)
            .Select(i => children.Get(i).Value)
            .OrderBy(v => v, StringComparer.Ordinal);

        Assert.Equal(node.Neighbors.Count, children.Count);
        Assert.Equal(["ab", "ba"], childValues);
    }

    [Fact]
    public void GetChildren_IsolatedNode_ReturnsNoChildren()
    {
        var node = NodeIn("aa", ["zz"]);

        Assert.Equal(0, HammingTopology.GetChildren(node).Count);
    }

    // The arrangement both tests open with - build the graph, find the state in it,
    // and insist the state was reachable - named once so the node it yields threads
    // into every assertion below instead of being re-found in each body.
    private static HammingNode NodeIn(string state, string[] values)
    {
        var graph = HammingGraph.Build(state, values);

        var found = graph.TryGetNode(state, out var node);
        Assert.True(found);

        return node;
    }
}
