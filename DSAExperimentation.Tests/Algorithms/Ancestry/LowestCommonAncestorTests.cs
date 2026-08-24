using DSAExperimentation.Graph.Algorithms.Ancestry;
using DSAExperimentation.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Ancestry;

public sealed partial class LowestCommonAncestorTests
{
    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Find_NodesInDifferentSubtrees_ReturnsSharedAncestor()
    {
        var root = TestTrees.NArySample();
        var e = root.Children[0].Children[0]; // B -> E
        var g = root.Children[2].Children[0]; // D -> G

        var lca = LowestCommonAncestor.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root, e, g);

        Assert.Same(root, lca);
    }

    [Fact]
    public void Find_OneNodeIsAncestorOfTheOther_ReturnsTheAncestor()
    {
        var root = TestTrees.NArySample();
        var b = root.Children[0];
        var f = b.Children[1];

        var lca = LowestCommonAncestor.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root, b, f);

        Assert.Same(b, lca);
    }

    [Fact]
    public void Find_SiblingsUnderSameParent_ReturnsTheParent()
    {
        var root = TestTrees.NArySample();
        var b = root.Children[0];
        var e = b.Children[0];
        var f = b.Children[1];

        var lca = LowestCommonAncestor.Find<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>>(root, e, f);

        Assert.Same(b, lca);
    }
}
