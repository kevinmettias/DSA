using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OnlineStockSpanBenchmarks (ARCHITECTURE 17.9): the baseline's spans against an
// oracle derived from the fixture rather than from either arm, which the generic check cannot supply
// - it holds the monotonic stack to the baseline. Setup feeds prices 1..Length, i.e. strictly
// increasing, which the class comment already names as the fixture's deliberate shape: every day's
// span therefore reaches back over every earlier day, so day i's span is exactly i + 1.
public sealed partial class OnlineStockSpanBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void BruteForceBackwardScan_IncreasingPrices_SpanOfEveryDayIsItsDayNumber() =>
        Assert.Equal(
            AnswerGraphText.Of(Enumerable.Range(1, SmallestLength)),
            AnswerGraphText.Of(BuildHarness().BruteForceBackwardScan()));

    private static OnlineStockSpanBenchmarks BuildHarness()
    {
        var harness = new OnlineStockSpanBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
