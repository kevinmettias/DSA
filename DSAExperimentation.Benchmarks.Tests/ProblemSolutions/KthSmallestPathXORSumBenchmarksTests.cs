using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthSmallestPathXORSumBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - the k-th smallest XOR sum along the root-to-node path
// for each query - so a harness whose arms disagree is timing two different problems. Each arm
// answers with the whole per-query result array, whose order is pinned by the query batch, so the
// outer order is compared as well as the values. The chain, the values and the query batch are
// all seeded, so the same NodeCount must rebuild the same workload.
public sealed partial class KthSmallestPathXORSumBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameChainAndQueries() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().PerQueryWalk()),
            AnswerText.Of(BuildHarness().PerQueryWalk()));

    [Fact]
    public void PerQueryWalk_RepeatedNodeChain_AgreesWithEulerTourCache()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.EulerTourCache()), AnswerText.Of(harness.PerQueryWalk()));
    }

    [Fact]
    public void EulerTourCache_RepeatedNodeChain_AgreesWithPerQueryWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.PerQueryWalk()), AnswerText.Of(harness.EulerTourCache()));
    }

    private static KthSmallestPathXORSumBenchmarks BuildHarness()
    {
        var harness = new KthSmallestPathXORSumBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
