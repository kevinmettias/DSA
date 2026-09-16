using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed class BinaryTreeTopologyTests
{
    [Fact]
    public void GetChildren_ExposesBothChildrenLeftFirst()
    {
        var root = new BinaryTreeNode<int>(1) { Left = new(2), Right = new(3) };

        var children = BinaryTreeTopology<int>.GetChildren(root);

        AssertChildrenAre(children, 2, 3);
    }

    [Fact]
    public void GetChildren_Leaf_ReturnsNone() => Assert.Equal(0, BinaryTreeTopology<int>.GetChildren(new BinaryTreeNode<int>(1)).Count);

    // Left first and then right, and exactly two of them: the order GetChildren promises,
    // with the count confirming no third one was invented.
    private static void AssertChildrenAre(BinaryTreeChildren<int> children, int left, int right)
    {
        Assert.Equal(2, children.Count);
        Assert.Equal(left, children.Get(0).Value);
        Assert.Equal(right, children.Get(1).Value);
    }
}
