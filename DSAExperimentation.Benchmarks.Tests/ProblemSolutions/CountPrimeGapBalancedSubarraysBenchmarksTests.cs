using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountPrimeGapBalancedSubarraysBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - re-tracking the window's running prime extremes
// against the compact-prime sliding window over this repo's Deque - so a harness whose arms disagree
// is timing two different problems. Setup seeds nums, so the same Length must rebuild the array.
public sealed partial class CountPrimeGapBalancedSubarraysBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every counted subarray is one index pair, so the answer is bounded by how many the array holds.
    private const int MostSubarrays = SmallestLength * (SmallestLength + 1) / 2;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: values span [1, 5_000), so the array holds many primes and pairs of
        // them land within the 500-wide gap bound - and no subarray can be counted twice.
        Assert.InRange(first.BruteForce(), 1L, MostSubarrays);
        Assert.Equal(first.BruteForce(), second.BruteForce());
    }

    [Fact]
    public void BruteForce_PrimeDenseValues_AgreesWithPrimeWindowDeque()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrimeWindowDeque(), harness.BruteForce());
    }

    [Fact]
    public void PrimeWindowDeque_PrimeDenseValues_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.PrimeWindowDeque());
    }

    private static CountPrimeGapBalancedSubarraysBenchmarks BuildHarness()
    {
        var harness = new CountPrimeGapBalancedSubarraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
