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
        var cells = new (int Value, int Row, int Col)[rows * cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                cells[(r * cols) + c] = (matrix[r][c], r, c);
            }
        }

        var byValue = Comparer<(int Value, int Row, int Col)>.Create((a, b) => a.Value.CompareTo(b.Value));
        MergeSort.Sort<(int Value, int Row, int Col), ArrayIndexedSequence<(int Value, int Row, int Col)>>(
            new ArrayIndexedSequence<(int Value, int Row, int Col)>(cells), byValue);

        var rowRank = new int[rows];
        var colRank = new int[cols];
        var result = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            result[r] = new int[cols];
        }

        var index = 0;

        while (index < cells.Length)
        {
            var end = index;

            while (end < cells.Length && cells[end].Value == cells[index].Value)
            {
                end++;
            }

            AssignBatchRanks(cells, index, end, rows, result, rowRank, colRank);
            index = end;
        }

        return result;
    }

    private static void AssignBatchRanks(
        (int Value, int Row, int Col)[] cells, int start, int end, int rows,
        int[][] result, int[] rowRank, int[] colRank)
    {
        var components = new DisjointSet(rows + colRank.Length);

        for (var i = start; i < end; i++)
        {
            components.Union(cells[i].Row, rows + cells[i].Col);
        }

        var bestByRoot = new HashMap<int, int>();

        for (var i = start; i < end; i++)
        {
            var (_, row, col) = cells[i];
            var root = components.Find(row);
            var candidate = Math.Max(rowRank[row], colRank[col]);

            if (!bestByRoot.TryGetValue(root, out var best) || candidate > best)
            {
                bestByRoot.Set(root, candidate);
            }
        }

        for (var i = start; i < end; i++)
        {
            var (_, row, col) = cells[i];
            var root = components.Find(row);
            bestByRoot.TryGetValue(root, out var best);
            var rank = best + 1;

            result[row][col] = rank;
            rowRank[row] = rank;
            colRank[col] = rank;
        }
    }
}
