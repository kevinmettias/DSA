using DiagonalStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.DiagonalTraverse;

// LeetCode 498. Diagonal Traverse: read an m x n matrix in the zig-zag diagonal
// order LeetCode publishes, alternating which end of each diagonal comes first.
//
// The two strategies are genuinely different mechanics for the same walk: the
// textbook approach bounces a single (row, col) cursor between the up-right and
// down-left directions in one O(1)-extra-space pass, while the other groups cells
// by row+col diagonal index and reverses every other diagonal with this repo's
// own LIFO Stack<int> - the same row-reversal primitive RotateImageSolution
// composes, applied to a matrix diagonal instead of a row.
internal static class DiagonalTraverseSolution
{
    private const int LastDiagonalIndexOffset = 2;
    private const int ParityDivisor = 2;

    // The textbook answer: a single cursor bouncing between two directions,
    // turning at whichever boundary it hits first. No extra structure beyond the
    // output array - the arm the diagonal-grouping strategy has to justify itself
    // against.
    public static int[] FindDiagonalOrderByDirectionToggle(int[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var result = new int[rows * cols];
        var state = new WalkState { Row = 0, Col = 0, GoingUp = true };

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = matrix[state.Row][state.Col];

            if (state.GoingUp)
            {
                AdvanceGoingUp(cols, state);
            }
            else
            {
                AdvanceGoingDown(rows, state);
            }
        }

        return result;
    }

    private static void AdvanceGoingUp(int cols, WalkState state)
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

    private static void AdvanceGoingDown(int rows, WalkState state)
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

    private sealed class WalkState
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public bool GoingUp { get; set; }
    }

    // Group cells by row+col diagonal index; every OTHER diagonal must come out
    // reversed, so pushing that diagonal's values onto this repo's own LIFO
    // Stack<int> and popping them back off reverses it with no separate reversal
    // loop.
    public static int[] FindDiagonalOrderByStackReversal(int[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var result = new int[rows * cols];
        var next = 0;

        for (var diagonal = 0; diagonal <= rows + cols - LastDiagonalIndexOffset; diagonal++)
        {
            next = WriteDiagonal(matrix, diagonal, (result, next));
        }

        return result;
    }

    // `output` is the destination buffer together with the write position into it -
    // the one place a diagonal's values land, and the one thing the walk advances.
    private static int WriteDiagonal(int[][] matrix, int diagonal, (int[] Result, int Next) output) =>
        IsReversedDiagonal(diagonal)
            ? WriteReversedDiagonal(matrix, diagonal, output)
            : WriteDirectDiagonal(matrix, diagonal, output);

    // Every other diagonal has to come out reversed, so which order a diagonal is
    // written in is its parity and nothing else.
    private static bool IsReversedDiagonal(int diagonal) => diagonal % ParityDivisor == 0;

    private static int WriteReversedDiagonal(int[][] matrix, int diagonal, (int[] Result, int Next) output)
    {
        var (result, next) = output;
        var range = RangeOf(matrix, diagonal);
        var reversed = new DiagonalStack();

        for (var r = range.Start; r <= range.End; r++)
        {
            reversed.Push(matrix[r][diagonal - r]);
        }

        for (var r = range.Start; r <= range.End; r++)
        {
            reversed.TryPop(out result[next++]);
        }

        return next;
    }

    private static int WriteDirectDiagonal(int[][] matrix, int diagonal, (int[] Result, int Next) output)
    {
        var (result, next) = output;
        var range = RangeOf(matrix, diagonal);

        for (var r = range.Start; r <= range.End; r++)
        {
            result[next++] = matrix[r][diagonal - r];
        }

        return next;
    }

    // A diagonal's row span is a function of the matrix shape and the diagonal index
    // alone, so the walkers derive it rather than taking it as a fifth argument.
    private static RowRange RangeOf(int[][] matrix, int diagonal) =>
        new(Math.Max(0, diagonal - matrix[0].Length + 1), Math.Min(diagonal, matrix.Length - 1));

    private readonly record struct RowRange(int Start, int End);
}
