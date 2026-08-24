using DSAExperimentation.Graph.Algorithms.Metrics;
using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Graph.Engines.Dags.Trees;
using DSAExperimentation.Graph.Engines.Folding;
using DSAExperimentation.Graph.Engines.Reducing;
using DSAExperimentation.Tests.Engines.Fixtures;
using DSAExperimentation.Tests.Fixtures;

namespace DSAExperimentation.Tests.Engines;

public sealed partial class ZipTests
{
    [Fact]
    public void Fold_Zip_ComputesBothAlgebrasInOnePass()
    {
        var root = TestTrees.NArySample();

        var (count, height) = TreeFold.Fold<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            ZipFoldAlgebra<TestNode, int, int, CountNodesFoldAlgebra, HeightAlgebra<TestNode>>,
            (int, int)>(root);

        Assert.Equal(7, count);
        Assert.Equal(3, height);
    }

    [Fact]
    public void Reduce_Zip_ComputesBothAlgebrasInOnePass()
    {
        var root = TestTrees.NArySample();

        var (count, path) = Reduce.Tree<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            DepthFirstReduceOrder<TestNode>,
            ZipReduceAlgebra<TestNode, int, string, CountNodesReduceAlgebra, PathReduceAlgebra>,
            (int, string)>(root);

        Assert.Equal(7, count);
        Assert.Equal("ABEFCDG", path);
    }
}
