using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountOfRangeSumBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing, and to answering the same on a rebuilt harness and when called again
// on one, so this asserts what it cannot: a range for the answer that comes from the workload's
// shape rather than from either arm.
public sealed partial class CountOfRangeSumBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every subarray is one index pair, so the answer is bounded by how many pairs the array holds.
    private const int MostIndexPairs = SmallestLength * (SmallestLength + 1) / 2;

    // The documented shape: values come from [-100, 100) while the queried range is [-1000,
    // 1000], so every one-element subarray lands inside it and the answer is at least the
    // array's own length - and never more than its number of subarrays.
    [Fact]
    public void PairwisePrefixScan_RangeWiderThanAnyValue_CountsEveryOneElementSubarray() =>
        Assert.InRange(BuildHarness().PairwisePrefixScan(), SmallestLength, MostIndexPairs);

    private static CountOfRangeSumBenchmarks BuildHarness()
    {
        var harness = new CountOfRangeSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
