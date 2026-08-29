using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumDepthOfBinaryTree;

public sealed partial class MaximumDepthOfBinaryTreeTests
{
    [Fact]
    public void MaxDepth_ClassicExample_ReturnsHeight()
        => Assert.Equal(3, TreeMetrics.Height<BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>, NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(Tree()));

    private static BinaryTreeNode<int> Tree() => new(3) { Left = new(9), Right = new(20) { Left = new(15), Right = new(7) } };
}
