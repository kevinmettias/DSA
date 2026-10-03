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
//
// LC 1334 caps the cities at 100 and lists each road once, lower city first. The
// shared road workload writes some roads higher city first and can draw the same
// pair twice, so each road is turned lower-first and only the first draw of a pair
// is kept; the larger CityCount is that cap.
public class FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceBenchmarks
{
    private const int DistanceThreshold = 50;
    private const int RandomSeed = 1334; // LC problem number
    private const int ExtraRoadsPerCity = 2;

    private CityGraph _graph = null!;

    [Params(30, 100)]
    public int CityCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var drawnRoads = CityRoadWorkloads.BuildRoads(CityCount, ExtraRoadsPerCity, seed: RandomSeed);
        var roads = drawnRoads.Select(LowerCityFirst).DistinctBy(road => (From: road[0], To: road[1])).ToArray();

        _graph = CityGraph.Build(CityCount, roads);
    }

    private static int[] LowerCityFirst(int[] road)
    {
        var (from, to, weight) = (road[0], road[1], road[2]);

        return [Math.Min(from, to), Math.Max(from, to), weight];
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
