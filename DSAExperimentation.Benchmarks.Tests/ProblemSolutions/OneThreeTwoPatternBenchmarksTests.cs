using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OneThreeTwoPatternBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one verdict - the full brute-force scan against the monotonic stack - so a harness
// whose arms disagree is timing two different problems. Setup makes the array strictly increasing
// (0..Length-1), which the class comment already pins as containing no 132 pattern at all, so every
// arm's answer is a known false: both arms are held to that fixed verdict as well as to each other.
public sealed partial class OneThreeTwoPatternBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().HasPatternByBruteForce(), BuildHarness().HasPatternByBruteForce());

    [Fact]
    public void HasPatternByBruteForce_IncreasingWorkload_ReportsNoPattern()
    {
        var harness = BuildHarness();

        Assert.False(harness.HasPatternByBruteForce());
        Assert.Equal(harness.HasPatternByMonotonicStack(), harness.HasPatternByBruteForce());
    }

    [Fact]
    public void HasPatternByMonotonicStack_IncreasingWorkload_ReportsNoPattern()
    {
        var harness = BuildHarness();

        Assert.False(harness.HasPatternByMonotonicStack());
        Assert.Equal(harness.HasPatternByBruteForce(), harness.HasPatternByMonotonicStack());
    }

    private static OneThreeTwoPatternBenchmarks BuildHarness()
    {
        var harness = new OneThreeTwoPatternBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
