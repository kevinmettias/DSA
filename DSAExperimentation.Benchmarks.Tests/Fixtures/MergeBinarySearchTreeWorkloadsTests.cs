using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MergeBinarySearchTreeWorkloads (ARCHITECTURE 17.7). The reading depends on the
// forest being a strictly descending left-leaning chain once merged - every tree but the last hangs the
// next tree's root as its left leaf - so BST order holds and the only thing treeCount changes is how
// much work the root-matching lookup has to do.
public sealed partial class MergeBinarySearchTreeWorkloadsTests
{
    private const int TreeCount = 8;
    private const int LowestRootValue = 1;

    [Fact]
    public void BuildChain_TreeCount_ReturnsOneTreePerRequestedTree() =>
        Assert.Equal(TreeCount, MergeBinarySearchTreeWorkloads.BuildChain(TreeCount).Count);

    [Fact]
    public void BuildChain_EveryRoot_DescendsByOneAcrossTheForest()
    {
        var forest = MergeBinarySearchTreeWorkloads.BuildChain(TreeCount);

        Assert.Equal(
            Enumerable.Range(LowestRootValue, TreeCount).Reverse(),
            forest.Select(tree => tree.Value));
    }

    // The link that makes the merge work: each tree's left leaf is the next tree's root, which is the
    // value the merge's "find the tree whose root matches this leaf" lookup is looking for.
    [Fact]
    public void BuildChain_EveryTreeButTheLast_HangsTheNextTreesRootAsItsLeftLeaf()
    {
        var forest = MergeBinarySearchTreeWorkloads.BuildChain(TreeCount);

        foreach (var index in Enumerable.Range(0, TreeCount - 1))
        {
            var leaf = forest[index].Left;
            Assert.NotNull(leaf);

            Assert.Equal(forest[index + 1].Value, leaf.Value);
        }

        Assert.Null(forest[^1].Left);
    }

    [Fact]
    public void BuildChain_EveryTree_LeansLeftOnly()
    {
        var forest = MergeBinarySearchTreeWorkloads.BuildChain(TreeCount);

        Assert.All(forest, tree => Assert.Null(tree.Right));
    }
}
