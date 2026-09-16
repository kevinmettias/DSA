using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignEventManagerBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a full linear scan for the highest-priority event
// against this repo's own lazy-deletion heap - so a harness whose arms disagree is timing two
// different problems. Setup builds the whole call script from one fixed seed, so the same
// EventCount must rebuild the same script, and that script must drain a pool of exactly
// EventCount events in exactly EventCount polls.
public sealed partial class DesignEventManagerBenchmarksTests
{
    private const int SmallestEventCount = 200;

    // The ids of a pool of 1 through EventCount, polled away one event per poll.
    private const long ExpectedPolledIdSum = ((long)SmallestEventCount * (SmallestEventCount + 1)) / 2;

    [Fact]
    public void Setup_SameEventCount_RebuildsTheSameCallScript()
    {
        // The script re-prioritizes every event once - which adds nothing and removes nothing -
        // and then polls exactly as many times as there are events, so every poll has a target and
        // every event is polled away exactly once. The summed event ids are therefore the whole
        // pool's ids, 1 through EventCount, whatever order the priorities drew: a PollHighest that
        // returned the empty-pool sentinel instead of a live event would leave the total short.
        Assert.Equal(ExpectedPolledIdSum, BuildHarness().LinearScan());
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());
    }

    [Fact]
    public void LinearScan_ReprioritizeThenDrainScript_AgreesWithLazyDeletionHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LazyDeletionHeap(), harness.LinearScan());
    }

    [Fact]
    public void LazyDeletionHeap_ReprioritizeThenDrainScript_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.LazyDeletionHeap());
    }

    private static DesignEventManagerBenchmarks BuildHarness()
    {
        var harness = new DesignEventManagerBenchmarks { EventCount = SmallestEventCount };
        harness.Setup();

        return harness;
    }
}
