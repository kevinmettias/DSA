using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed class BinaryTreeChildrenTests
{
    private static BinaryTreeNode<int> Node(int value, BinaryTreeNode<int>? left = null, BinaryTreeNode<int>? right = null) =>
        new(value) { Left = left, Right = right };

    [Fact]
    public void Count_Leaf_IsZero()
    {
        Assert.Equal(0, new BinaryTreeChildren<int>(Node(1)).Count);
    }

    [Fact]
    public void Count_OnlyALeftChild_IsOne()
    {
        Assert.Equal(1, new BinaryTreeChildren<int>(Node(1, left: Node(2))).Count);
    }

    [Fact]
    public void Count_OnlyARightChild_IsOne()
    {
        Assert.Equal(1, new BinaryTreeChildren<int>(Node(1, right: Node(3))).Count);
    }

    [Fact]
    public void Count_BothChildren_IsTwo()
    {
        Assert.Equal(2, new BinaryTreeChildren<int>(Node(1, Node(2), Node(3))).Count);
    }

    [Fact]
    public void Get_BothChildren_YieldsLeftThenRight()
    {
        var children = new BinaryTreeChildren<int>(Node(1, Node(2), Node(3)));

        Assert.Equal(2, children.Get(0).Value);
        Assert.Equal(3, children.Get(1).Value);
    }

    [Fact]
    public void Get_OnlyARightChild_PutsItAtIndexZero()
    {
        // A missing left child compacts the index space rather than leaving a hole.
        var children = new BinaryTreeChildren<int>(Node(1, right: Node(3)));

        Assert.Equal(3, children.Get(0).Value);
    }

    [Fact]
    public void Get_PastTheLastPresentChild_Throws()
    {
        var children = new BinaryTreeChildren<int>(Node(1, left: Node(2)));

        Assert.Throws<IndexOutOfRangeException>(() => children.Get(1));
    }

    [Fact]
    public void Get_Leaf_Throws()
    {
        Assert.Throws<IndexOutOfRangeException>(() => new BinaryTreeChildren<int>(Node(1)).Get(0));
    }
}
