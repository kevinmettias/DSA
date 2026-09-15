using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountIslandsWithTotalValueDivisibleByK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountIslandsWithTotalValueDivisibleByKSolution's,
// the same methods CountIslandsWithTotalValueDivisibleByKTests proves correct.
// Neither strategy has meaningful construction to hoist beyond the grid itself,
// so [GlobalSetup] only builds the workload, not a prepared domain object.
[MemoryDiagnoser]
public class CountIslandsWithTotalValueDivisibleByKBenchmarks
{
    private const int GridSeed = 3619;
    private const int Divisor = 7;

    private int[][] _grid = [];

    [Params(50, 200)]
    public int GridSize { get; set; }

    [GlobalSetup]
    public void Setup() => _grid = IslandValueGridWorkloads.BuildGrid(GridSize, GridSize, GridSeed);

    [Benchmark(Baseline = true)]
    public int FloodFillStack() =>
        CountIslandsWithTotalValueDivisibleByKSolution.CountByFloodFillStack(_grid, Divisor);

    [Benchmark]
    public int DepthFirstSearchTraverse() =>
        CountIslandsWithTotalValueDivisibleByKSolution.CountByDepthFirstSearchTraverse(_grid, Divisor);
}
