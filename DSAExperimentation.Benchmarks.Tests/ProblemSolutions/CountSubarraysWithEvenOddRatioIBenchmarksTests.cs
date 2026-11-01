using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubarraysWithEvenOddRatioIBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a fresh harness, so this asserts what it
// cannot: a range for the answer that comes from the workload's shape rather than from either arm,
// and that a harness which has already answered once answers as a rebuild does, since
// BenchmarkDotNet calls an arm many times on one harness while the generic check builds a fresh
// harness for every call. Setup seeds nums, so the same Length must rebuild the same array.
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

    private static CountSubarraysWithEvenOddRatioIBenchmarks BuildHarness()
    {
        var harness = new CountSubarraysWithEvenOddRatioIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
