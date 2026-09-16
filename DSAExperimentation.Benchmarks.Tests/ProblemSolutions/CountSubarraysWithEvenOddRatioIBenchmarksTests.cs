using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubarraysWithEvenOddRatioIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - enumerating every subarray against the weighted
// prefix-sum Fenwick sweep - so a harness whose arms disagree is timing two different problems.
// Setup seeds nums, so the same Length must rebuild the same array.
public sealed partial class CountSubarraysWithEvenOddRatioIBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every counted subarray is one index pair, so the answer is bounded by how many the array holds.
    private const int MostSubarrays = SmallestLength * (SmallestLength + 1) / 2;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: with the ratio fixed at 1/1 a single odd element is already valid
        // (zero evens against one odd), so a two-hundred value array is never an empty scan.
        Assert.InRange(first.BruteForce(), 1, MostSubarrays);
        Assert.Equal(first.BruteForce(), second.BruteForce());
    }

    [Fact]
    public void BruteForce_HalfOfAllSubarraysQualify_AgreesWithFenwickPrefixSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickPrefixSweep(), harness.BruteForce());
    }

    [Fact]
    public void FenwickPrefixSweep_HalfOfAllSubarraysQualify_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.FenwickPrefixSweep());
    }

    private static CountSubarraysWithEvenOddRatioIBenchmarks BuildHarness()
    {
        var harness = new CountSubarraysWithEvenOddRatioIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
