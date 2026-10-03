using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

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
            AnswerGraphText.Of(BuildHarness().MonotonicStackSweep()),
            AnswerGraphText.Of(BuildHarness().MonotonicStackSweep()));

    [Fact]
    public void BruteForceBackwardScan_IncreasingPrices_SpanOfEveryDayIsItsDayNumber()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.Of(Enumerable.Range(1, SmallestLength)),
            AnswerGraphText.Of(harness.BruteForceBackwardScan()));
        Assert.Equal(
            AnswerGraphText.Of(harness.MonotonicStackSweep()),
            AnswerGraphText.Of(harness.BruteForceBackwardScan()));
    }

    [Fact]
    public void MonotonicStackSweep_IncreasingPrices_SpanOfEveryDayIsItsDayNumber()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.Of(Enumerable.Range(1, SmallestLength)),
            AnswerGraphText.Of(harness.MonotonicStackSweep()));
        Assert.Equal(
            AnswerGraphText.Of(harness.BruteForceBackwardScan()),
            AnswerGraphText.Of(harness.MonotonicStackSweep()));
    }

    private static OnlineStockSpanBenchmarks BuildHarness()
    {
        var harness = new OnlineStockSpanBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
