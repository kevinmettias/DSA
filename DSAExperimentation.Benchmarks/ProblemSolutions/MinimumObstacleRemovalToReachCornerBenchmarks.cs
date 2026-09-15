using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.LeetCode.MinimumObstacleRemovalToReachCorner;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumObstacleRemovalToReachCornerSolution's, the
// same methods MinimumObstacleRemovalToReachCornerTests proves correct. The
// composed arm is handed the prepared obstacle-cost graph its hoisted overload
// takes, so wiring the grid is charged to [GlobalSetup] rather than to the
// Dijkstra search being measured.
[MemoryDiagnoser]
public class MinimumObstacleRemovalToReachCornerBenchmarks
{
    private const int RandomSeed = 2290; private int[][] _grid = [];

    private Dictionary<(int Row, int Col), WeightedGridNode> _nodes = new();
    // LC problem number

    [Params(15, 60)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _grid = ObstacleGridWorkloads.BuildGrid(Size, RandomSeed);
        _nodes = MinimumObstacleRemovalToReachCornerSolution.BuildObstacleCostGraph(_grid);
    }

    [Benchmark(Baseline = true)]
    public int ArrayScanDijkstra() =>
        MinimumObstacleRemovalToReachCornerSolution.MinimumObstaclesByArrayScanDijkstra(_grid);

    [Benchmark]
    public int WeightedGridDijkstra() =>
        MinimumObstacleRemovalToReachCornerSolution.MinimumObstaclesByWeightedGridDijkstra(_nodes, Size, Size);
}
