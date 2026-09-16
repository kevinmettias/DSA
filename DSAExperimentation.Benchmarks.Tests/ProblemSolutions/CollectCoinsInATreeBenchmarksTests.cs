using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CollectCoinsInATreeBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - rescanning every surviving node each round until a full pass
// removes nothing against a one-pass queue of zero-coin leaves - so a harness whose arms disagree is
// timing two different problems. Setup builds both the tree and the coin tosses from one fixed seed,
// so the same NodeCount must rebuild the same tree and the same coins; otherwise two published
// numbers were never comparable in the first place.
//
// The adjacency list and the coin array are both private, and the trimmed edge count is the only
// thing either arm reports, so the workload's documented shape is asserted through that: a tree on
// NodeCount vertices has NodeCount-1 edges, and no trim can ever remove more than all of them.
public sealed partial class CollectCoinsInATreeBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameTreeAndCoins()
    {
        Assert.InRange(BuildHarness().RescanUntilFixedPoint(), 0, SmallestNodeCount - 1);
        Assert.Equal(BuildHarness().RescanUntilFixedPoint(), BuildHarness().RescanUntilFixedPoint());
    }

    [Fact]
    public void LeafQueue_RandomTreeWithSparseCoins_AgreesWithRescanUntilFixedPoint()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RescanUntilFixedPoint(), harness.LeafQueue());
    }

    [Fact]
    public void RescanUntilFixedPoint_RandomTreeWithSparseCoins_AgreesWithLeafQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LeafQueue(), harness.RescanUntilFixedPoint());
    }

    private static CollectCoinsInATreeBenchmarks BuildHarness()
    {
        var harness = new CollectCoinsInATreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
