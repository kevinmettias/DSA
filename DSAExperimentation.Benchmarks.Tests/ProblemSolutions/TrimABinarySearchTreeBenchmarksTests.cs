using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TrimABinarySearchTreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - collecting the in-range values and rebuilding a tree
// from them against reattaching the nodes that are already there - so a harness whose arms disagree
// is timing two different problems.
//
// Both arms return the trimmed tree's root. The class comment is what makes the expected tree
// decisive - low/high span the tree's whole value range, so no node is out of range and every one of
// Setup's inserted nodes survives under both approaches - and LC 669 forbids a trim from changing
// the relative structure of the nodes it keeps, so the answer must be Setup's own tree, unchanged.
// Each arm is asserted against that tree, rebuilt here the way Setup builds it.
//
// Setup inserts 0..NodeCount-1 into a BinarySearchTree<int>, so the same NodeCount must rebuild the
// same tree. TrimByInPlaceMutation mutates the shared tree in place, but with the whole range in
// bounds every recursive call returns its own node unchanged, so a single harness can be called by
// either arm in either order.
public sealed partial class TrimABinarySearchTreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    // Every inserted value lies in [0, NodeCount - 1], which is exactly the low/high the arms trim
    // with, so no node is dropped.
    private const int ExpectedSurvivingNodeCount = SmallestNodeCount;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameTree()
    {
        Assert.Equal(AnswerGraphText.Of(BuildHarness().CollectAndRebuild()), AnswerGraphText.Of(BuildHarness().CollectAndRebuild()));
        Assert.Equal(ExpectedSurvivingNodeCount, CountNodes(BuildHarness().InPlaceTrim()));
    }

    [Fact]
    public void CollectAndRebuild_WholeValueRange_KeepsSetupsTreeUnchanged() =>
        Assert.Equal(SetupTree(), AnswerGraphText.Of(BuildHarness().CollectAndRebuild()));

    [Fact]
    public void InPlaceTrim_WholeValueRange_KeepsSetupsTreeUnchanged() =>
        Assert.Equal(SetupTree(), AnswerGraphText.Of(BuildHarness().InPlaceTrim()));

    // [GlobalSetup]'s tree, restated: 0..NodeCount-1 inserted in ascending order.
    private static string SetupTree()
    {
        var tree = new BinarySearchTree<int>();

        for (var value = 0; value < SmallestNodeCount; value++)
        {
            tree.Insert(value);
        }

        return AnswerGraphText.Of(tree.Root);
    }

    private static int CountNodes(object? root) => CountSubtree((BinaryTreeNode<int>?)root);

    private static int CountSubtree(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + CountSubtree(node.Left) + CountSubtree(node.Right);

    private static TrimABinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new TrimABinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
