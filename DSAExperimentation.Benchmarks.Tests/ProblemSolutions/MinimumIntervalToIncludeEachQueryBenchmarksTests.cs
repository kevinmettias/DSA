using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumIntervalToIncludeEachQueryBenchmarks (ARCHITECTURE 17.9): both
// arms are MinimumIntervalToIncludeEachQuerySolution's, the same methods
// MinimumIntervalToIncludeEachQueryTests proves correct, and both return one smallest-covering
// interval size per query. LeetCode fixes the answer's order - it is the query order - so the
// comparison here is order-sensitive, and arms that disagree are timing two different problems.
//
// Setup's workload is seeded, so the same parameters must rebuild the same intervals and
// queries; otherwise two published numbers were never comparable in the first place.
public sealed partial class MinimumIntervalToIncludeEachQueryBenchmarksTests
{
    // The smallest declared [Params] value: the query array is what both arms walk, and 200
    // queries already mix covered and uncovered coordinates.
    private const int SmallestCount = 200;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().HeapSweep()),
            AnswerText.Of(BuildHarness().HeapSweep()));

    [Fact]
    public void PerQueryScan_AgreesWithHeapSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.HeapSweep()), AnswerText.Of(harness.PerQueryScan()));
    }

    [Fact]
    public void HeapSweep_AgreesWithPerQueryScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.PerQueryScan()), AnswerText.Of(harness.HeapSweep()));
    }

    private static MinimumIntervalToIncludeEachQueryBenchmarks BuildHarness()
    {
        var harness = new MinimumIntervalToIncludeEachQueryBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
