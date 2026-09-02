using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SameTree;

public sealed partial class SameTreeTests
{
    [Fact]
    public void IsSameTree_IdenticalTrees_ReturnsTrue()
    {
        var same = IsSame(Tree(), Tree());
        Assert.True(same);
    }

    [Fact]
    public void IsSameTree_DifferentShape_ReturnsFalse()
    {
        var same = IsSame(new BinaryTreeNode<int>(1) { Left = new(2) }, new BinaryTreeNode<int>(1) { Right = new(2) });
        Assert.False(same);
    }

    private static bool IsSame(BinaryTreeNode<int>? p, BinaryTreeNode<int>? q) => p is null || q is null ? p is null && q is null : p.Value == q.Value && IsSame(p.Left, q.Left) && IsSame(p.Right, q.Right);
    private static BinaryTreeNode<int> Tree() => new(1) { Left = new(2), Right = new(3) };
}
