using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ZigzagGridTraversalWithSkip;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ZigzagGridTraversalWithSkipSolution's, the same
// methods ZigzagGridTraversalWithSkipTests proves correct. The grid needs no
// preprocessing beyond building it, so [GlobalSetup] only charges workload
// construction, not any part of either traversal.
[MemoryDiagnoser]
public class ZigzagGridTraversalWithSkipBenchmarks
{
    private const int Seed = 3417;

    // LeetCode caps both dimensions at 50; a square grid at that bound and a
    // smaller one keep both strategies exercising a real snake traversal.
    [Params(10, 50)]
    public int GridSize;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup() => _grid = ZigzagGridWorkloads.BuildGrid(GridSize, GridSize, Seed);

    [Benchmark(Baseline = true)]
    public int[] IndexFormula() => ZigzagGridTraversalWithSkipSolution.TraverseByIndexFormula(_grid);

    [Benchmark]
    public int[] RowStack() => ZigzagGridTraversalWithSkipSolution.TraverseByRowStack(_grid);
}
