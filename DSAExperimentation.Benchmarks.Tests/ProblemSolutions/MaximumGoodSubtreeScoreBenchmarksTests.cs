using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumGoodSubtreeScoreBenchmarks (ARCHITECTURE 17.9): both arms are
// MaximumGoodSubtreeScoreSolution's competing strategies for one question - the exponential
// per-subtree brute force against one bitmask fold that reuses each node's own mask - so a harness
// whose arms disagree is timing two different problems. Both answer with a single score, so the
// two returns are compared directly rather than through a rendering.
public sealed partial class MaximumGoodSubtreeScoreBenchmarksTests
{
    private const int SmallestNodeCount = 12;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameTreeAndValues()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The values and parent array are private, so the rebuild is pinned through the score they
        // produce: the same NodeCount must draw the same seeded workload from GoodSubtreeWorkloads
        // and fold it to the same score.
        Assert.Equal(first.BruteForce(), second.BruteForce());
        Assert.Equal(first.BitmaskTreeFold(), second.BitmaskTreeFold());
    }

    [Fact]
    public void BruteForce_SeededWorkload_AgreesWithBitmaskTreeFold()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitmaskTreeFold(), harness.BruteForce());
    }

    [Fact]
    public void BitmaskTreeFold_SeededWorkload_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.BitmaskTreeFold());
    }

    private static MaximumGoodSubtreeScoreBenchmarks BuildHarness()
    {
        var harness = new MaximumGoodSubtreeScoreBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
