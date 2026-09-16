using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumAndMinimumSumsOfAtMostSizeKSubarraysBenchmarks (ARCHITECTURE 17.9):
// both arms are competing strategies for one question - the summed maximum and minimum over every
// subarray of at most k elements - so a harness whose arms disagree is timing two different problems.
// Setup draws the array from a fixed seed, so the same length must rebuild the same workload; neither
// arm mutates it.
public sealed partial class MaximumAndMinimumSumsOfAtMostSizeKSubarraysBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceWindow(), BuildHarness().BruteForceWindow());

    [Fact]
    public void BruteForceWindow_AgreesWithMonotonicStackContribution()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceWindow(), harness.MonotonicStackContribution());
    }

    [Fact]
    public void MonotonicStackContribution_AgreesWithBruteForceWindow()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStackContribution(), harness.BruteForceWindow());
    }

    private static MaximumAndMinimumSumsOfAtMostSizeKSubarraysBenchmarks BuildHarness()
    {
        var harness = new MaximumAndMinimumSumsOfAtMostSizeKSubarraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
