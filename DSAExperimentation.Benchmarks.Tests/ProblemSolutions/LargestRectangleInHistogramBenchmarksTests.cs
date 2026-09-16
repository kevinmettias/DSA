using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LargestRectangleInHistogramBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - per-bar left/right expansion against one monotonic
// stack sweep - so a harness whose arms disagree is timing two different problems. Setup draws the
// bar heights from one fixed seed, so the same Length must rebuild the same histogram; otherwise two
// published numbers were never comparable in the first place.
public sealed partial class LargestRectangleInHistogramBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameHistogram() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_RandomHeights_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStack(), harness.BruteForce());
    }

    [Fact]
    public void MonotonicStack_RandomHeights_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.MonotonicStack());
    }

    private static LargestRectangleInHistogramBenchmarks BuildHarness()
    {
        var harness = new LargestRectangleInHistogramBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
