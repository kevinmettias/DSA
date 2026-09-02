using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.RangeSumQuery2DImmutable;

// LeetCode 304. Range Sum Query 2D - Immutable: given an immutable matrix, answer repeated
// SumRegion(row1, col1, row2, col2) queries efficiently.
//
// This is a design problem - LeetCode's own shape is a stateful object (a constructor plus a
// SumRegion operation), not a single return value - the same shape MinStackSolution and
// LRUCacheSolution use for their own design problems. The composed strategy needs no new 2D data
// structure: one of this repo's own FenwickTree<int, SumOperation<int>> per row, the same "compose
// the existing 1D primitive multiple times" move RangeFenwickTree.cs already makes with two
// FenwickTree instances. SumRegion then sums each covered row's O(log cols)
// FenwickTree.Query(col1, col2) across [row1, row2], instead of rescanning every cell in the
// region the way the baseline arm does.
internal static class RangeSumQuery2DImmutableSolution
{
    // The textbook baseline this composition has to justify itself against: no preprocessing at
    // all, just an O(rows*cols) scan of the raw matrix per SumRegion call.
    public static INumMatrix CreateByBruteForceCellScan(int[][] matrix) => new BruteForceCellScanNumMatrix(matrix);

    // The composed answer: one FenwickTree per row, built once so each SumRegion call afterward
    // walks only the covered rows and does an O(log cols) query per row.
    public static INumMatrix CreateByRowFenwickTree(int[][] matrix) => new RowFenwickTreeNumMatrix(matrix);

    private sealed class BruteForceCellScanNumMatrix(int[][] matrix) : INumMatrix
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

    private sealed class RowFenwickTreeNumMatrix : INumMatrix
    {
        private readonly FenwickTree<int, SumOperation<int>>[] _rows;

        public RowFenwickTreeNumMatrix(int[][] matrix) =>
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
}

// LeetCode's own NumMatrix operation, common to both strategies so a harness can hold either
// behind one type. Scoped to this problem alone - nothing else in the repo answers a 2D range-sum
// query.
internal interface INumMatrix
{
    int SumRegion(int row1, int col1, int row2, int col2);
}
