using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubarraysWithEvenOddRatioIBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing, and to answering the same on a rebuilt
// harness and when called again on one, so this asserts what it cannot: a range for the answer
// that comes from the workload's shape rather than from either arm.
public sealed partial class CountSubarraysWithEvenOddRatioIBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every counted subarray is one index pair, so the answer is bounded by how many the array holds.
    private const int MostSubarrays = SmallestLength * (SmallestLength + 1) / 2;

    // The documented shape: with the ratio fixed at 1/1 a single odd element is already valid
    // (zero evens against one odd), so a two-hundred value array is never an empty scan.
    [Fact]
    public void BruteForce_OneToOneRatio_CountsWithinTheSubarrays() =>
        Assert.InRange(BuildHarness().BruteForce(), 1, MostSubarrays);

    private static CountSubarraysWithEvenOddRatioIBenchmarks BuildHarness()
    {
        var harness = new CountSubarraysWithEvenOddRatioIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
