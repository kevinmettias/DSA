using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximizeActiveSectionWithTradeIIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - re-encoding each query's window from scratch
// against this repo's own segment-tree maximum over the preprocessed zero-runs - so a harness whose
// arms disagree is timing two different problems. Answers come back one per query in query order, and
// query i's answer belongs to query i, so AnswerText.Of and not OfUnorderedSet is the rendering that
// keeps each answer scored against its own query. The index the composed arm is handed is immutable
// (the string, the run array and the segment tree are all built once), so no state is shared between
// the two calls and one harness is safe to call twice in either order. Setup draws the text and the
// queries from one fixed seed, so the same Length must rebuild the same workload.
public sealed partial class MaximizeActiveSectionWithTradeIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(
            AnswerText.Of(BuildHarness().RunScan()),
            AnswerText.Of(BuildHarness().RunScan()));
        Assert.Equal(
            AnswerText.Of(BuildHarness().RangeMaxIndex()),
            AnswerText.Of(BuildHarness().RangeMaxIndex()));
    }

    [Fact]
    public void RunScan_SeededZeroRichText_AgreesWithRangeMaxIndex()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RangeMaxIndex()),
            AnswerText.Of(harness.RunScan()));
    }

    [Fact]
    public void RangeMaxIndex_SeededZeroRichText_AgreesWithRunScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RunScan()),
            AnswerText.Of(harness.RangeMaxIndex()));
    }

    private static MaximizeActiveSectionWithTradeIIBenchmarks BuildHarness()
    {
        var harness = new MaximizeActiveSectionWithTradeIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
