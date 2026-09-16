using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthSmallestElementInABSTBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - the target-rank-th smallest value in the tree - so a
// harness whose arms disagree is timing two different problems.
//
// The expected value is decisive and independent of either walk: Setup inserts the seeded shuffle
// of 1..NodeCount (all distinct) into a BinarySearchTree, so the in-order sequence is exactly
// 1..NodeCount and the targetRank = NodeCount / 2 th smallest value is NodeCount / 2 itself.
//
// Each arm resets its own state at the top of its call - the recursive walk resets its remaining
// counter and result field, the hook arm resets the shared AsyncLocal state - so neither carries
// anything across calls and one harness instance is safe to call twice in either order.
public sealed partial class KthSmallestElementInABSTBenchmarksTests
{
    private const int SmallestNodeCount = 500;
    private const int ExpectedRankValue = SmallestNodeCount / AlgorithmConstants.HalvingFactor;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameRankedTree()
    {
        Assert.Equal(ExpectedRankValue, BuildHarness().RecursiveInOrderWalk());
        Assert.Equal(ExpectedRankValue, BuildHarness().InOrderTraversalHooks());
    }

    [Fact]
    public void RecursiveInOrderWalk_MedianRankOnDistinctValues_AgreesWithInOrderTraversalHooks()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InOrderTraversalHooks(), harness.RecursiveInOrderWalk());
    }

    [Fact]
    public void InOrderTraversalHooks_MedianRankOnDistinctValues_AgreesWithRecursiveInOrderWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveInOrderWalk(), harness.InOrderTraversalHooks());
    }

    // Visit is the private recursion the recursive arm walks with, so the only thing a harness
    // test can observe about it is the rank it reports: this is the same two-arm agreement
    // expressed at the level of the walk itself rather than of the arm that calls it.
    [Fact]
    public void Visit_MedianRankOnDistinctValues_RanksTheSameNodeAsTheHookDrivenWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InOrderTraversalHooks(), harness.RecursiveInOrderWalk());
    }

    private static KthSmallestElementInABSTBenchmarks BuildHarness()
    {
        var harness = new KthSmallestElementInABSTBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
