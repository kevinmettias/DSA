using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumPointsAfterCollectingCoinsFromAllNodesBenchmarks (ARCHITECTURE 17.9):
// both arms are MaximumPointsAfterCollectingCoinsFromAllNodesSolution's competing strategies for one
// question - the dictionary-memoized recursion against the flat coin-points algebra fold - so a
// harness whose arms disagree is timing two different problems. Both answer with a single point
// total, compared directly.
public sealed partial class MaximumPointsAfterCollectingCoinsFromAllNodesBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameChainTreeAndCoins()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The edge list and the per-node coins are private, so the rebuild is pinned through the
        // total they produce: the same NodeCount must build the same chain and draw the same
        // seeded coins over it.
        Assert.Equal(first.MemoizedRecursion(), second.MemoizedRecursion());
        Assert.Equal(first.TreeFold(), second.TreeFold());
    }

    [Fact]
    public void MemoizedRecursion_SeededChainTreeAndCoins_AgreesWithTreeFold()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TreeFold(), harness.MemoizedRecursion());
    }

    [Fact]
    public void TreeFold_SeededChainTreeAndCoins_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.TreeFold());
    }

    private static MaximumPointsAfterCollectingCoinsFromAllNodesBenchmarks BuildHarness()
    {
        var harness = new MaximumPointsAfterCollectingCoinsFromAllNodesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
