using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumDeletionsToMakeAlternatingSubstringBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - an O(r - l) rescan per query against a
// Fenwick-backed adjacency count - so a harness whose arms disagree is timing two different texts.
// Answers come back one per query in query order, and that order is part of the answer: the count at
// index i belongs to query i, so AnswerText.Of and not OfUnorderedSet is the rendering that keeps each
// count scored against its own query. Setup draws the text and the queries from one seeded stream, so
// the same Length must rebuild both.
public sealed partial class MinimumDeletionsToMakeAlternatingSubstringBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameTextAndQueries() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().DirectScan()),
            AnswerText.Of(BuildHarness().DirectScan()));

    [Fact]
    public void DirectScan_SeededTextAndRangeQueries_AgreesWithFenwickAdjacency()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.FenwickAdjacency()), AnswerText.Of(harness.DirectScan()));
    }

    [Fact]
    public void FenwickAdjacency_SeededTextAndRangeQueries_AgreesWithDirectScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.DirectScan()), AnswerText.Of(harness.FenwickAdjacency()));
    }

    private static MinimumDeletionsToMakeAlternatingSubstringBenchmarks BuildHarness()
    {
        var harness = new MinimumDeletionsToMakeAlternatingSubstringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
