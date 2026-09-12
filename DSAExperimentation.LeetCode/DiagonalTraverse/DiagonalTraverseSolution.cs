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
            var range = new RowRange(Math.Max(0, diagonal - cols + 1), Math.Min(diagonal, rows - 1));
            next = WriteDiagonal(matrix, diagonal, range, result, next);
        }

        return result;
    }

    private static int WriteDiagonal(int[][] matrix, int diagonal, RowRange range, int[] result, int next) =>
        diagonal % ParityDivisor == 0
            ? WriteReversedDiagonal(matrix, diagonal, range, result, next)
            : WriteDirectDiagonal(matrix, diagonal, range, result, next);

    private static int WriteReversedDiagonal(int[][] matrix, int diagonal, RowRange range, int[] result, int next)
    {
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

    private static int WriteDirectDiagonal(int[][] matrix, int diagonal, RowRange range, int[] result, int next)
    {
        for (var r = range.Start; r <= range.End; r++)
        {
            result[next++] = matrix[r][diagonal - r];
        }

        return next;
    }

    private readonly record struct RowRange(int Start, int End);
}
