using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Transpose Matrix (LC 867): direct row-major index-swap transpose vs. a
// cache-blocked (tiled) transpose that processes fixed-size sub-blocks so both the
// read and write working sets stay small enough to fit in cache - a real, well-
// known transpose technique, not a repo primitive. No repo Representation/
// Operations primitive applies to either shape, same reasoning SpiralMatrixBenchmarks
// already states for GridChildren/GridTopology: those model unordered orthogonal
// adjacency for graph walks, not a fixed diagonal-flip copy.
[MemoryDiagnoser]
public class TransposeMatrixBenchmarks
{
    private const int BlockSize = 32;
    private const int MatrixValueUpperBoundExclusive = 1_000;

    [Params(100, 800)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _matrix = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _matrix[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _matrix[r][c] = random.Next(1, MatrixValueUpperBoundExclusive);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] DirectIndexSwap()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var result = NewMatrix(cols, rows);

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                result[c][r] = _matrix[r][c];
            }
        }

        return result;
    }

    [Benchmark]
    public int[][] CacheBlockedTranspose()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var result = NewMatrix(cols, rows);

        for (var blockRow = 0; blockRow < rows; blockRow += BlockSize)
        {
            for (var blockCol = 0; blockCol < cols; blockCol += BlockSize)
            {
                TransposeBlock(result, blockRow, blockCol);
            }
        }

        return result;
    }

    private void TransposeBlock(int[][] result, int blockRow, int blockCol)
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var rowLimit = Math.Min(blockRow + BlockSize, rows);
        var colLimit = Math.Min(blockCol + BlockSize, cols);

        for (var r = blockRow; r < rowLimit; r++)
        {
            for (var c = blockCol; c < colLimit; c++)
            {
                result[c][r] = _matrix[r][c];
            }
        }
    }

    private static int[][] NewMatrix(int rows, int cols)
    {
        var matrix = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            matrix[r] = new int[cols];
        }

        return matrix;
    }
}
