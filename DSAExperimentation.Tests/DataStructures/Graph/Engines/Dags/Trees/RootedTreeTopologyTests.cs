using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed partial class RootedTreeTopologyTests
{
    [Fact]
    public void GetChildren_ExposesTheNodesOwnChildrenInOrder()
    {
        var nodes = ParentArrayTree.Build([-1, 0, 0]);

        var children = RootedTreeTopology.GetChildren(nodes[0]);

        Assert.Equal(2, children.Count);
        Assert.Equal([1, 2], Enumerable.Range(0, children.Count).Select(i => children.Get(i).Id));
    }

    [Fact]
    public void GetChildren_Leaf_ReturnsNoChildren()
    {
        var nodes = ParentArrayTree.Build([-1, 0]);

        Assert.Equal(0, RootedTreeTopology.GetChildren(nodes[1]).Count);
    }
}
