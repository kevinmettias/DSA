using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountIntegersInIntervalsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a HashSet holding every individual integer against
// this repo's IntervalSet merging whole ranges - so a harness whose arms disagree is timing two
// different problems. Both arms return an int, so they are compared directly. Setup generates the
// fixed add() script from one seed and charges its construction to itself, so the same
// OperationCount must rebuild the same script; each arm builds its own stateful object per call, so
// the two arms are answering the same replay rather than one continuing the other's work.
public sealed partial class CountIntegersInIntervalsBenchmarksTests
{
    private const int SmallestOperationCount = 200;

    [Fact]
    public void Setup_SameOperationCount_RebuildsTheSameAddScript() =>
        Assert.Equal(BuildHarness().IntervalSetMerge(), BuildHarness().IntervalSetMerge());

    [Fact]
    public void HashSetPerInteger_TwoHundredReplayedRanges_AgreesWithIntervalSetMerge()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IntervalSetMerge(), harness.HashSetPerInteger());
    }

    [Fact]
    public void IntervalSetMerge_TwoHundredReplayedRanges_AgreesWithHashSetPerInteger()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashSetPerInteger(), harness.IntervalSetMerge());
    }

    private static CountIntegersInIntervalsBenchmarks BuildHarness()
    {
        var harness = new CountIntegersInIntervalsBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
