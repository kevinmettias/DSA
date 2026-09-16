using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestDistanceAfterRoadAdditionQueriesIIBenchmarks (ARCHITECTURE 17.9): both
// arms answer the same per-query distance question about the same road additions, so a harness whose
// arms disagree is timing two different query sets. Setup carves the queries from the shared
// RoadAdditionQueryWorkloads fixture, which draws them from one fixed seed and guarantees they are
// pairwise nested or disjoint by construction, so the same CityCount must rebuild the same queries;
// neither arm writes to them, so one harness instance is safe to call twice in either order.
// AnswerText.Of renders the per-query distances in the order the problem pins.
public sealed partial class ShortestDistanceAfterRoadAdditionQueriesIIBenchmarksTests
{
    private const int SmallestCityCount = 100;

    [Fact]
    public void Setup_SameCityCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().AdjacencyBfs()),
            AnswerText.Of(BuildHarness().AdjacencyBfs()));

    [Fact]
    public void AdjacencyBfs_NonCrossingQueries_AgreesWithIntervalSet()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.IntervalSet()), AnswerText.Of(harness.AdjacencyBfs()));
    }

    [Fact]
    public void IntervalSet_NonCrossingQueries_AgreesWithAdjacencyBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.AdjacencyBfs()), AnswerText.Of(harness.IntervalSet()));
    }

    private static ShortestDistanceAfterRoadAdditionQueriesIIBenchmarks BuildHarness()
    {
        var harness = new ShortestDistanceAfterRoadAdditionQueriesIIBenchmarks { CityCount = SmallestCityCount };
        harness.Setup();

        return harness;
    }
}
