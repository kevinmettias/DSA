using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BeautifulTowersIIBenchmarks (ARCHITECTURE 17.9): its two arms are BeautifulTowersIISolution's
// competing strategies for the same question - a per-peak clamped walk against one monotonic-stack sweep - so a
// harness whose arms disagree has optimized two different skylines. Same algorithm as Beautiful Towers I at II's
// much larger [Params] sizes, so the two classes are covered by parallel harnesses; both arms return the one long
// height total, compared directly, and the total is asserted positive since every drawn max height starts at 1.
// Setup draws that array from one fixed seed, so the same Length must rebuild the same heights.
public sealed partial class BeautifulTowersIIBenchmarksTests
{
    // The smaller of Setup's [Params(1_000, 8_000)] tower counts.
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_ThousandTowerSkyline_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.True(harness.BruteForce() > 0);
        Assert.Equal(harness.MonotonicStack(), harness.BruteForce());
    }

    [Fact]
    public void MonotonicStack_ThousandTowerSkyline_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.True(harness.MonotonicStack() > 0);
        Assert.Equal(harness.BruteForce(), harness.MonotonicStack());
    }

    private static BeautifulTowersIIBenchmarks BuildHarness()
    {
        var harness = new BeautifulTowersIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
