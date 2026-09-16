using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TrappingRainWaterBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the O(n^2) per-bar left/right rescan against the O(n) sweep
// using this repo's own Stack<int> as a monotonic stack - so a harness whose arms disagree is
// timing two different problems. Both arms return the trapped volume as an int, so they are
// compared directly. Setup draws the heights from a fixed seed, so the same Length must rebuild the
// same elevation map.
public sealed partial class TrappingRainWaterBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameElevationMap() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStack(), harness.BruteForce());
    }

    [Fact]
    public void MonotonicStack_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.MonotonicStack());
    }

    private static TrappingRainWaterBenchmarks BuildHarness()
    {
        var harness = new TrappingRainWaterBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
