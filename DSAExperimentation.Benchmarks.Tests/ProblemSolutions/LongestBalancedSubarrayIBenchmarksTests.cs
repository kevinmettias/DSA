using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestBalancedSubarrayIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning each candidate window's distinct-value sets
// against one forward scan keyed on the two set sizes - so a harness whose arms disagree is timing two
// different problems. Setup builds the array arithmetically from the length, so the same Length must
// rebuild the same numbers; otherwise two published numbers were never comparable in the first place.
public sealed partial class LongestBalancedSubarrayIBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameNumbers() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_AlternatingEvenOddValues_AgreesWithDistinctSetScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DistinctSetScan(), harness.BruteForce());
    }

    [Fact]
    public void DistinctSetScan_AlternatingEvenOddValues_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.DistinctSetScan());
    }

    private static LongestBalancedSubarrayIBenchmarks BuildHarness()
    {
        var harness = new LongestBalancedSubarrayIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
