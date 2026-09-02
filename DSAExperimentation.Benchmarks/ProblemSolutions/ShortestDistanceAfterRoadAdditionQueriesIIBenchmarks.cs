using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ShortestDistanceAfterRoadAdditionQueriesII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestDistanceAfterRoadAdditionQueriesIISolution's, the
// same methods ShortestDistanceAfterRoadAdditionQueriesIITests proves correct.
[MemoryDiagnoser]
public class ShortestDistanceAfterRoadAdditionQueriesIIBenchmarks
{
    private const int Seed = 3244;

    [Params(100, 2_000)]
    public int CityCount;

    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup() => _queries = RoadAdditionQueryWorkloads.BuildQueries(CityCount, CityCount, Seed);

    [Benchmark(Baseline = true)]
    public int[] AdjacencyBfs() =>
        ShortestDistanceAfterRoadAdditionQueriesIISolution.ShortestDistancesByAdjacencyBfs(CityCount, _queries);

    [Benchmark]
    public int[] IntervalSet() =>
        ShortestDistanceAfterRoadAdditionQueriesIISolution.ShortestDistancesByIntervalSet(CityCount, _queries);
}
