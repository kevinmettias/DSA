using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthSmallestElementInABSTBenchmarks (ARCHITECTURE 17.9). Arm agreement and
// rebuild determinism are BenchmarkArmsTests' job; this pins what the generic check cannot know.
//
// The expected value is decisive and independent of either walk: Setup inserts the seeded shuffle
// of 1..NodeCount (all distinct) into a BinarySearchTree, so the in-order sequence is exactly
// 1..NodeCount and the targetRank = NodeCount / 2 th smallest value is NodeCount / 2 itself.
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

    // The hook arm counts its running rank in a hook it builds per call, so a second walk on one
    // harness reports the same rank only if nothing carries over from the first.
    [Fact]
    public void InOrderTraversalHooks_CalledTwiceOnOneHarness_ReportsTheSameRankBothTimes()
    {
        var harness = BuildHarness();
        var first = harness.InOrderTraversalHooks();
        var second = harness.InOrderTraversalHooks();

        Assert.Equal(ExpectedRankValue, first);
        Assert.Equal(first, second);
    }

    private static KthSmallestElementInABSTBenchmarks BuildHarness()
    {
        var harness = new KthSmallestElementInABSTBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
