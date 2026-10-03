using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BalanceABinarySearchTreeBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin: that the rebuilt tree is as short as any tree of its node count can be, a bound known without consulting
// either arm. Setup's input is a right-only chain of the ascending values 1..n, which the midpoint split turns
// into a minimum-height BST. Both arms return the rebuilt root as object? (the node type is internal, CS0050).
public sealed partial class BalanceABinarySearchTreeBenchmarksTests
{
    // The smaller of Setup's [Params(200, 2_000)] node counts.
    private const int SmallestNodeCount = 200;

    // A binary tree gives every node this many children, so one more level holds this many times the
    // nodes below it - which is what makes a tree of height h hold at most 2^h - 1 nodes.
    private const int BinaryBranchingFactor = 2;

    [Fact]
    public void RepeatedKthSmallestScan_TwoHundredNodeSkewedTree_RebuildsAMinimumHeightTree() =>
        Assert.Equal(MinimumBalancedHeight(SmallestNodeCount), HeightOf(BuildHarness().RepeatedKthSmallestScan()));

    [Fact]
    public void InOrderTraversalCollectAndRebuild_TwoHundredNodeSkewedTree_RebuildsAMinimumHeightTree() =>
        Assert.Equal(MinimumBalancedHeight(SmallestNodeCount), HeightOf(BuildHarness().InOrderTraversalCollectAndRebuild()));

    private static int HeightOf(object? answer) =>
        TreeMetrics.Height<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>>(
            Assert.IsType<BinaryTreeNode<int>>(answer));

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
            capacity = (capacity * BinaryBranchingFactor) + 1;
        }

        return height;
    }
}
