using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed class BinaryTreeTopologyTests
{
    [Fact]
    public void GetChildren_ExposesBothChildrenLeftFirst()
    {
        var root = new BinaryTreeNode<int>(1) { Left = new(2), Right = new(3) };

        var children = BinaryTreeTopology<int>.GetChildren(root);

        Assert.Equal(2, children.Count);
        Assert.Equal(2, children.Get(0).Value);
        Assert.Equal(3, children.Get(1).Value);
    }

    [Fact]
    public void GetChildren_Leaf_ReturnsNone()
    {
        Assert.Equal(0, BinaryTreeTopology<int>.GetChildren(new BinaryTreeNode<int>(1)).Count);
    }
}
