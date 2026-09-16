using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximizeSubarraysAfterRemovingOneConflictingPairBenchmarks (ARCHITECTURE 17.9):
// both arms are competing strategies for one question - the subarray count once the single best
// conflicting pair is removed - so a harness whose arms disagree is timing two different problems.
// Setup draws the conflicting pairs from a fixed seed, so the same value count must rebuild the same
// workload; neither arm mutates it.
public sealed partial class MaximizeSubarraysAfterRemovingOneConflictingPairBenchmarksTests
{
    private const int SmallestValueCount = 10;

    [Fact]
    public void Setup_SameValueCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_AgreesWithGroupedBoundSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.GroupedBoundSweep());
    }

    [Fact]
    public void GroupedBoundSweep_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GroupedBoundSweep(), harness.BruteForce());
    }

    private static MaximizeSubarraysAfterRemovingOneConflictingPairBenchmarks BuildHarness()
    {
        var harness = new MaximizeSubarraysAfterRemovingOneConflictingPairBenchmarks
        {
            ValueCount = SmallestValueCount,
        };

        harness.Setup();

        return harness;
    }
}
