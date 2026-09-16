using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckingExistenceOfEdgeLengthLimitedPathsBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - one graph walk per query against a single
// offline sweep of both sorted lists - so a harness whose arms disagree is timing two different
// problems. Both arms answer with one bool per query in query order, which is part of this answer:
// answer i belongs to query i, so AnswerText.Of and not OfUnorderedSet is the rendering that keeps
// each verdict scored against its own query.
public sealed partial class CheckingExistenceOfEdgeLengthLimitedPathsBenchmarksTests
{
    private const int SmallestNodeCount = 100;

    // Setup draws two queries per node, so a rebuilt workload must answer with exactly this many
    // verdicts - one per query, none dropped and none invented.
    private const int QueryCountPerNodeMultiplier = 2;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameEdgesAndQueries()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The edge list and the query list are private, so the rebuild is pinned through what they
        // produce: one verdict per drawn query is the shape the answer promises, and the same
        // NodeCount must draw the same seeded edges and queries and answer them identically.
        Assert.Equal(SmallestNodeCount * QueryCountPerNodeMultiplier, first.DfsPerQuery().Length);
        Assert.Equal(AnswerText.Of(first.DfsPerQuery()), AnswerText.Of(second.OfflineDisjointSetSweep()));
    }

    [Fact]
    public void DfsPerQuery_SeededMultigraphAndQueries_AgreesWithOfflineDisjointSetSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.OfflineDisjointSetSweep()), AnswerText.Of(harness.DfsPerQuery()));
    }

    [Fact]
    public void OfflineDisjointSetSweep_SeededMultigraphAndQueries_AgreesWithDfsPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.DfsPerQuery()), AnswerText.Of(harness.OfflineDisjointSetSweep()));
    }

    private static CheckingExistenceOfEdgeLengthLimitedPathsBenchmarks BuildHarness()
    {
        var harness = new CheckingExistenceOfEdgeLengthLimitedPathsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
