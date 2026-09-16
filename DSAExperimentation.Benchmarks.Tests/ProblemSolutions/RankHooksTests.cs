using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthSmallestElementInABSTBenchmarks's nested RankHooks (ARCHITECTURE 17.9).
//
// RankHooks is a private nested type, so no test class can name it in code. Coverage is attributed
// by TYPE, though, so the nested unit needs a class named after it, and the only surface that runs
// RankHooks.Visit is the outer benchmark's InOrderTraversalHooks arm
// (InOrderTraversal.Walk<int, RankHooks>). These tests therefore drive that arm and assert the rank
// Visit recorded.
//
// The expected value is decisive and independent of the walk: Setup inserts the seeded shuffle of
// 1..NodeCount (all distinct) into a BinarySearchTree, so the in-order sequence is exactly
// 1..NodeCount and the targetRank = NodeCount / HalvingFactor th smallest value is that rank
// itself. A Visit that skipped a node, decremented twice, or failed to record at zero would report
// a different rank instead of matching this constant.
public sealed partial class RankHooksTests
{
    private const int SmallestNodeCount = 500;
    private const int ExpectedVisitRank = SmallestNodeCount / AlgorithmConstants.HalvingFactor;

    [Fact]
    public void Visit_MedianRankOnDistinctValues_RecordsTheValueAtTheTargetRank() =>
        Assert.Equal(ExpectedVisitRank, BuildHarness().InOrderTraversalHooks());

    // RankHooks records through the outer benchmark's shared AsyncLocal state, so a second walk on
    // one harness only reports the same rank if the arm resets that state before walking. Without
    // the reset the remaining count is already spent and Visit would never reach zero again.
    [Fact]
    public void Visit_CalledTwiceOnOneHarness_RecordsTheSameRankBothTimes()
    {
        var harness = BuildHarness();
        var first = harness.InOrderTraversalHooks();
        var second = harness.InOrderTraversalHooks();

        Assert.Equal(ExpectedVisitRank, first);
        Assert.Equal(first, second);
    }

    private static KthSmallestElementInABSTBenchmarks BuildHarness()
    {
        var harness = new KthSmallestElementInABSTBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
