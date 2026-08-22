using DSAExperimentation.Trees;

namespace DSAExperimentation.Tests;

public sealed class FoldTests
{
    [Fact]
    public void DepthFirstFold_CountsNodes()
    {
        var root = TestTrees.NArySample();

        var count = TreeFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            CountNodesFoldAlgebra,
            int>(root);

        Assert.Equal(7, count);
    }

    [Fact]
    public void DepthFirstFold_NullRoot_ReturnsEmpty()
    {
        var count = TreeFold.Fold<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            CountNodesFoldAlgebra,
            int>(null);

        Assert.Equal(0, count);
    }

    [Fact]
    public void BreadthFirstReduce_CountsNodes()
    {
        var root = TestTrees.NArySample();

        var count = BreadthFirstReduce.Reduce<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            CountNodesBreadthFirstAlgebra,
            int>(root);

        Assert.Equal(7, count);
    }

    [Fact]
    public void BreadthFirstReduce_NullRoot_ReturnsSeed()
    {
        var count = BreadthFirstReduce.Reduce<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            CountNodesBreadthFirstAlgebra,
            int>(null);

        Assert.Equal(0, count);
    }
}
