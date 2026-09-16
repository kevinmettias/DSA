using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for CityRoadWorkloads (ARCHITECTURE 17.7). The reading depends on LC 1334's own
// int[][] road shape over a connected, undirected network, so neither shortest-path strategy is
// measured on a degenerate or partitioned graph.
public sealed partial class CityRoadWorkloadsTests
{
    private const int CityCount = 16;
    private const int ExtraRoadsPerCity = 3;
    private const int Seed = 1334; // LC problem number
    private const int RoadFieldCount = 3; // FromCity, ToCity, Weight
    private const int WeightFieldIndex = 2;
    private const int MinRoadWeight = 1;
    private const int MaxRoadWeight = 19;

    [Fact]
    public void BuildRoads_EveryRoad_JoinsTwoDistinctCitiesWithAWeightInsideTheBand()
    {
        var roads = CityRoadWorkloads.BuildRoads(CityCount, ExtraRoadsPerCity, Seed);

        Assert.All(roads, road => Assert.Equal(RoadFieldCount, road.Length));
        Assert.All(roads, road => Assert.NotEqual(road[0], road[1]));
        Assert.All(roads, road => Assert.InRange(road[0], 0, CityCount - 1));
        Assert.All(roads, road => Assert.InRange(road[1], 0, CityCount - 1));
        Assert.All(roads, road => Assert.InRange(road[WeightFieldIndex], MinRoadWeight, MaxRoadWeight));
    }

    // LC 1334's roads are undirected, and the generator emits its spanning road as
    // { later city, earlier city, weight } while the other generators emit { earlier, later },
    // so the direction the pair was written in is not part of what the graph says.
    [Fact]
    public void BuildRoads_EveryCityBeyondTheFirst_ReachesAnEarlierCity()
    {
        var roads = CityRoadWorkloads.BuildRoads(CityCount, ExtraRoadsPerCity, Seed);

        foreach (var city in Enumerable.Range(1, CityCount - 1))
        {
            Assert.Contains(roads, road => ReachesAnEarlierCity(road, city));
        }
    }

    [Fact]
    public void BuildRoads_RoadCount_StaysBetweenTheSpanningBackboneAndTheDensityCap()
    {
        var roads = CityRoadWorkloads.BuildRoads(CityCount, ExtraRoadsPerCity, Seed);
        var spanningBackbone = CityCount - 1;
        var densityCap = spanningBackbone + (CityCount * ExtraRoadsPerCity);

        Assert.InRange(roads.Length, spanningBackbone, densityCap);
    }

    [Fact]
    public void BuildRoads_SameSeed_ReturnsTheSameRoads() =>
        Assert.Equal(
            AnswerText.Of(CityRoadWorkloads.BuildRoads(CityCount, ExtraRoadsPerCity, Seed)),
            AnswerText.Of(CityRoadWorkloads.BuildRoads(CityCount, ExtraRoadsPerCity, Seed)));

    private static bool ReachesAnEarlierCity(int[] road, int city) =>
        (road[0] == city && road[1] < city) || (road[1] == city && road[0] < city);
}
