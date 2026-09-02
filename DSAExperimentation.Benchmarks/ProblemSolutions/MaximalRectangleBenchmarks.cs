using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximalRectangle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximalRectangleSolution's, the same methods
// MaximalRectangleTests proves correct - the O(rows^2 * cols) row-pair-window
// baseline vs. the O(rows * cols) reduction to LC 84, one histogram-max-
// rectangle sweep per row using this repo's own Stack<int>.
[MemoryDiagnoser]
public class MaximalRectangleBenchmarks
{
    private const int CellValueUpperBound = 2;

    [Params(50, 300)]
    public int Size;

    private char[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size)
                .Select(_ => random.Next(0, CellValueUpperBound) == 0 ? '0' : '1').ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RowPairScan() => MaximalRectangleSolution.MaximalRectangleAreaByRowPairScan(_matrix);

    [Benchmark]
    public int RowHistogramStack() => MaximalRectangleSolution.MaximalRectangleAreaByRowHistogramStack(_matrix);
}
