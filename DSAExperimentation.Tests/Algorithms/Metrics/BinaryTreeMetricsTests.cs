using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Metrics;

// Proves BinaryTreeTopology/BinaryTreeChildren correctly close TreeMetrics' generics
// - same precedent as MetricsTests.cs's own TestTopology-based cases, against
// BinaryTreeTrees.Sample():
//       4
//      / \
//     2   6
//    / \   \
//   1   3   7
// Heights: 1=3=7=1, 2=6=2, 4=3. Diameter through 4 = height(2)+height(6) = 2+2 = 4
// (path 1-2-4-6-7).
public sealed partial class BinaryTreeMetricsTests
{
    [Fact]
    public void Size_ComputesCorrectSize()
    {
        var root = BinaryTreeTrees.Sample();

        Assert.Equal(
            6,
            TreeMetrics.Size<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root));
    }

    [Fact]
    public void Height_ComputesCorrectHeight()
    {
        var root = BinaryTreeTrees.Sample();

        Assert.Equal(
            3,
            TreeMetrics.Height<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root));
    }

    [Fact]
    public void Diameter_ComputesCorrectDiameter()
    {
        var root = BinaryTreeTrees.Sample();

        Assert.Equal(
            4,
            TreeMetrics.Diameter<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root));
    }

    [Fact]
    public void HeightAndSize_ComputesBothInOnePass()
    {
        var root = BinaryTreeTrees.Sample();

        var (height, size) = TreeMetrics.HeightAndSize<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root);

        Assert.Equal(3, height);
        Assert.Equal(6, size);
    }

    [Fact]
    public void SingleNode_HeightOneSizeOneDiameterZero()
    {
        var root = BinaryTreeTrees.SingleNode();

        Assert.Equal(
            1,
            TreeMetrics.Size<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root));
        Assert.Equal(
            1,
            TreeMetrics.Height<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root));
        Assert.Equal(
            0,
            TreeMetrics.Diameter<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root));
    }

    [Fact]
    public void Metrics_NullRoot_ReturnZero()
    {
        Assert.Equal(
            0,
            TreeMetrics.Size<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(null));
        Assert.Equal(
            0,
            TreeMetrics.Height<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(null));
        Assert.Equal(
            0,
            TreeMetrics.Diameter<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(null));
    }
}
