using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.LeetCode.FindASafeWalkThroughAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindASafeWalkThroughAGridSolution's, the same
// methods FindASafeWalkThroughAGridTests proves correct. The composed arm is
// handed the prepared cell-cost graph its hoisted overload takes, so graph
// construction is charged to [GlobalSetup] rather than the Dijkstra search being
// measured.
[MemoryDiagnoser]
public class FindASafeWalkThroughAGridBenchmarks
{
    private const int Seed = 3286;

    private int[][] _grid = [];

    private Dictionary<(int Row, int Col), WeightedGridNode> _nodes = new();
    // LeetCode caps health at m + n; the maximum for both benchmarked sizes keeps
    // every run's answer meaningful rather than trivially false.
    [Params(10, 50)]
    public int GridSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _grid = SafeWalkGridWorkloads.BuildGrid(GridSize, Seed);
        _nodes = FindASafeWalkThroughAGridSolution.BuildCellCostGraph(_grid, GridSize, GridSize);
    }

    [Benchmark(Baseline = true)]
    public bool BruteForceArrayDijkstra() =>
        FindASafeWalkThroughAGridSolution.IsSafeByBruteForceArrayDijkstra(_grid, health: 2 * GridSize);

    [Benchmark]
    public bool WeightedGridDijkstra() =>
        FindASafeWalkThroughAGridSolution.IsSafeByWeightedGridDijkstra(
            _nodes, (GridSize, GridSize), _grid[0][0], health: 2 * GridSize);
}
