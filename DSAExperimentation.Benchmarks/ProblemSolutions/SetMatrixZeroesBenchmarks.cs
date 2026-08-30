using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Set Matrix Zeroes (LC 73): a full-matrix-copy baseline (snapshots the matrix so
// zero-detection never reads an already-zeroed cell, O(rows*cols) extra space)
// vs. this repo's own Set<int>, tracking only the zero rows/columns
// (O(rows+cols) extra space). Both methods clone the shared fixture first so
// mutating one iteration's result never corrupts the next.
[MemoryDiagnoser]
public class SetMatrixZeroesBenchmarks
{
    [Params(50, 300)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, 100) == 0 ? 0 : random.Next(1, 1_000)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[][] CopyAndScan()
    {
        var matrix = CloneMatrix(_matrix);
        var snapshot = CloneMatrix(matrix);

        for (var r = 0; r < matrix.Length; r++)
        {
            for (var c = 0; c < matrix[0].Length; c++)
            {
                if (snapshot[r][c] != 0)
                {
                    continue;
                }

                for (var cc = 0; cc < matrix[0].Length; cc++)
                {
                    matrix[r][cc] = 0;
                }

                for (var rr = 0; rr < matrix.Length; rr++)
                {
                    matrix[rr][c] = 0;
                }
            }
        }

        return matrix;
    }

    [Benchmark]
    public int[][] RowColumnSets()
    {
        var matrix = CloneMatrix(_matrix);
        var zeroRows = new Set<int>();
        var zeroCols = new Set<int>();

        for (var r = 0; r < matrix.Length; r++)
        {
            for (var c = 0; c < matrix[0].Length; c++)
            {
                if (matrix[r][c] == 0)
                {
                    zeroRows.TryAdd(r);
                    zeroCols.TryAdd(c);
                }
            }
        }

        for (var r = 0; r < matrix.Length; r++)
        {
            for (var c = 0; c < matrix[0].Length; c++)
            {
                if (zeroRows.Has(r) || zeroCols.Has(c))
                {
                    matrix[r][c] = 0;
                }
            }
        }

        return matrix;
    }

    private static int[][] CloneMatrix(int[][] source)
        => source.Select(row => (int[])row.Clone()).ToArray();
}
