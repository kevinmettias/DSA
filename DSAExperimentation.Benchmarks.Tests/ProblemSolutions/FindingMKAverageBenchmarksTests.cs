using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindingMKAverageBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems. Both arms construct their own MKAverage inside the call and replay the same fixed
// stream, so one harness instance is safe to call twice in either order. Setup draws the stream from
// a fixed seed, so the same Length must rebuild the same workload.
//
// The agreement is weak by construction and the assertions say only what the arms expose: both arms
// return a single long checksum - the sum of every calculateMKAverage result over the stream - so a
// disagreement in the middle of the stream that happened to cancel in the sum would go unnoticed.
// What is compared is the aggregate the arms actually return, not the per-element answers.
public sealed partial class FindingMKAverageBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().SortingSlidingWindow()),
            AnswerText.Of(BuildHarness().SortingSlidingWindow()));

    [Fact]
    public void SortingSlidingWindow_AgreesWithFenwickOrderStatistics()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickOrderStatistics(), harness.SortingSlidingWindow());
    }

    [Fact]
    public void FenwickOrderStatistics_AgreesWithSortingSlidingWindow()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortingSlidingWindow(), harness.FenwickOrderStatistics());
    }

    private static FindingMKAverageBenchmarks BuildHarness()
    {
        var harness = new FindingMKAverageBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
