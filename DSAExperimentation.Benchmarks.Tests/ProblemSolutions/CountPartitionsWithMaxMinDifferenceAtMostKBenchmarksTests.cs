using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountPartitionsWithMaxMinDifferenceAtMostKBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - a backward rescan per position against
// the two-pointer sliding window - so a harness whose arms disagree is timing two different
// problems. Setup draws nums from a fixed seed through MaxMinPartitionWorkloads, so the same Length
// must rebuild the same array.
public sealed partial class CountPartitionsWithMaxMinDifferenceAtMostKBenchmarksTests
{
    private const int SmallestLength = 500;

    // Mirrors the benchmark's own seed, so the workload asserted here is the one Setup builds.
    private const int NumsSeed = 3578;

    // The fixture's own value bound: values are drawn from [1, ValueBound).
    private const int MaxValue = 999;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var nums = MaxMinPartitionWorkloads.BuildNums(SmallestLength, NumsSeed);

        // The documented shape: the array's values stay well below the scenario's
        // MaxMinPartitionScenario.MaxMinDifference bound, so every window is valid and each
        // position's segment stretches back to the start of the array.
        Assert.Equal(SmallestLength, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, 1, MaxValue));
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
    }

    [Fact]
    public void BruteForce_EveryWindowValid_AgreesWithSlidingWindowDeque()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SlidingWindowDeque(), harness.BruteForce());
    }

    [Fact]
    public void SlidingWindowDeque_EveryWindowValid_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SlidingWindowDeque());
    }

    private static CountPartitionsWithMaxMinDifferenceAtMostKBenchmarks BuildHarness()
    {
        var harness = new CountPartitionsWithMaxMinDifferenceAtMostKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
