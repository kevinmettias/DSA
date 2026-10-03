using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfPointsFromGridQueriesBenchmarks (ARCHITECTURE 17.9): both arms
// are MaximumNumberOfPointsFromGridQueriesSolution's competing strategies for one question - a fresh
// per-query flood fill against one shared min-heap frontier drained across all queries - so a
// harness whose arms disagree is timing two different problems.
//
// Each arm answers with one point count per query, in the queries' original order, which is part of
// this answer: count i belongs to query i, so AnswerGraphText.Of and not OfUnordered is the rendering
// that keeps each count scored against its own query. The per-query arm keeps its own visited
// bitmap and never writes the grid, so one harness instance is safe to call twice in either order.
public sealed partial class MaximumNumberOfPointsFromGridQueriesBenchmarksTests
{
    private const int SmallestQueriesCount = 50;

    [Fact]
    public void Setup_SameQueriesCount_RebuildsTheSameGridAndQueries()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // One count per query is the shape both arms promise, so the rebuilt workload is pinned
        // through the counts it produces: the same QueriesCount must draw the same seeded grid and
        // query list and answer it identically.
        Assert.Equal(SmallestQueriesCount, first.FloodFillPerQuery().Length);
        Assert.Equal(AnswerGraphText.Of(first.MinHeapFloodFill()), AnswerGraphText.Of(second.MinHeapFloodFill()));
    }

    [Fact]
    public void FloodFillPerQuery_SeededGridAndQueries_AgreesWithMinHeapFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerGraphText.Of(harness.MinHeapFloodFill()), AnswerGraphText.Of(harness.FloodFillPerQuery()));
    }

    [Fact]
    public void MinHeapFloodFill_SeededGridAndQueries_AgreesWithFloodFillPerQuery()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerGraphText.Of(harness.FloodFillPerQuery()), AnswerGraphText.Of(harness.MinHeapFloodFill()));
    }

    private static MaximumNumberOfPointsFromGridQueriesBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfPointsFromGridQueriesBenchmarks { QueriesCount = SmallestQueriesCount };
        harness.Setup();

        return harness;
    }
}
