using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DataStreamAsDisjointIntervalsBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests
// cannot pin: a bound on the summary that follows from the workload's construction rather than from either arm.
// Each arm drains the whole stream the way LeetCode replays it and returns the summarized intervals, whose
// documented shape is at least one interval and at most one per value added.
public sealed partial class DataStreamAsDisjointIntervalsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every value added belongs to some interval, so the summary is never empty.
    private const int MinimumIntervalCount = 1;

    [Fact]
    public void FullRebuildEachCall_TwoHundredSeededValues_SummarizesIntoAtMostOneIntervalPerValue() =>
        Assert.InRange(BuildHarness().FullRebuildEachCall().Count, MinimumIntervalCount, SmallestLength);

    [Fact]
    public void IntervalSetMerge_TwoHundredSeededValues_SummarizesIntoAtMostOneIntervalPerValue() =>
        Assert.InRange(BuildHarness().IntervalSetMerge().Count, MinimumIntervalCount, SmallestLength);

    private static DataStreamAsDisjointIntervalsBenchmarks BuildHarness()
    {
        var harness = new DataStreamAsDisjointIntervalsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
