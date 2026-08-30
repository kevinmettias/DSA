using BenchmarkDotNet.Attributes;
using StackOfInt = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Rotate Image (LC 48): transpose-then-reverse using the BCL's Array.Reverse vs. the
// same transpose followed by this repo's own LIFO Stack<T> to reverse each row - the
// same digit-reversal primitive ReverseInteger composes, applied to matrix rows
// instead of decimal digits.
[MemoryDiagnoser]
public class RotateImageBenchmarks
{
    [Params(50, 300)]
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
                _matrix[r][c] = random.Next(1, 1_000);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] TransposeThenArrayReverse()
    {
        var matrix = Clone(_matrix);
        Transpose(matrix);

        foreach (var row in matrix)
        {
            Array.Reverse(row);
        }

        return matrix;
    }

    [Benchmark]
    public int[][] TransposeThenStackReverse()
    {
        var matrix = Clone(_matrix);
        Transpose(matrix);

        foreach (var row in matrix)
        {
            ReverseWithStack(row);
        }

        return matrix;
    }

    private static void Transpose(int[][] matrix)
    {
        for (var r = 0; r < matrix.Length; r++)
        {
            for (var c = r + 1; c < matrix.Length; c++)
            {
                (matrix[r][c], matrix[c][r]) = (matrix[c][r], matrix[r][c]);
            }
        }
    }

    private static void ReverseWithStack(int[] row)
    {
        var pushed = new StackOfInt();

        foreach (var value in row)
        {
            pushed.Push(value);
        }

        for (var i = 0; i < row.Length; i++)
        {
            pushed.TryPop(out row[i]);
        }
    }

    private static int[][] Clone(int[][] matrix)
    {
        var copy = new int[matrix.Length][];

        for (var r = 0; r < matrix.Length; r++)
        {
            copy[r] = (int[])matrix[r].Clone();
        }

        return copy;
    }
}
