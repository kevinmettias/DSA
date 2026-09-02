using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PowerGridMaintenance;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PowerGridMaintenanceSolution's, the same methods
// PowerGridMaintenanceTests proves correct.
[MemoryDiagnoser]
public class PowerGridMaintenanceBenchmarks
{
    private const int Seed = 3607;
    private const int QueriesPerStation = 5;

    [Params(200, 2_000)]
    public int StationCount;

    private int[][] _connections = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        _connections = PowerGridMaintenanceWorkloads.BuildConnections(StationCount, Seed);
        _queries = PowerGridMaintenanceWorkloads.BuildQueries(StationCount, StationCount * QueriesPerStation, Seed);
    }

    [Benchmark(Baseline = true)]
    public int[] UnionFindSortedSet() =>
        PowerGridMaintenanceSolution.MaintenanceResultsByUnionFindSortedSet(StationCount, _connections, _queries);

    [Benchmark]
    public int[] UnionFindHeap() =>
        PowerGridMaintenanceSolution.MaintenanceResultsByUnionFindHeap(StationCount, _connections, _queries);
}
