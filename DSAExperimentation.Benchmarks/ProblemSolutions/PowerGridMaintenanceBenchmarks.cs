using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.LeetCode.Solutions.PowerGridMaintenance;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PowerGridMaintenanceSolution's, the same methods
// PowerGridMaintenanceSolutionTests proves correct. Grouping stations into grids is the
// step both share, so [GlobalSetup] does it once and each arm is handed the grids and
// timed on what it keeps per grid. Queries only ever Find, never Union, so the grids
// stay the same input on every invocation.
public class PowerGridMaintenanceBenchmarks
{
    private const int Seed = 3607;
    private const int QueriesPerStation = 5;

    private int[][] _connections = [];

    private int[][] _queries = [];

    private DisjointSet _grids = null!;

    [Params(200, 2_000)]
    public int StationCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _connections = PowerGridMaintenanceWorkloads.BuildConnections(StationCount, Seed);
        _queries = PowerGridMaintenanceWorkloads.BuildQueries(StationCount, StationCount * QueriesPerStation, Seed);
        _grids = PowerGridMaintenanceSolution.GroupByGrid(StationCount, _connections);
    }

    [Benchmark(Baseline = true)]
    public int[] UnionFindSortedSet() =>
        PowerGridMaintenanceSolution.MaintenanceResultsByUnionFindSortedSet(StationCount, _grids, _queries);

    [Benchmark]
    public int[] UnionFindHeap() =>
        PowerGridMaintenanceSolution.MaintenanceResultsByUnionFindHeap(StationCount, _grids, _queries);
}
