using BenchmarkDotNet.Attributes;
using DiagonalStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Diagonal Traverse (LC 498): the classic O(1)-extra-space direction-toggle walk
// (bounce between up-right and down-left at each boundary, one pass, no extra
// structure) vs. grouping cells by diagonal and reversing every other diagonal
// with this repo's own LIFO Stack<int> - the same row-reversal primitive
// RotateImageBenchmarks composes, applied to a matrix diagonal instead of a row.
[MemoryDiagnoser]
public class DiagonalTraverseBenchmarks
{
    [Params(50, 300)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var value = 0;
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => value++).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] DirectionToggleWalk()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var result = new int[rows * cols];
        var row = 0;
        var col = 0;
        var goingUp = true;

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = _matrix[row][col];

            if (goingUp)
            {
                if (col == cols - 1)
                {
                    row++;
                    goingUp = false;
                }
                else if (row == 0)
                {
                    col++;
                    goingUp = false;
                }
                else
                {
                    row--;
                    col++;
                }
            }
            else
            {
                if (row == rows - 1)
                {
                    col++;
                    goingUp = true;
                }
                else if (col == 0)
                {
                    row++;
                    goingUp = true;
                }
                else
                {
                    row++;
                    col--;
                }
            }
        }

        return result;
    }

    [Benchmark]
    public int[] DiagonalGroupsWithStackReversal()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var result = new int[rows * cols];
        var next = 0;

        for (var diagonal = 0; diagonal <= rows + cols - 2; diagonal++)
        {
            var rowStart = Math.Max(0, diagonal - cols + 1);
            var rowEnd = Math.Min(diagonal, rows - 1);

            if (diagonal % 2 == 0)
            {
                var reversed = new DiagonalStack();

                for (var r = rowStart; r <= rowEnd; r++)
                {
                    reversed.Push(_matrix[r][diagonal - r]);
                }

                for (var r = rowStart; r <= rowEnd; r++)
                {
                    reversed.TryPop(out result[next++]);
                }
            }
            else
            {
                for (var r = rowStart; r <= rowEnd; r++)
                {
                    result[next++] = _matrix[r][diagonal - r];
                }
            }
        }

        return result;
    }
}
