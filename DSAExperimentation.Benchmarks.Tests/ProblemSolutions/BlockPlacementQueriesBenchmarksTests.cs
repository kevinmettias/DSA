using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BlockPlacementQueriesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning the obstacle set per query against a
// segment tree merged with disjoint sets - so a harness whose arms disagree is timing two different
// problems, and the disagreement shows up per query, not as a summary. AnswerText.Of, not
// OfUnorderedSet: the answers come back one per operation in script order, and a set rendering would
// score a result against the wrong query. Setup is the only place the obstacle script is built, so
// the same QueryCount must rebuild the same script, and with it the same answers.
public sealed partial class BlockPlacementQueriesBenchmarksTests
{
    private const int SmallestQueryCount = 5_000;

    [Fact]
    public void Setup_SameQueryCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearScan()),
            AnswerText.Of(BuildHarness().LinearScan()));

    [Fact]
    public void LinearScan_HalfPlacementsHalfQueries_AgreesWithSegmentTreeMerge()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.SegmentTreeMerge()), AnswerText.Of(harness.LinearScan()));
    }

    [Fact]
    public void SegmentTreeMerge_HalfPlacementsHalfQueries_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.LinearScan()), AnswerText.Of(harness.SegmentTreeMerge()));
    }

    private static BlockPlacementQueriesBenchmarks BuildHarness()
    {
        var harness = new BlockPlacementQueriesBenchmarks { QueryCount = SmallestQueryCount };
        harness.Setup();

        return harness;
    }
}
