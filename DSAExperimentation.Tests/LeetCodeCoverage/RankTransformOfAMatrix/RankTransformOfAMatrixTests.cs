using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RankTransformOfAMatrix;

// LeetCode 1632. Rank Transform of a Matrix: sort every cell by value with
// this repo's MergeSort (the same ArrayIndexedSequence convention
// AccountsMergeTests/QueueReconstructionByHeightTests use), then process
// equal-value cells one batch at a time with a fresh DisjointSet over
// (rows + cols) nodes - one node per row, one per column - unioning each
// cell's row node to its column node. Two equal-value cells only share a
// rank when a chain of shared rows/columns actually connects them, exactly
// what the union-find partition captures. Each batch's component gets
// rank = 1 + the max rank already assigned to any row/column it touches,
// tracked per-root via a HashMap<int,int> the same way AccountsMergeTests
// tracks emails per DisjointSet root.
public sealed partial class RankTransformOfAMatrixTests
{
    [Fact]
    public void MatrixRankTransform_LeetCodeExampleOne_RanksStrictlyIncreasingRowsAndColumns()
    {
        int[][] matrix =
        [
            [1, 2],
            [3, 4],
        ];

        int[][] expected =
        [
            [1, 2],
            [2, 3],
        ];

        Assert.Equal(expected, MatrixRankTransform(matrix));
    }

    [Fact]
    public void MatrixRankTransform_AllEqualValues_GivesEveryCellRankOne()
    {
        int[][] matrix =
        [
            [7, 7],
            [7, 7],
        ];

        int[][] expected =
        [
            [1, 1],
            [1, 1],
        ];

        Assert.Equal(expected, MatrixRankTransform(matrix));
    }

    [Fact]
    public void MatrixRankTransform_DisconnectedTies_RanksThemIndependently()
    {
        int[][] matrix =
        [
            [20, -21, 14],
            [-19, 4, 19],
            [22, -47, 24],
            [-19, 4, 19],
        ];

        int[][] expected =
        [
            [4, 2, 3],
            [1, 3, 4],
            [5, 1, 6],
            [1, 3, 4],
        ];

        Assert.Equal(expected, MatrixRankTransform(matrix));
    }

    private static int[][] MatrixRankTransform(int[][] matrix)
    {
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var cells = BuildCells(matrix, rows, cols);
        SortCellsByValue(cells);

        var rowRank = new int[rows];
        var colRank = new int[cols];
        var result = CreateEmptyResult(rows, cols);
        var grid = new RankingGrid(cells, rows, result, rowRank, colRank);
        var index = 0;

        while (index < cells.Length)
        {
            index = ProcessRankBatch(grid, index);
        }

        return result;
    }

    private static (int Value, int Row, int Col)[] BuildCells(int[][] matrix, int rows, int cols)
    {
        var cells = new (int Value, int Row, int Col)[rows * cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                cells[(r * cols) + c] = (matrix[r][c], r, c);
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

    private static int[][] CreateEmptyResult(int rows, int cols)
    {
        var result = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            result[r] = new int[cols];
        }

        return result;
    }

    // Advances past the batch of equal-value cells starting at `index`, assigning
    // them ranks as one group, and returns the index where the next batch starts.
    private static int ProcessRankBatch(RankingGrid grid, int index)
    {
        var end = index;

        while (end < grid.Cells.Length && grid.Cells[end].Value == grid.Cells[index].Value)
        {
            end++;
        }

        AssignBatchRanks(grid, index, end);
        return end;
    }

    private static void AssignBatchRanks(RankingGrid grid, int start, int end)
    {
        var components = new DisjointSet(grid.Rows + grid.ColRank.Length);

        for (var i = start; i < end; i++)
        {
            components.Union(grid.Cells[i].Row, grid.Rows + grid.Cells[i].Col);
        }

        var bestByRoot = new HashMap<int, int>();

        for (var i = start; i < end; i++)
        {
            RecordBestCandidate(grid, grid.Cells[i], components, bestByRoot);
        }

        for (var i = start; i < end; i++)
        {
            AssignRankToCell(grid, grid.Cells[i], components, bestByRoot);
        }
    }

    private static void RecordBestCandidate(
        RankingGrid grid, (int Value, int Row, int Col) cell, DisjointSet components, HashMap<int, int> bestByRoot)
    {
        var (_, row, col) = cell;
        var root = components.Find(row);
        var candidate = Math.Max(grid.RowRank[row], grid.ColRank[col]);

        if (!bestByRoot.TryGetValue(root, out var best) || candidate > best)
        {
            bestByRoot.Set(root, candidate);
        }
    }

    private static void AssignRankToCell(
        RankingGrid grid, (int Value, int Row, int Col) cell, DisjointSet components, HashMap<int, int> bestByRoot)
    {
        var (_, row, col) = cell;
        var root = components.Find(row);
        bestByRoot.TryGetValue(root, out var best);
        var rank = best + 1;

        grid.Result[row][col] = rank;
        grid.RowRank[row] = rank;
        grid.ColRank[col] = rank;
    }

    private readonly record struct RankingGrid(
        (int Value, int Row, int Col)[] Cells, int Rows, int[][] Result, int[] RowRank, int[] ColRank);
}
