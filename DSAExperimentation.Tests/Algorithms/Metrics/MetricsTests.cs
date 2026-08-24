using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Engines.Dags.Trees;
using DSAExperimentation.Tests.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Metrics;

public sealed partial class MetricsTests
{
    // A -> [B, C, D], B -> [E, F], D -> [G]
    // Heights: E=F=G=C=1, B=D=2, A=3.
    // Diameter through A = tallest two children (B, D) = 2 + 2 = 4
    // (path E-B-A-D-G).

    [Fact]
    public void Size_ComputesCorrectSize()
    {
        var root = TestTrees.NArySample();

        Assert.Equal(7, TreeMetrics.Size<TestNode, TestTopology, ListChildren<TestNode>, NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root));
    }

    [Fact]
    public void Height_ComputesCorrectHeight()
    {
        var root = TestTrees.NArySample();

        Assert.Equal(3, TreeMetrics.Height<TestNode, TestTopology, ListChildren<TestNode>, NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root));
    }

    [Fact]
    public void Diameter_ComputesCorrectDiameter()
    {
        var root = TestTrees.NArySample();

        Assert.Equal(4, TreeMetrics.Diameter<TestNode, TestTopology, ListChildren<TestNode>, NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root));
    }

    [Fact]
    public void HeightAndSize_ComputesBothInOnePass()
    {
        var root = TestTrees.NArySample();

        var (height, size) = TreeMetrics.HeightAndSize<TestNode, TestTopology, ListChildren<TestNode>, NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root);

        Assert.Equal(3, height);
        Assert.Equal(7, size);
    }

    [Fact]
    public void SingleNode_HeightOneSizeOneDiameterZero()
    {
        var root = TestTrees.SingleNode();

        Assert.Equal(1, TreeMetrics.Size<TestNode, TestTopology, ListChildren<TestNode>, NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root));
        Assert.Equal(1, TreeMetrics.Height<TestNode, TestTopology, ListChildren<TestNode>, NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root));
        Assert.Equal(0, TreeMetrics.Diameter<TestNode, TestTopology, ListChildren<TestNode>, NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root));
    }

    [Fact]
    public void Metrics_NullRoot_ReturnZero()
    {
        Assert.Equal(0, TreeMetrics.Size<TestNode, TestTopology, ListChildren<TestNode>, NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(null));
        Assert.Equal(0, TreeMetrics.Height<TestNode, TestTopology, ListChildren<TestNode>, NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(null));
        Assert.Equal(0, TreeMetrics.Diameter<TestNode, TestTopology, ListChildren<TestNode>, NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(null));
    }
}
