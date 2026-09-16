using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DataStreamAsDisjointIntervalsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - re-deriving the whole summarized range set on every
// addNum against merging each value into this repo's interval set - so a harness whose arms disagree is
// timing two different problems. Setup draws the stream from one fixed seed over a range that spreads
// beyond the value count so intervals genuinely merge, and each arm drains the whole stream the way
// LeetCode replays it and reports the summarized interval count, whose documented shape is at least one
// interval and at most one per value added. The same Length must rebuild the same stream and with it
// the same summary.
public sealed partial class DataStreamAsDisjointIntervalsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every value added belongs to some interval, so the summary is never empty.
    private const int MinimumIntervalCount = 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.InRange(
            BuildHarness().FullRebuildEachCall(),
            MinimumIntervalCount,
            SmallestLength);
        Assert.Equal(BuildHarness().FullRebuildEachCall(), BuildHarness().FullRebuildEachCall());
    }

    [Fact]
    public void FullRebuildEachCall_TwoHundredSeededValues_AgreesWithIntervalSetMerge()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IntervalSetMerge(), harness.FullRebuildEachCall());
    }

    [Fact]
    public void IntervalSetMerge_TwoHundredSeededValues_AgreesWithFullRebuildEachCall()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FullRebuildEachCall(), harness.IntervalSetMerge());
    }

    private static DataStreamAsDisjointIntervalsBenchmarks BuildHarness()
    {
        var harness = new DataStreamAsDisjointIntervalsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
