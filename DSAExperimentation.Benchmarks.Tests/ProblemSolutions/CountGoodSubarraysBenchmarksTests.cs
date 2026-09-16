using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountGoodSubarraysBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the O(n^2) start/end scan against the run-length-compressed
// running-OR groups - so a harness whose arms disagree is timing two different problems. Both arms
// return a long, so they are compared directly. Setup draws from one fixed seed, so the same Length
// must rebuild the same array, and its documented shape is that the OR groups really do grow across
// several bit widths: every single-element subarray has OR == max == its own value, which the
// problem's predicate admits, so no admissible answer can fall below the number of starting points.
public sealed partial class CountGoodSubarraysBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

        Assert.True(BuildHarness().RunningOrGroups() >= SmallestLength);
    }

    [Fact]
    public void BruteForce_TwoHundredValues_AgreesWithRunningOrGroups()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RunningOrGroups(), harness.BruteForce());
    }

    [Fact]
    public void RunningOrGroups_TwoHundredValues_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.RunningOrGroups());
    }

    private static CountGoodSubarraysBenchmarks BuildHarness()
    {
        var harness = new CountGoodSubarraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
