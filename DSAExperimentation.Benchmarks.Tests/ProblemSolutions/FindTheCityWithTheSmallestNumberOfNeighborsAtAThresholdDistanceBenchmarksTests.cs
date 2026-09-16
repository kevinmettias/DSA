using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for one question - a single-source
// Dijkstra run once per city against one Floyd-Warshall all-pairs closure - so a harness whose
// arms disagree is timing two different problems. Both are handed the same prepared CityGraph and
// the same distance threshold, and Setup rebuilds that graph from one fixed seed, so the same
// CityCount must rebuild the same road network.
public sealed partial class FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceBenchmarksTests
{
    private const int SmallestCityCount = 30;

    [Fact]
    public void Setup_SameCityCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DijkstraPerSource(), BuildHarness().DijkstraPerSource());

    [Fact]
    public void DijkstraPerSource_SameThreshold_AgreesWithFloydWarshallAllPairs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FloydWarshallAllPairs(), harness.DijkstraPerSource());
    }

    [Fact]
    public void FloydWarshallAllPairs_SameThreshold_AgreesWithDijkstraPerSource()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DijkstraPerSource(), harness.FloydWarshallAllPairs());
    }

    private static FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceBenchmarks BuildHarness()
    {
        var harness = new FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceBenchmarks
        {
            CityCount = SmallestCityCount,
        };
        harness.Setup();

        return harness;
    }
}
