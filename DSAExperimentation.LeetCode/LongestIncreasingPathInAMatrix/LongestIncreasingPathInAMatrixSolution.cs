using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.LongestIncreasingPathInAMatrix;

// LeetCode 329. Longest Increasing Path in a Matrix: the longest strictly-increasing
// path starting at each cell, maximized over every cell as a candidate start. A
// strictly-increasing value relation can never revisit a cell already on the path, so
// the induced relation is a DAG - safe to memoize, and the two strategies differ only
// in whether they do.
internal static class LongestIncreasingPathInAMatrixSolution
{
    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The textbook answer: plain recursion, re-exploring every shared sub-path from
    // scratch per candidate start cell. Deliberately written without this repo's
    // primitives - it is the arm the memoized strategy below has to justify itself
    // against.
    public static int LongestPathByNaiveRecursion(int[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        var best = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                var lengthHere = LengthFromNaive(matrix, row, col);
                best = Math.Max(best, lengthHere);
            }
        }

        return best;
    }

    // This repo's own Memoizer caches, per cell, the longest strictly-increasing path
    // starting there - the same (Row,Col)-state grid recurrence UniquePaths/EditDistance
    // already use, closed over four-directional neighbors instead of just right/down.
    public static int LongestPathByMemoizedRecurrence(int[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        var best = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                var length = Memoizer.Memoize<(int Row, int Col), int>(
                    (row, col), new LongestIncreasingPathFromCell(matrix));
                best = Math.Max(best, length);
            }
        }

        return best;
    }

    private static int LengthFromNaive(int[,] matrix, int row, int col)
    {
        var longest = 1;
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);

        foreach (var (rowOffset, colOffset) in Directions)
        {
            var nextRow = row + rowOffset;
            var nextCol = col + colOffset;

            if (IsInside(nextRow, nextCol, rows, cols)
                && matrix[nextRow, nextCol] > matrix[row, col])
            {
                longest = Math.Max(longest, 1 + LengthFromNaive(matrix, nextRow, nextCol));
            }
        }

        return longest;
    }

    // Both coordinates within the matrix is one idea, and it was written out twice.
    private static bool IsInside(int row, int col, int rows, int cols)
        => row >= 0 && row < rows && col >= 0 && col < cols;

    // The recurrence, named: a cell's longest increasing path is one step past the best
    // of its four strictly-greater neighbours, and a cell with none is a path of one.
    private sealed class LongestIncreasingPathFromCell(int[,] matrix)
        : IRecurrence<(int Row, int Col), int>
    {
        /// <inheritdoc/>
        public int Replay((int Row, int Col) state, IRecurrence<(int Row, int Col), int> rest)
        {
            var longest = 1;
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);

            foreach (var (rowOffset, colOffset) in Directions)
            {
                var nextRow = state.Row + rowOffset;
                var nextCol = state.Col + colOffset;

                if (IsInside(nextRow, nextCol, rows, cols)
                    && matrix[nextRow, nextCol] > matrix[state.Row, state.Col])
                {
                    longest = Math.Max(longest, 1 + rest.Replay((nextRow, nextCol), rest));
                }
            }

            return longest;
        }
    }
}
