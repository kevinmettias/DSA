using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubarraysWithCostLessThanOrEqualToKBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - extending every start index against the
// two monotonic deques - so a harness whose arms disagree is timing two different problems. Setup
// seeds nums, so the same Length must rebuild the same array.
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

    [Fact]
    public void BruteForce_EverySingleElementWithinBudget_AgreesWithMonotonicDeques()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicDeques(), harness.BruteForce());
    }

    [Fact]
    public void MonotonicDeques_EverySingleElementWithinBudget_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.MonotonicDeques());
    }

    private static CountSubarraysWithCostLessThanOrEqualToKBenchmarks BuildHarness()
    {
        var harness = new CountSubarraysWithCostLessThanOrEqualToKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
