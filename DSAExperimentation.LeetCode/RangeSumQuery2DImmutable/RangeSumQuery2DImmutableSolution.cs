using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.RangeSumQuery2DImmutable;

// LeetCode 304. Range Sum Query 2D - Immutable: given an immutable matrix, answer repeated
// SumRegion(row1, col1, row2, col2) queries; LeetCode requires SumRegion in O(1).
//
// This is a design problem - LeetCode's own shape is a stateful object (a constructor plus a
// SumRegion operation), not a single return value - the same shape MinStackSolution and
// LRUCacheSolution use for their own design problems. Three strategies, by how much SumRegion
// pays: the baseline rescans every cell of the region; the row Fenwick trees - one of this repo's
// own FenwickTree<int, SumOperation<int>> per row, the move RangeFenwickTree.cs makes with two -
// pay O(log cols) per covered row, which is the right shape only if the matrix could change
// (LC 308); and the prefix-sum table answers in four lookups, the O(1) LeetCode asks for, because
// the matrix never changes.
internal static class RangeSumQuery2DImmutableSolution
{
    // The textbook baseline this composition has to justify itself against: no preprocessing at
    // all, just an O(rows*cols) scan of the raw matrix per SumRegion call.
    public static INumMatrix CreateByBruteForceCellScan(int[][] matrix) => new BruteForceCellScanMatrix(matrix);

    // One FenwickTree per row, built once so each SumRegion call afterward walks only the covered
    // rows and does an O(log cols) query per row.
    public static INumMatrix CreateByRowFenwickTree(int[][] matrix) => new RowFenwickTreeMatrix(matrix);

    // The O(1) answer LeetCode asks for: a table of every top-left rectangle's sum, built once, so a
    // region is its bottom-right rectangle minus the two strips beside it plus the corner they both
    // removed.
    public static INumMatrix CreateByPrefixSums(int[][] matrix) => new PrefixSumMatrix(matrix);

    private sealed class BruteForceCellScanMatrix(int[][] matrix) : INumMatrix
    {
        public int SumRegion(int row1, int col1, int row2, int col2)
        {
            var total = 0;

            for (var row = row1; row <= row2; row++)
            {
                for (var col = col1; col <= col2; col++)
                {
                    total += matrix[row][col];
                }
            }

            return total;
        }
    }

    private sealed class RowFenwickTreeMatrix : INumMatrix
    {
        private readonly FenwickTree<int, SumOperation<int>>[] _rows;

        public RowFenwickTreeMatrix(int[][] matrix) =>
            _rows = matrix.Select(row => new FenwickTree<int, SumOperation<int>>(row)).ToArray();

        public int SumRegion(int row1, int col1, int row2, int col2)
        {
            var total = 0;

            for (var row = row1; row <= row2; row++)
            {
                total += _rows[row].Query(col1, col2);
            }

            return total;
        }
    }

    private sealed class PrefixSumMatrix : INumMatrix
    {
        // _prefix[r + 1, c + 1] is the sum of matrix[0..r][0..c]; the zero row and column in front
        // let every region read four entries with no edge cases. LeetCode guarantees at least one
        // row, and its bounds keep every sum under 200 * 200 * 10^4, inside an int.
        private readonly int[,] _prefix;

        public PrefixSumMatrix(int[][] matrix)
        {
            var rows = matrix.Length;
            var cols = matrix[0].Length;
            _prefix = new int[rows + 1, cols + 1];

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    _prefix[row + 1, col + 1] =
                        matrix[row][col] + _prefix[row, col + 1] + _prefix[row + 1, col] - _prefix[row, col];
                }
            }
        }

        public int SumRegion(int row1, int col1, int row2, int col2) =>
            _prefix[row2 + 1, col2 + 1] - _prefix[row1, col2 + 1] - _prefix[row2 + 1, col1] + _prefix[row1, col1];
    }
}
