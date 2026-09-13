using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution's, the
// same methods its test proves correct. Each is handed the prepared CityGraph its
// hoisted overload takes, so building the road network is charged to
// [GlobalSetup] rather than to the search being measured - leaving the comparison
// where it belongs: this repo's single-source Dijkstra run once per city against
// one Floyd-Warshall call, the mirror image of ShortestPathAlgorithmBenchmarks'
// own point about picking the wrong tool for an all-pairs question.
[MemoryDiagnoser]
public class FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceBenchmarks
{
    private const int DistanceThreshold = 50;
    private const int RandomSeed = 1334; // LC problem number
    private const int ExtraRoadsPerCity = 2;

    [Params(30, 120)]
    public int CityCount;

    private CityGraph _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        var roads = CityRoadWorkloads.BuildRoads(CityCount, ExtraRoadsPerCity, seed: RandomSeed);

        _graph = CityGraph.Build(CityCount, roads);
    }

    [Benchmark(Baseline = true)]
    public int DijkstraPerSource() =>
        FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution
            .FindCityByDijkstraPerSource(_graph, DistanceThreshold);

    [Benchmark]
    public int FloydWarshallAllPairs() =>
        FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution
            .FindCityByFloydWarshall(_graph, DistanceThreshold);
}
