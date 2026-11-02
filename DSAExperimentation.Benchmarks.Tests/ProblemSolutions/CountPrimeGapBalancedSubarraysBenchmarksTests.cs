using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountPrimeGapBalancedSubarraysBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing, and to answering the same on a rebuilt
// harness and when called again on one, so this asserts what it cannot: a range for the answer
// that comes from the workload's shape rather than from either arm.
public sealed partial class CountPrimeGapBalancedSubarraysBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every counted subarray is one index pair, so the answer is bounded by how many the array holds.
    private const int MostSubarrays = SmallestLength * (SmallestLength + 1) / 2;

    // The documented shape: values span [1, 5_000), so the array holds many primes and pairs of
    // them land within the 500-wide gap bound - and no subarray can be counted twice.
    [Fact]
    public void BruteForce_PrimeRichValues_CountsWithinTheSubarrays() =>
        Assert.InRange(BuildHarness().BruteForce(), 1L, MostSubarrays);

    private static CountPrimeGapBalancedSubarraysBenchmarks BuildHarness()
    {
        var harness = new CountPrimeGapBalancedSubarraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
