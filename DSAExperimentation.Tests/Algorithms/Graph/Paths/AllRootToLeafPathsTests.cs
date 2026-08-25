using DSAExperimentation.Algorithms.Graph.Paths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Graph.Paths;

public sealed partial class AllRootToLeafPathsTests
{
    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Find_ReturnsEveryRootToLeafPath()
    {
        var root = TestTrees.NArySample();

        var paths = AllRootToLeafPaths.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root);

        var asNames = paths.Select(path => string.Join("", path.Select(n => n.Name))).ToArray();

        Assert.Equal(
            new[] { "ABE", "ABF", "AC", "ADG" },
            asNames);
    }

    [Fact]
    public void Find_SingleNode_ReturnsOnePathOfJustTheRoot()
    {
        var root = TestTrees.SingleNode();

        var paths = AllRootToLeafPaths.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root);

        Assert.Equal(new[] { "A" }, paths.Select(path => string.Join("", path.Select(n => n.Name))));
    }

    [Fact]
    public void Find_NullRoot_ReturnsNoPaths()
    {
        var paths = AllRootToLeafPaths.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(null);

        Assert.Empty(paths);
    }
}
