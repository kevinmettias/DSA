using DSAExperimentation.Trees;

namespace DSAExperimentation.Tests;

public sealed class MetricsTests
{
    [Fact]
    public void Height_ComputesCorrectHeight()
    {
        var root = TestTrees.BinarySample();

        var height = BinaryTreeMetrics.Height<
            TestNode,
            TestBinaryTopology,
            LeftFirstBinaryOrder>(root);

        Assert.Equal(3, height);
    }

    [Fact]
    public void Size_ComputesCorrectSize()
    {
        var root = TestTrees.BinarySample();

        var size = BinaryTreeMetrics.Size<
            TestNode,
            TestBinaryTopology,
            LeftFirstBinaryOrder>(root);

        Assert.Equal(5, size);
    }

    [Fact]
    public void Diameter_ComputesCorrectDiameter()
    {
        var root = TestTrees.BinarySample();

        var diameter = BinaryTreeMetrics.Diameter<
            TestNode,
            TestBinaryTopology,
            LeftFirstBinaryOrder>(root);

        Assert.Equal(3, diameter);
    }

    [Fact]
    public void HeightAndSize_ZipsBothResults()
    {
        var root = TestTrees.BinarySample();

        var result = BinaryTreeMetrics.HeightAndSize<
            TestNode,
            TestBinaryTopology,
            LeftFirstBinaryOrder>(root);

        Assert.Equal(3, result.First);
        Assert.Equal(5, result.Second);
    }

    [Fact]
    public void Metrics_NullRoot_ReturnZero()
    {
        Assert.Equal(0, BinaryTreeMetrics.Height<TestNode, TestBinaryTopology, LeftFirstBinaryOrder>(null));
        Assert.Equal(0, BinaryTreeMetrics.Size<TestNode, TestBinaryTopology, LeftFirstBinaryOrder>(null));
        Assert.Equal(0, BinaryTreeMetrics.Diameter<TestNode, TestBinaryTopology, LeftFirstBinaryOrder>(null));
    }
}
