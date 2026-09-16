using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ClosestNodesQueriesInABinarySearchTreeBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - rescanning every node for every query against
// one in-order walk bisected per query - so a harness whose arms disagree is timing two different
// problems. Both arms answer with one pair per query in query order, which is part of this answer:
// row i belongs to query i, so AnswerText.Of and not OfUnorderedSet is the rendering that keeps each
// pair scored against its own query.
public sealed partial class ClosestNodesQueriesInABinarySearchTreeBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameTreeAndQueries()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The tree and the query list are private, so the rebuild is pinned through what they
        // produce: one pair per drawn query is the shape the answer promises, and the same NodeCount
        // must shuffle the same seeded permutation into the same tree, draw the same queries off the
        // same stream, and answer them identically.
        Assert.Equal(SmallestNodeCount, first.InOrderBinarySearch().Length);
        Assert.Equal(AnswerText.Of(first.InOrderBinarySearch()), AnswerText.Of(second.InOrderBinarySearch()));
    }

    [Fact]
    public void LinearScanPerQuery_SeededTreeAndQueries_AgreesWithInOrderBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.InOrderBinarySearch()), AnswerText.Of(harness.LinearScanPerQuery()));
    }

    [Fact]
    public void InOrderBinarySearch_SeededTreeAndQueries_AgreesWithLinearScanPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.LinearScanPerQuery()), AnswerText.Of(harness.InOrderBinarySearch()));
    }

    private static ClosestNodesQueriesInABinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new ClosestNodesQueriesInABinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
