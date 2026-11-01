using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignEventManagerBenchmarks (ARCHITECTURE 17.9): the generic arm check
// compares the arms only with each other, so what stays here is a total derived from the script
// rather than from either arm: the script must drain a pool of exactly EventCount events in exactly
// EventCount polls.
public sealed partial class DesignEventManagerBenchmarksTests
{
    private const int SmallestEventCount = 200;

    // The ids of a pool of 1 through EventCount, polled away one event per poll.
    private const long ExpectedPolledIdSum = ((long)SmallestEventCount * (SmallestEventCount + 1)) / 2;

    // The script re-prioritizes every event once - which adds nothing and removes nothing - and then
    // polls exactly as many times as there are events, so every poll has a target and every event is
    // polled away exactly once. The summed event ids are therefore the whole pool's ids, 1 through
    // EventCount, whatever order the priorities drew: a PollHighest that returned the empty-pool
    // sentinel instead of a live event would leave the total short.
    [Fact]
    public void LinearScan_ReprioritizeThenDrainScript_PollsEveryEventOnce() =>
        Assert.Equal(ExpectedPolledIdSum, BuildHarness().LinearScan());

    private static DesignEventManagerBenchmarks BuildHarness()
    {
        var harness = new DesignEventManagerBenchmarks { EventCount = SmallestEventCount };
        harness.Setup();

        return harness;
    }
}
