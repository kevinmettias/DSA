using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BalanceABinarySearchTreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// BalanceABinarySearchTreeSolution's competing strategies for the same question - re-deriving the k-th smallest for
// every rank against one in-order visit - so a harness whose arms disagree has collected two different value
// orders. Both then rebuild by the identical midpoint split, so the height each reports is the whole observable,
// and that height is also asserted against the minimum any tree of this node count admits: Setup's input is a
// right-only chain of the ascending values 0..n-1, which the midpoint split turns into a minimum-height BST.
public sealed partial class BalanceABinarySearchTreeBenchmarksTests
{
    // The smaller of Setup's [Params(200, 2_000)] node counts.
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RepeatedKthSmallestScan(), BuildHarness().RepeatedKthSmallestScan());

    [Fact]
    public void RepeatedKthSmallestScan_TwoHundredNodeSkewedTree_AgreesWithInOrderTraversalCollectAndRebuild()
    {
        var harness = BuildHarness();

        Assert.Equal(MinimumBalancedHeight(SmallestNodeCount), harness.RepeatedKthSmallestScan());
        Assert.Equal(harness.InOrderTraversalCollectAndRebuild(), harness.RepeatedKthSmallestScan());
    }

    [Fact]
    public void InOrderTraversalCollectAndRebuild_TwoHundredNodeSkewedTree_AgreesWithRepeatedKthSmallestScan()
    {
        var harness = BuildHarness();

        Assert.Equal(MinimumBalancedHeight(SmallestNodeCount), harness.InOrderTraversalCollectAndRebuild());
        Assert.Equal(harness.RepeatedKthSmallestScan(), harness.InOrderTraversalCollectAndRebuild());
    }

    private static BalanceABinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new BalanceABinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // The shortest height any binary tree of this many nodes can have: a tree of height h holds at
    // most 2^h - 1 nodes.
    private static int MinimumBalancedHeight(int nodeCount)
    {
        var height = 0;
        var capacity = 0;

        while (capacity < nodeCount)
        {
            height++;
            capacity = (capacity * 2) + 1;
        }

        return height;
    }
}
