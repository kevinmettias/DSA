using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubarraysWithCostLessThanOrEqualToKBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a fresh harness, so this asserts what it
// cannot: a range for the answer that comes from the workload's shape rather than from either arm,
// and that a harness which has already answered once answers as a rebuild does, since
// BenchmarkDotNet calls an arm many times on one harness while the generic check builds a fresh
// harness for every call. Setup seeds nums, so the same Length must rebuild the same array.
public sealed partial class CountSubarraysWithCostLessThanOrEqualToKBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every counted subarray is one index pair, so the answer is bounded by how many the array holds.
    private const long MostSubarrays = (long)SmallestLength * (SmallestLength + 1) / 2;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: a one-element window has max - min = 0 against a 5_000 cost limit, so
        // every single element qualifies and the answer is at least the array's own length.
        Assert.InRange(first.BruteForce(), (long)SmallestLength, MostSubarrays);
        Assert.Equal(first.BruteForce(), second.BruteForce());
    }

    private static CountSubarraysWithCostLessThanOrEqualToKBenchmarks BuildHarness()
    {
        var harness = new CountSubarraysWithCostLessThanOrEqualToKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
