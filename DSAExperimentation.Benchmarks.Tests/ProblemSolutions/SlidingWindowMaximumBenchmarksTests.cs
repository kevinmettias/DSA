using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SlidingWindowMaximumBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a full rescan of every window against one
// monotonic deque sweep - so a harness whose arms disagree is sliding over two different
// arrays. Setup draws the values from one fixed seed, so the same Length must rebuild the same
// array; otherwise two published numbers were never comparable.
//
// The answer is one maximum per window, and window i's maximum belongs to window i, so
// AnswerText.Of rather than OfUnorderedSet keeps each maximum scored against its own window.
public sealed partial class SlidingWindowMaximumBenchmarksTests
{
    private const int SmallestLength = 2_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValues() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceRescan()),
            AnswerText.Of(BuildHarness().BruteForceRescan()));

    [Fact]
    public void BruteForceRescan_FiftyWideWindowOverTwoThousandValues_AgreesWithMonotonicDeque()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MonotonicDeque()),
            AnswerText.Of(harness.BruteForceRescan()));
    }

    [Fact]
    public void MonotonicDeque_FiftyWideWindowOverTwoThousandValues_AgreesWithBruteForceRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceRescan()),
            AnswerText.Of(harness.MonotonicDeque()));
    }

    private static SlidingWindowMaximumBenchmarks BuildHarness()
    {
        var harness = new SlidingWindowMaximumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
