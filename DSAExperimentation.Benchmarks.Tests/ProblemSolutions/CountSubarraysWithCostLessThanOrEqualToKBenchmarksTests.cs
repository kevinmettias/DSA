using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubarraysWithCostLessThanOrEqualToKBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing, and to answering the same on a rebuilt
// harness and when called again on one, so this asserts what it cannot: a range for the answer
// that comes from the workload's shape rather than from either arm.
public sealed partial class CountSubarraysWithCostLessThanOrEqualToKBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every counted subarray is one index pair, so the answer is bounded by how many the array holds.
    private const long MostSubarrays = (long)SmallestLength * (SmallestLength + 1) / 2;

    // The documented shape: a one-element window has max - min = 0 against a 5_000 cost limit, so
    // every single element qualifies and the answer is at least the array's own length.
    [Fact]
    public void BruteForce_CostLimitAboveAnySingleElement_CountsEveryOneElementWindow() =>
        Assert.InRange(BuildHarness().BruteForce(), (long)SmallestLength, MostSubarrays);

    private static CountSubarraysWithCostLessThanOrEqualToKBenchmarks BuildHarness()
    {
        var harness = new CountSubarraysWithCostLessThanOrEqualToKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
