using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BeautifulTowersIBenchmarks (ARCHITECTURE 17.9): its two arms are BeautifulTowersISolution's
// competing strategies for the same question - a per-peak clamped walk against one monotonic-stack sweep - so a
// harness whose arms disagree has optimized two different skylines. Both return the one long height total, so they
// are compared directly, and the total is asserted positive as well: Setup draws every max height from 1 upward,
// so a sum of zero could only mean an arm placed no tower. Setup draws that array from one fixed seed, so the same
// Length must rebuild the same heights.
public sealed partial class BeautifulTowersIBenchmarksTests
{
    // The smaller of Setup's [Params(200, 1_000)] tower counts.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_TwoHundredTowerSkyline_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.True(harness.BruteForce() > 0);
        Assert.Equal(harness.MonotonicStack(), harness.BruteForce());
    }

    [Fact]
    public void MonotonicStack_TwoHundredTowerSkyline_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.True(harness.MonotonicStack() > 0);
        Assert.Equal(harness.BruteForce(), harness.MonotonicStack());
    }

    private static BeautifulTowersIBenchmarks BuildHarness()
    {
        var harness = new BeautifulTowersIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
