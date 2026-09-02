using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestIncreasingPathInAMatrix;

// LeetCode 329. Longest Increasing Path in a Matrix: Memoizer caches, per cell, the
// longest strictly-increasing path starting there - the same (Row,Col)-state grid
// recurrence UniquePaths/EditDistance already use, closed over four-directional
// neighbors instead of just right/down. The answer is the max over every cell as a
// candidate start, which is safe to memoize at all: a strictly-increasing value
// relation can never cycle back to a cell already on the path, so the induced
// relation is a DAG with no cycle for Memoizer's well-founded-state precondition to
// trip over.
public sealed partial class LongestIncreasingPathInAMatrixTests
{
    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Fact]
    public void LongestPath_ThreeByThreeMatrix_ReturnsFourStepClimb()
    {
        int[,] matrix =
        {
            { 9, 9, 4 },
            { 6, 6, 8 },
            { 2, 1, 1 },
        };

        Assert.Equal(4, LongestIncreasingPath(matrix));
    }

    [Fact]
    public void LongestPath_MatrixWithFlatRegion_ReturnsFourStepClimb()
    {
        int[,] matrix =
        {
            { 3, 4, 5 },
            { 3, 2, 6 },
            { 2, 2, 1 },
        };

        Assert.Equal(4, LongestIncreasingPath(matrix));
    }

    [Fact]
    public void LongestPath_SingleCell_ReturnsOne()
    {
        int[,] matrix = { { 7 } };

        Assert.Equal(1, LongestIncreasingPath(matrix));
    }

    private readonly record struct MatrixGrid(int[,] Matrix, int Rows, int Cols);

    private static int LongestIncreasingPath(int[,] matrix)
    {
        var grid = new MatrixGrid(matrix, matrix.GetLength(0), matrix.GetLength(1));

        return ComputeLongestPath(grid);
    }

    private static int ComputeLongestPath(MatrixGrid grid)
    {
        var longest = 0;

        for (var row = 0; row < grid.Rows; row++)
        {
            for (var col = 0; col < grid.Cols; col++)
            {
                var pathLength = Memoizer.Memoize<(int Row, int Col), int>(
                    (row, col), (state, lengthFrom) => LengthFrom(grid, state, lengthFrom));
                longest = Math.Max(longest, pathLength);
            }
        }

        return longest;
    }

    private static int LengthFrom(
        MatrixGrid grid, (int Row, int Col) state, Func<(int Row, int Col), int> lengthFrom)
    {
        var best = 1;

        foreach (var (rowOffset, colOffset) in Directions)
        {
            var nextRow = state.Row + rowOffset;
            var nextCol = state.Col + colOffset;

            if (nextRow >= 0 && nextRow < grid.Rows && nextCol >= 0 && nextCol < grid.Cols
                && grid.Matrix[nextRow, nextCol] > grid.Matrix[state.Row, state.Col])
            {
                best = Math.Max(best, 1 + lengthFrom((nextRow, nextCol)));
            }
        }

        return best;
    }
}
