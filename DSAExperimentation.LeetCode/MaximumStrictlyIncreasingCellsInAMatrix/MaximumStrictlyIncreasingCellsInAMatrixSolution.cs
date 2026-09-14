using DSAExperimentation.Algorithms.Sorting;

using CellSequence = DSAExperimentation.DataStructures.Sequence.ArrayIndexedSequence<(int Value, int Row, int Col)>;

namespace DSAExperimentation.LeetCode.MaximumStrictlyIncreasingCellsInAMatrix;

// LeetCode 2713. Maximum Strictly Increasing Cells in a Matrix: from a cell you may
// move to any cell in the same row or the same column holding a strictly greater
// value; report the most cells one such walk can visit.
//
// Both strategies answer the same question with the same signature, so the test
// harness can pin them to the same examples and the benchmark can time them against
// each other without either restating the algorithm.
internal static class MaximumStrictlyIncreasingCellsInAMatrixSolution
{
    // Textbook DAG memoization: dp(row, col) recurses into every strictly-smaller
    // cell in its own row and its own column, memoized per cell - so each of the
    // rows*cols cells rescans a whole row plus a whole column, O(rows*cols*(rows+cols)).
    // Deliberately written with nothing but a jagged array and an int[,] memo: it is
    // what you would write without this repo, and it is the arm the composed strategy
    // below has to justify itself against.
    public static int MaxIncreasingCellsByMemoizedRowColumnScan(int[][] mat)
    {
        var rows = mat.Length;
        var cols = mat[0].Length;
        var scan = new RowColumnScan(mat, new int[rows, cols]);
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                best = Math.Max(best, ComputeBest(scan, r, c));
            }
        }

        return best;
    }

    // A dp value is always at least 1, so 0 doubles as "not computed yet" and the
    // memo needs no separate occupancy map.
    private static int ComputeBest(RowColumnScan scan, int row, int col)
    {
        if (scan.Memo[row, col] != 0)
        {
            return scan.Memo[row, col];
        }

        var mat = scan.Matrix;
        var best = 1;

        for (var c = 0; c < mat[0].Length; c++)
        {
            if (mat[row][c] < mat[row][col])
            {
                best = Math.Max(best, 1 + ComputeBest(scan, row, c));
            }
        }

        for (var r = 0; r < mat.Length; r++)
        {
            if (mat[r][col] < mat[row][col])
            {
                best = Math.Max(best, 1 + ComputeBest(scan, r, col));
            }
        }

        scan.Memo[row, col] = best;
        return best;
    }

    private readonly record struct RowColumnScan(int[][] Matrix, int[,] Memo);

    // Composed: sort every cell by value once with this repo's own MergeSort over an
    // ArrayIndexedSequence (the same convention AccountsMerge and RankTransformOfAMatrix
    // use), then walk the sorted cells one equal-value batch at a time.
    // dp(row, col) = 1 + max(rowBest[row], colBest[col]) is computed for the whole batch
    // from rowBest/colBest as they stood BEFORE the batch and only folded back in
    // afterward - so two cells sharing a value can never extend one another's path, since
    // neither strictly exceeds the other. No DisjointSet is needed (unlike
    // RankTransformOfAMatrix's tie-rank problem): a cell only needs the best path already
    // ending anywhere in its own row or column, not which other cells share its exact
    // value. Every cell's dp is folded into both running arrays, so the answer is simply
    // the largest value they ever reached. O(rows*cols*log(rows*cols)), with no per-cell
    // row/column rescan at all.
    public static int MaxIncreasingCellsBySortedBatchDp(int[][] mat)
    {
        var rows = mat.Length;
        var cols = mat[0].Length;
        var cells = BuildCells(mat, rows, cols);

        SortCellsByValue(cells);

        var rowBest = new int[rows];
        var colBest = new int[cols];
        var index = 0;

        while (index < cells.Length)
        {
            index = ProcessBatch(cells, index, rowBest, colBest);
        }

        return Math.Max(rowBest.Max(), colBest.Max());
    }

    private static (int Value, int Row, int Col)[] BuildCells(int[][] mat, int rows, int cols)
    {
        var cells = new (int Value, int Row, int Col)[rows * cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                cells[(r * cols) + c] = (mat[r][c], r, c);
            }
        }

        return cells;
    }

    private static void SortCellsByValue((int Value, int Row, int Col)[] cells)
    {
        var byValue = Comparer<(int Value, int Row, int Col)>.Create((a, b) => a.Value.CompareTo(b.Value));
        var sequence = new CellSequence(cells);

        MergeSort.Sort<(int Value, int Row, int Col), CellSequence>(sequence, byValue);
    }

    // Advances past the batch of equal-value cells starting at `index`, computing each
    // cell's dp value from rowBest/colBest as they stood before the batch, then folding
    // the whole batch's results back in. Returns the index where the next batch starts.
    private static int ProcessBatch((int Value, int Row, int Col)[] cells, int index, int[] rowBest, int[] colBest)
    {
        var end = index;

        while (end < cells.Length && cells[end].Value == cells[index].Value)
        {
            end++;
        }

        var dp = new int[end - index];

        for (var i = index; i < end; i++)
        {
            var (_, row, col) = cells[i];
            dp[i - index] = 1 + Math.Max(rowBest[row], colBest[col]);
        }

        for (var i = index; i < end; i++)
        {
            var (_, row, col) = cells[i];
            rowBest[row] = Math.Max(rowBest[row], dp[i - index]);
            colBest[col] = Math.Max(colBest[col], dp[i - index]);
        }

        return end;
    }
}
