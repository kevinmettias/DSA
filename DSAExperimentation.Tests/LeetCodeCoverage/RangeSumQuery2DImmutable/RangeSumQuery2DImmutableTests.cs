using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RangeSumQuery2DImmutable;

// LeetCode 304. Range Sum Query 2D - Immutable: no new 2D data structure - one of this repo's own
// FenwickTree<int,SumOperation<int>> per row, the same "compose the existing 1D primitive
// multiple times" move RangeFenwickTree.cs already makes with two FenwickTree instances.
// SumRegion sums each covered row's O(log cols) FenwickTree.Query(col1, col2) across
// [row1, row2], instead of rescanning every cell in the region.
public sealed class RangeSumQuery2DImmutableTests
{
    private static readonly int[][] ExampleMatrix =
    [
        [3, 0, 1, 4, 2],
        [5, 6, 3, 2, 1],
        [1, 2, 0, 1, 5],
        [4, 1, 0, 1, 7],
        [1, 0, 3, 0, 5],
    ];

    [Fact]
    public void SumRegion_LeetCodeExample_ReturnsExpectedSums()
    {
        var numMatrix = new NumMatrixOperations(ExampleMatrix);

        var firstRegion = numMatrix.SumRegion(2, 1, 4, 3);
        var secondRegion = numMatrix.SumRegion(1, 1, 2, 2);
        var thirdRegion = numMatrix.SumRegion(1, 2, 2, 4);

        Assert.Equal(8, firstRegion);
        Assert.Equal(11, secondRegion);
        Assert.Equal(12, thirdRegion);
    }

    [Fact]
    public void SumRegion_SingleCell_ReturnsThatCell()
    {
        var numMatrix = new NumMatrixOperations(ExampleMatrix);

        var region = numMatrix.SumRegion(1, 1, 1, 1);

        Assert.Equal(6, region);
    }

    private sealed class NumMatrixOperations
    {
        private readonly FenwickTree<int, SumOperation<int>>[] _rows;

        public NumMatrixOperations(int[][] matrix)
            => _rows = matrix.Select(row => new FenwickTree<int, SumOperation<int>>(row)).ToArray();

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
