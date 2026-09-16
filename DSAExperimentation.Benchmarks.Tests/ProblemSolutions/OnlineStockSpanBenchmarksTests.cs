using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OnlineStockSpanBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one span sequence - the backward rescan against the amortized monotonic stack - so a
// harness whose arms disagree is timing two different problems. Setup feeds prices 1..Length, i.e.
// strictly increasing, which the class comment already names as the fixture's deliberate shape: every
// day's span therefore reaches back over every earlier day, so day i's span is exactly i + 1. That is
// an oracle derived from the fixture rather than from either arm, and both arms are held to it.
public sealed partial class OnlineStockSpanBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().MonotonicStackSweep()),
            AnswerText.Of(BuildHarness().MonotonicStackSweep()));

    [Fact]
    public void BruteForceBackwardScan_IncreasingPrices_SpanOfEveryDayIsItsDayNumber()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Enumerable.Range(1, SmallestLength)),
            AnswerText.Of(harness.BruteForceBackwardScan()));
        Assert.Equal(
            AnswerText.Of(harness.MonotonicStackSweep()),
            AnswerText.Of(harness.BruteForceBackwardScan()));
    }

    [Fact]
    public void MonotonicStackSweep_IncreasingPrices_SpanOfEveryDayIsItsDayNumber()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(Enumerable.Range(1, SmallestLength)),
            AnswerText.Of(harness.MonotonicStackSweep()));
        Assert.Equal(
            AnswerText.Of(harness.BruteForceBackwardScan()),
            AnswerText.Of(harness.MonotonicStackSweep()));
    }

    private static OnlineStockSpanBenchmarks BuildHarness()
    {
        var harness = new OnlineStockSpanBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
