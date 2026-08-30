using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reshape the Matrix (LC 566): a linear-index div/mod walk (recomputes both the
// source and destination row/col from a flat index every cell) vs. a cursor walk
// that increments destination row/col directly, wrapping only when a row fills up
// (no division or modulo per cell). Both visit exactly rows*cols cells - the gap
// is per-cell arithmetic overhead, not algorithm class, the same framing this
// repo's SpiralMatrixII benchmark already uses for a matrix-fill comparison.
[MemoryDiagnoser]
public class ReshapeTheMatrixBenchmarks
{
    [Params(20, 200)]
    public int Rows;

    private int[][] _mat = null!;
    private int _r;
    private int _c;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(566);
        const int cols = 8;
        _mat = Enumerable.Range(0, Rows).Select(_ => Enumerable.Range(0, cols).Select(_ => random.Next(1, 1_000)).ToArray()).ToArray();

        // Same total cell count (Rows*cols), reshaped into twice as many rows and half as many columns.
        _r = Rows * 2;
        _c = cols / 2;
    }

    [Benchmark(Baseline = true)]
    public int[][] LinearIndexDivMod()
    {
        var cols = _mat[0].Length;
        var reshaped = Enumerable.Range(0, _r).Select(_ => new int[_c]).ToArray();

        for (var i = 0; i < Rows * cols; i++)
        {
            reshaped[i / _c][i % _c] = _mat[i / cols][i % cols];
        }

        return reshaped;
    }

    [Benchmark]
    public int[][] CursorWalk()
    {
        var reshaped = Enumerable.Range(0, _r).Select(_ => new int[_c]).ToArray();
        var destRow = 0;
        var destCol = 0;

        for (var r = 0; r < _mat.Length; r++)
        {
            for (var c = 0; c < _mat[0].Length; c++)
            {
                reshaped[destRow][destCol] = _mat[r][c];
                destCol++;

                if (destCol != _c)
                {
                    continue;
                }

                destCol = 0;
                destRow++;
            }
        }

        return reshaped;
    }
}
