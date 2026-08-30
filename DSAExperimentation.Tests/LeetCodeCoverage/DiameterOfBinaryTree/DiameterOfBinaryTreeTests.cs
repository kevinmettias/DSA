using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DiameterOfBinaryTree;

// LeetCode 543. Diameter of Binary Tree: the longest path between any two nodes,
// measured in edges, is exactly this repo's own TreeMetrics.Diameter -
// DiameterAlgebra already tracks, per node, the two tallest child heights (the best
// path THROUGH that node) alongside the best diameter seen in any subtree so far,
// via one TreeFold pass. No new primitive needed - same shape MaximumDepthOfBinary
// TreeTests already proves for TreeMetrics.Height.
public sealed partial class DiameterOfBinaryTreeTests
{
    [Fact]
    public void DiameterOfBinaryTree_ClassicExample_ReturnsLongestPathEdgeCount()
        => Assert.Equal(3, Diameter(new BinaryTreeNode<int>(1) { Left = new(2) { Left = new(4), Right = new(5) }, Right = new(3) }));

    [Fact]
    public void DiameterOfBinaryTree_TwoNodeTree_ReturnsOne()
        => Assert.Equal(1, Diameter(new BinaryTreeNode<int>(1) { Left = new(2) }));

    [Fact]
    public void DiameterOfBinaryTree_SingleNode_ReturnsZero()
        => Assert.Equal(0, Diameter(new BinaryTreeNode<int>(1)));

    private static int Diameter(BinaryTreeNode<int> root)
        => TreeMetrics.Diameter<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root);
}
