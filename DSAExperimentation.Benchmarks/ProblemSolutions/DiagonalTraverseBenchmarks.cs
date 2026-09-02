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
    private const int LastDiagonalIndexOffset = 2;
    private const int ParityDivisor = 2;

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
        var state = new WalkState { Row = 0, Col = 0, GoingUp = true };

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = _matrix[state.Row][state.Col];

            if (state.GoingUp)
            {
                AdvanceGoingUp(cols, ref state);
            }
            else
            {
                AdvanceGoingDown(rows, ref state);
            }
        }

        return result;
    }

    private static void AdvanceGoingUp(int cols, ref WalkState state)
    {
        if (state.Col == cols - 1)
        {
            state.Row++;
            state.GoingUp = false;
        }
        else if (state.Row == 0)
        {
            state.Col++;
            state.GoingUp = false;
        }
        else
        {
            state.Row--;
            state.Col++;
        }
    }

    private static void AdvanceGoingDown(int rows, ref WalkState state)
    {
        if (state.Row == rows - 1)
        {
            state.Col++;
            state.GoingUp = true;
        }
        else if (state.Col == 0)
        {
            state.Row++;
            state.GoingUp = true;
        }
        else
        {
            state.Row++;
            state.Col--;
        }
    }

    private struct WalkState
    {
        public int Row;
        public int Col;
        public bool GoingUp;
    }

    [Benchmark]
    public int[] DiagonalGroupsWithStackReversal()
    {
        var rows = _matrix.Length;
        var cols = _matrix[0].Length;
        var result = new int[rows * cols];
        var next = 0;

        for (var diagonal = 0; diagonal <= rows + cols - LastDiagonalIndexOffset; diagonal++)
        {
            var range = new RowRange(Math.Max(0, diagonal - cols + 1), Math.Min(diagonal, rows - 1));
            WriteDiagonal(diagonal, range, result, ref next);
        }

        return result;
    }

    private void WriteDiagonal(int diagonal, RowRange range, int[] result, ref int next)
    {
        if (diagonal % ParityDivisor == 0)
        {
            WriteReversedDiagonal(diagonal, range, result, ref next);
        }
        else
        {
            WriteDirectDiagonal(diagonal, range, result, ref next);
        }
    }

    private void WriteReversedDiagonal(int diagonal, RowRange range, int[] result, ref int next)
    {
        var reversed = new DiagonalStack();

        for (var r = range.Start; r <= range.End; r++)
        {
            reversed.Push(_matrix[r][diagonal - r]);
        }

        for (var r = range.Start; r <= range.End; r++)
        {
            reversed.TryPop(out result[next++]);
        }
    }

    private void WriteDirectDiagonal(int diagonal, RowRange range, int[] result, ref int next)
    {
        for (var r = range.Start; r <= range.End; r++)
        {
            result[next++] = _matrix[r][diagonal - r];
        }
    }

    private readonly record struct RowRange(int Start, int End);
}
