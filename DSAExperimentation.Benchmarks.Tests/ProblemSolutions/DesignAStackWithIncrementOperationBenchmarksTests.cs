using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAStackWithIncrementOperationBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a BCL List<int> whose indexer reaches the bottom slots
// directly against this repo's Stack<int>, which is deliberately LIFO-only and so has to drain and rebuild
// - so a harness whose arms disagree is timing two different problems. Setup draws every pushed value from
// one fixed seed, and each arm builds its own stack inside the call, so a single harness is safe to call
// in either order. Both arms report the top of the stack after the replay, which is the last push: the
// increments are all applied once every value is already on the stack, so they only ever reach its bottom
// window and never the top. That makes the reading a pushed value from Setup's own domain, and the same
// PushCount must rebuild the same values and with it the same top.
public sealed partial class DesignAStackWithIncrementOperationBenchmarksTests
{
    private const int SmallestPushCount = 200;

    // [GlobalSetup] draws every pushed value from this half-open range.
    private const int MinPushedValue = 1;
    private const int MaxPushedValueExclusive = 1_000;

    // The top is never inside the increment window, so it keeps the value it was pushed with.
    private const int ExpectedTopValueLowerBound = MinPushedValue;
    private const int ExpectedTopValueUpperBound = MaxPushedValueExclusive - 1;

    [Fact]
    public void Setup_SamePushCount_RebuildsTheSameWorkload()
    {
        Assert.InRange(
            BuildHarness().IndexedListIncrement(),
            ExpectedTopValueLowerBound,
            ExpectedTopValueUpperBound);
        Assert.Equal(BuildHarness().IndexedListIncrement(), BuildHarness().IndexedListIncrement());
    }

    [Fact]
    public void IndexedListIncrement_TwoHundredPushes_AgreesWithStackDrainAndRebuild()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StackDrainAndRebuild(), harness.IndexedListIncrement());
    }

    [Fact]
    public void StackDrainAndRebuild_TwoHundredPushes_AgreesWithIndexedListIncrement()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IndexedListIncrement(), harness.StackDrainAndRebuild());
    }

    private static DesignAStackWithIncrementOperationBenchmarks BuildHarness()
    {
        var harness = new DesignAStackWithIncrementOperationBenchmarks { PushCount = SmallestPushCount };
        harness.Setup();

        return harness;
    }
}
