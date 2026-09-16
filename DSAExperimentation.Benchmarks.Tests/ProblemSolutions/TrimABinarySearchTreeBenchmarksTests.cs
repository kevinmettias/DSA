using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TrimABinarySearchTreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - collecting the in-range values and rebuilding a tree
// from them against reattaching the nodes that are already there - so a harness whose arms disagree
// is timing two different problems.
//
// Both arms return only the surviving node COUNT, because BinaryTreeNode<int> is internal and a
// public [Benchmark] method cannot return it (CS0050): the count is a projection, so agreement here
// witnesses "both kept the same number of nodes", not that they kept the same nodes. The class
// comment is what makes the expected count decisive - low/high span the tree's whole value range,
// so no node is out of range and every one of Setup's inserted nodes survives under both
// approaches - so the count is asserted against that quantity rather than only against the other
// arm.
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
        Assert.Equal(BuildHarness().CollectAndRebuild(), BuildHarness().CollectAndRebuild());
        Assert.Equal(ExpectedSurvivingNodeCount, BuildHarness().InPlaceTrim());
    }

    [Fact]
    public void CollectAndRebuild_SmallestNodeCount_AgreesWithInPlaceTrim()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InPlaceTrim(), harness.CollectAndRebuild());
    }

    [Fact]
    public void InPlaceTrim_SmallestNodeCount_AgreesWithCollectAndRebuild()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CollectAndRebuild(), harness.InPlaceTrim());
    }

    private static TrimABinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new TrimABinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
