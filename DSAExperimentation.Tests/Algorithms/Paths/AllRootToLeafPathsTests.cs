using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Paths;

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

    private static List<TestNode[]> Find(TestNode root)
        => AllRootToLeafPaths.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root);

    // CollectPathsHooks is private to AllRootToLeafPaths, so its two hooks are driven
    // through Find: Descend builds each child's path from its parent's, and Visit decides
    // which of those paths reach the output.
    public sealed partial class CollectPathsHooksTests
    {
        // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
        [Fact]
        public void Visit_RecordsAPathOnlyWhenItsNodeIsALeaf()
        {
            var paths = Find(TestTrees.NArySample());

            Assert.Equal(new[] { "E", "F", "C", "G" }, paths.Select(path => path[^1].Name));
            Assert.All(paths, path => Assert.Empty(path[^1].Children));
        }

        [Fact]
        public void Descend_ExtendsTheParentsPathByExactlyTheChild()
        {
            var paths = Find(TestTrees.NArySample());

            Assert.All(paths, path =>
            {
                Assert.Equal("A", path[0].Name);
                Assert.All(path.Zip(path.Skip(1)), step => Assert.Contains(step.Second, step.First.Children));
            });
        }

        [Fact]
        public void Descend_GivesSiblingsSeparateCopiesOfTheSharedPrefix()
        {
            // E and F both descend from A -> B. Had Descend extended one shared buffer,
            // F's descent would have overwritten the path already recorded for E.
            var paths = Find(TestTrees.NArySample());

            Assert.NotSame(paths[0], paths[1]);
            Assert.Equal(new[] { "A", "B", "E" }, paths[0].Select(n => n.Name));
            Assert.Equal(new[] { "A", "B", "F" }, paths[1].Select(n => n.Name));
        }
    }
}
