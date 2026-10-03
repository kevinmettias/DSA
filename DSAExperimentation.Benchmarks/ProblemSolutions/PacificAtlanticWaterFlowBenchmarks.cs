using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PacificAtlanticWaterFlow;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PacificAtlanticWaterFlowSolution's, the same methods
// PacificAtlanticWaterFlowSolutionTests proves correct - a per-cell DFS re-scan (for every
// cell, independently DFS downhill toward each ocean's border with a
// freshly-allocated rows*cols visited grid) vs. a multi-source reverse-flow flood
// fill from every border cell.
public class PacificAtlanticWaterFlowBenchmarks
{
    private const int MaxHeight = 1_000;

    private int[][] _heights = [];

    [Params(10, 25)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _heights = Enumerable.Range(0, Size)
            .Select(_ => SeededDraws.Values(Size, 0, MaxHeight, random))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public List<(int Row, int Col)> PerCellDfs() =>
        PacificAtlanticWaterFlowSolution.FindCellsByPerCellDfs(_heights);

    [Benchmark]
    public List<(int Row, int Col)> MultiSourceFloodFill() =>
        PacificAtlanticWaterFlowSolution.FindCellsByMultiSourceFloodFill(_heights);
}
