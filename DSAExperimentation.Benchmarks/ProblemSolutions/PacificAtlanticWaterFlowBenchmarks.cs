using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PacificAtlanticWaterFlow;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PacificAtlanticWaterFlowSolution's, the same methods
// PacificAtlanticWaterFlowTests proves correct - a per-cell DFS re-scan (for every
// cell, independently DFS downhill toward each ocean's border with a
// freshly-allocated rows*cols visited grid) vs. a multi-source reverse-flow flood
// fill from every border cell.
[MemoryDiagnoser]
public class PacificAtlanticWaterFlowBenchmarks
{
    private const int MaxHeight = 1_000;

    [Params(10, 25)]
    public int Size;

    private int[][] _heights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _heights = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, MaxHeight)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public List<(int Row, int Col)> PerCellDfs() =>
        PacificAtlanticWaterFlowSolution.FindCellsByPerCellDfs(_heights);

    [Benchmark]
    public List<(int Row, int Col)> MultiSourceFloodFill() =>
        PacificAtlanticWaterFlowSolution.FindCellsByMultiSourceFloodFill(_heights);
}
