using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfRecentCallsBenchmarks (ARCHITECTURE 17.9): both arms replay the same
// non-decreasing timestamp stream and return one ping count per call - the full-history rescan against
// the sliding queue window - so a harness whose arms disagree is timing two different questions. The
// returned array is the problem's whole answer rather than a proxy, and its outer order is pinned to
// the call order, so the default order-sensitive rendering is the right comparison. Setup generates
// the stream from a fixed seed, so the same CallCount must rebuild the same one.
public sealed partial class NumberOfRecentCallsBenchmarksTests
{
    private const int SmallestCallCount = 500;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().FullHistoryRescan()),
            AnswerText.Of(BuildHarness().FullHistoryRescan()));

    [Fact]
    public void FullHistoryRescan_AgreesWithSlidingWindowQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SlidingWindowQueue()),
            AnswerText.Of(harness.FullHistoryRescan()));
    }

    [Fact]
    public void SlidingWindowQueue_AgreesWithFullHistoryRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.FullHistoryRescan()),
            AnswerText.Of(harness.SlidingWindowQueue()));
    }

    private static NumberOfRecentCallsBenchmarks BuildHarness()
    {
        var harness = new NumberOfRecentCallsBenchmarks { CallCount = SmallestCallCount };
        harness.Setup();

        return harness;
    }
}
