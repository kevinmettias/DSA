using DSAExperimentation.Trees;

namespace DSAExperimentation.Tests;

public sealed class FoldTraversalStrategyTests
{
    [Fact]
    public void TreeFold_BreadthFirstTraversal_MatchesDepthFirstTraversal()
    {
        var root = TestTrees.NArySample();

        var depthFirstHeight = TreeFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            DepthFirstFoldTraversal<TestNode>,
            HeightFoldAlgebra,
            int>(root);

        var breadthFirstHeight = TreeFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            BreadthFirstFoldTraversal<TestNode>,
            HeightFoldAlgebra,
            int>(root);

        Assert.Equal(3, depthFirstHeight);
        Assert.Equal(depthFirstHeight, breadthFirstHeight);
    }

    [Fact]
    public void TreeFold_BreadthFirstTraversal_NullRoot_ReturnsEmpty()
    {
        var count = TreeFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            BreadthFirstFoldTraversal<TestNode>,
            CountNodesFoldAlgebra,
            int>(null);

        Assert.Equal(0, count);
    }

    [Fact]
    public void BinaryTreeFold_BreadthFirstTraversal_MatchesDepthFirstTraversal_ForHeightAndSize()
    {
        var root = TestTrees.BinarySample();

        var depthFirstResult = BinaryTreeFold.Fold<
            TestNode,
            TestBinaryTopology,
            LeftFirstBinaryOrder,
            DepthFirstBinaryFoldTraversal<TestNode>,
            ZipBinaryFoldAlgebra<TestNode, int, int, HeightAlgebra<TestNode>, SizeAlgebra<TestNode>>,
            FoldResultPair<int, int>>(root);

        var breadthFirstResult = BinaryTreeFold.Fold<
            TestNode,
            TestBinaryTopology,
            LeftFirstBinaryOrder,
            BreadthFirstBinaryFoldTraversal<TestNode>,
            ZipBinaryFoldAlgebra<TestNode, int, int, HeightAlgebra<TestNode>, SizeAlgebra<TestNode>>,
            FoldResultPair<int, int>>(root);

        Assert.Equal(3, depthFirstResult.First);
        Assert.Equal(5, depthFirstResult.Second);
        Assert.Equal(depthFirstResult, breadthFirstResult);
    }

    [Fact]
    public void BinaryTreeFold_BreadthFirstTraversal_NullRoot_ReturnsEmpty()
    {
        var height = BinaryTreeFold.Fold<
            TestNode,
            TestBinaryTopology,
            LeftFirstBinaryOrder,
            BreadthFirstBinaryFoldTraversal<TestNode>,
            HeightAlgebra<TestNode>,
            int>(null);

        Assert.Equal(0, height);
    }
}
