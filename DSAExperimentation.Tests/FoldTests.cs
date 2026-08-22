using DSAExperimentation.Trees;

namespace DSAExperimentation.Tests;

public class FoldTests
{
    [Fact]
    public void DepthFirstFold_CountsNodes()
    {
        var root = TestTrees.NArySample();

        var count = DepthFirstFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            CountNodesDepthFirstAlgebra,
            int>(root);

        Assert.Equal(7, count);
    }

    [Fact]
    public void DepthFirstFold_NullRoot_ReturnsEmpty()
    {
        var count = DepthFirstFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            CountNodesDepthFirstAlgebra,
            int>(null);

        Assert.Equal(0, count);
    }

    [Fact]
    public void BreadthFirstFold_CountsNodes()
    {
        var root = TestTrees.NArySample();

        var count = BreadthFirstFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            CountNodesBreadthFirstAlgebra,
            int>(root);

        Assert.Equal(7, count);
    }

    [Fact]
    public void BreadthFirstFold_NullRoot_ReturnsSeed()
    {
        var count = BreadthFirstFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            CountNodesBreadthFirstAlgebra,
            int>(null);

        Assert.Equal(0, count);
    }
}
