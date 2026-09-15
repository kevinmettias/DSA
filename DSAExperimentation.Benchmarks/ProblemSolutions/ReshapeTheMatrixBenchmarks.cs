using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ReshapeTheMatrix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reshape the Matrix (LC 566): a linear-index div/mod walk (recomputes both the
// source and destination row/col from a flat index every cell) vs. a cursor walk
// that increments destination row/col directly, wrapping only when a row fills up
// (no division or modulo per cell). Both visit exactly rows*cols cells - the gap
// is per-cell arithmetic overhead, not algorithm class, the same framing this
// repo's SpiralMatrixII benchmark already uses for a matrix-fill comparison. Both
// strategies are proved equivalent by ReshapeTheMatrixTests.
[MemoryDiagnoser]
public class ReshapeTheMatrixBenchmarks
{
    private const int RandomSeed = 566; // LC problem number
    private const int MaxCellValue = 1_000; // exclusive upper bound passed to Random.Next
    private const int Cols = 8;
    private const int ReshapeFactor = 2; private int[][] _mat = [];

    private int _r;
    private int _c;
    // rows multiplied, cols divided by the same factor to preserve total cell count

    [Params(20, 200)]
    public int Rows { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _mat = Enumerable.Range(0, Rows).Select(_ => Enumerable.Range(0, Cols).Select(_ => random.Next(1, MaxCellValue)).ToArray()).ToArray();

        // Same total cell count (Rows*Cols), reshaped into twice as many rows and half as many columns.
        _r = Rows * ReshapeFactor;
        _c = Cols / ReshapeFactor;
    }

    [Benchmark(Baseline = true)]
    public int[][] LinearIndexDivMod() => ReshapeTheMatrixSolution.ReshapeByLinearIndexDivMod(_mat, _r, _c);

    [Benchmark]
    public int[][] CursorWalk() => ReshapeTheMatrixSolution.ReshapeByCursorWalk(_mat, _r, _c);
}
