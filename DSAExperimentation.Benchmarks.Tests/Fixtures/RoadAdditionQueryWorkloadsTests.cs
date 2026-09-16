using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RoadAdditionQueryWorkloads (ARCHITECTURE 17.7). The LC 3244 reading depends
// on the queries meeting the "II" variant's non-crossing guarantee by construction rather than by
// filtering random pairs down to the ones that happen to qualify: carving every query out of one
// range makes any two of them nested or disjoint, and nothing in between.
public sealed partial class RoadAdditionQueryWorkloadsTests
{
    private const int CityCount = 100;
    private const int ApproximateCount = 100;
    private const int Seed = 3244; // LC problem number
    private const int QueryFieldCount = 2; // FromCity, ToCity

    [Fact]
    public void BuildQueries_CityCount_KeepsEveryEndpointInsideTheCityRange()
    {
        var queries = RoadAdditionQueryWorkloads.BuildQueries(CityCount, ApproximateCount, Seed);

        Assert.All(queries, query => Assert.Equal(QueryFieldCount, query.Length));
        Assert.All(queries, query => Assert.InRange(query[0], 0, CityCount - 1));
        Assert.All(queries, query => Assert.InRange(query[1], 0, CityCount - 1));
        Assert.All(queries, query => Assert.True(query[0] < query[1]));
    }

    // The whole point of the carving recursion: a crossing pair would be input the "II" variant
    // never promises to handle, so a workload containing one would time a problem neither arm was
    // written for.
    [Fact]
    public void BuildQueries_EveryPairOfQueries_IsEitherNestedOrDisjoint()
    {
        var queries = RoadAdditionQueryWorkloads.BuildQueries(CityCount, ApproximateCount, Seed);

        foreach (var first in queries)
        {
            foreach (var second in queries)
            {
                Assert.True(
                    IsNested(first, second) || IsDisjoint(first, second),
                    $"LC 3244 queries must not cross: [{first[0]}, {first[1]}] and [{second[0]}, {second[1]}] do.");
            }
        }
    }

    // Every emitted query spends one of the caller's requested count, and a split spends none, so
    // the recursion can never hand back more queries than were asked for.
    [Fact]
    public void BuildQueries_ApproximateCount_ReturnsAtMostThatManyQueries()
    {
        var queries = RoadAdditionQueryWorkloads.BuildQueries(CityCount, ApproximateCount, Seed);

        Assert.InRange(queries.Length, 1, ApproximateCount);
    }

    [Fact]
    public void BuildQueries_SameSeed_ReturnsTheSameQueries() =>
        Assert.Equal(
            AnswerText.Of(RoadAdditionQueryWorkloads.BuildQueries(CityCount, ApproximateCount, Seed)),
            AnswerText.Of(RoadAdditionQueryWorkloads.BuildQueries(CityCount, ApproximateCount, Seed)));

    private static bool IsNested(int[] first, int[] second) =>
        (second[0] <= first[0] && first[1] <= second[1])
        || (first[0] <= second[0] && second[1] <= first[1]);

    private static bool IsDisjoint(int[] first, int[] second) =>
        first[1] <= second[0] || second[1] <= first[0];
}
