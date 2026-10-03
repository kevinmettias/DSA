using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ZigzagGridTraversalWithSkip;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ZigzagGridTraversalWithSkipSolution's, the same
// methods ZigzagGridTraversalWithSkipSolutionTests proves correct. The grid needs no
// preprocessing beyond building it, so [GlobalSetup] only charges workload
// construction, not any part of either traversal.
public class ZigzagGridTraversalWithSkipBenchmarks
{
    private const int Seed = 3417;

    private int[][] _grid = [];

    // LeetCode caps both dimensions at 50; a square grid at that bound and a
    // smaller one keep both strategies exercising a real snake traversal.
    [Params(10, 50)]
    public int GridSize { get; set; }

    [GlobalSetup]
    public void Setup() => _grid = ZigzagGridWorkloads.BuildGrid(GridSize, GridSize, Seed);

    [Benchmark(Baseline = true)]
    public int[] IndexFormula() => ZigzagGridTraversalWithSkipSolution.TraverseByIndexFormula(_grid);

    [Benchmark]
    public int[] RowStack() => ZigzagGridTraversalWithSkipSolution.TraverseByRowStack(_grid);
}
