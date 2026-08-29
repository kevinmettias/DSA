using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumDepthOfBinaryTree;

public sealed partial class MinimumDepthOfBinaryTreeTests
{
    [Fact]
    public void MinDepth_RightSkewedExample_ReturnsShortestLeafDepth()
        => Assert.Equal(2, MinDepth(new BinaryTreeNode<int>(1) { Left = new(2), Right = new(3) { Right = new(4) } }));

    private static int MinDepth(BinaryTreeNode<int>? node)
    {
        if (node is null) return 0;
        if (node.Left is null) return 1 + MinDepth(node.Right);
        if (node.Right is null) return 1 + MinDepth(node.Left);
        return 1 + Math.Min(MinDepth(node.Left), MinDepth(node.Right));
    }
}
