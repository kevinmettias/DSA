using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfPointsFromGridQueriesBenchmarks (ARCHITECTURE 17.9): each arm
// answers with one point count per query, in the queries' original order - count i belongs to query
// i - and Setup draws QueriesCount queries. That shape is a property of the workload the generic
// agreement check, which only compares arms with each other, cannot supply, so the baseline's count
// of answers is pinned to it.
public sealed partial class MaximumNumberOfPointsFromGridQueriesBenchmarksTests
{
    private const int SmallestQueriesCount = 50;

    // One count per query is the shape both arms promise.
    [Fact]
    public void FloodFillPerQuery_SeededGridAndQueries_AnswersEveryQuery() =>
        Assert.Equal(SmallestQueriesCount, BuildHarness().FloodFillPerQuery().Length);

    private static MaximumNumberOfPointsFromGridQueriesBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfPointsFromGridQueriesBenchmarks { QueriesCount = SmallestQueriesCount };
        harness.Setup();

        return harness;
    }
}
