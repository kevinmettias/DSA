using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumStrictlyIncreasingCellsInAMatrix;

// LeetCode 2713. Maximum Strictly Increasing Cells in a Matrix: sort every cell by
// value with this repo's MergeSort (the same ArrayIndexedSequence convention
// RankTransformOfAMatrixTests/AccountsMergeTests use), then process equal-value cells
// one batch at a time. dp(row,col) = 1 + max(rowBest[row], colBest[col]) is computed
// for the whole batch from rowBest/colBest as they stood BEFORE the batch, and only
// folded back in afterward - so two cells that share a value can never extend one
// another's path, since neither strictly exceeds the other. No DisjointSet needed here
// (unlike RankTransformOfAMatrixTests' tie-rank problem): a cell only needs the best
// path already ending anywhere in its own row or column, not which other cells happen
// to share its exact value. Once every batch is folded in, the answer is simply the
// largest value rowBest/colBest ever reached, since every cell's dp got folded into
// both.
public sealed partial class MaximumStrictlyIncreasingCellsInAMatrixTests
{
    [Fact]
    public void MaxIncreasingCells_LeetCodeExampleOne_ReturnsTwo()
    {
        int[][] mat =
        [
            [3, 1],
            [3, 4],
        ];

        Assert.Equal(2, MaxIncreasingCells(mat));
    }

    [Fact]
    public void MaxIncreasingCells_LeetCodeExampleTwo_OneLowValueSurroundedByTies()
    {
        int[][] mat =
        [
            [3, 3, 3],
            [3, 2, 3],
            [3, 3, 3],
        ];

        Assert.Equal(2, MaxIncreasingCells(mat));
    }

    [Fact]
    public void MaxIncreasingCells_SingleRowStrictlyIncreasing_VisitsEveryCell()
    {
        int[][] mat = [[1, 2, 3, 4]];

        Assert.Equal(4, MaxIncreasingCells(mat));
    }

    [Fact]
    public void MaxIncreasingCells_AllValuesEqual_OnlyTheStartingCellCounts()
    {
        int[][] mat =
        [
            [5, 5],
            [5, 5],
        ];

        Assert.Equal(1, MaxIncreasingCells(mat));
    }

    private static int MaxIncreasingCells(int[][] mat)
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
        MergeSort.Sort<(int Value, int Row, int Col), ArrayIndexedSequence<(int Value, int Row, int Col)>>(
            new ArrayIndexedSequence<(int Value, int Row, int Col)>(cells), byValue);
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
