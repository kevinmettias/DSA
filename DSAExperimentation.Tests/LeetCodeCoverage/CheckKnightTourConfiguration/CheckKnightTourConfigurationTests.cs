namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckKnightTourConfiguration;

// LeetCode 2596. Check Knight Tour Configuration: build an inverse position lookup
// (order -> cell) with one full-board scan, then walk consecutive orders checking
// each pair is a knight's-move apart. No repo Representation/Operations primitive
// applies to either step - the same "fixed-shape grid index arithmetic, not a
// Representation to abstract over" call AvailableCapturesForRookTests/
// TransposeMatrix already make; Grid/GridChildren models unordered single-step
// ORTHOGONAL adjacency for graph walks, not the two-over-one-across offsets a
// knight makes, so forcing this through Grid/** would not be a genuine fit.
public sealed class CheckKnightTourConfigurationTests
{
    private static readonly (int DRow, int DCol)[] KnightOffsets =
        [(1, 2), (1, -2), (-1, 2), (-1, -2), (2, 1), (2, -1), (-2, 1), (-2, -1)];

    [Fact]
    public void ValidTour_SingleCellBoard_ReturnsTrue()
    {
        int[][] grid = [[0]];

        Assert.True(ValidTour(grid));
    }

    [Fact]
    public void ValidTour_RowMajorOrderOnThreeByThreeBoard_ReturnsFalse()
    {
        int[][] grid =
        [
            [0, 1, 2],
            [3, 4, 5],
            [6, 7, 8],
        ];

        Assert.False(ValidTour(grid));
    }

    [Fact]
    public void ValidTour_GenuineFiveByFiveKnightsTour_ReturnsTrue()
    {
        int[][] grid =
        [
            [0, 13, 18, 7, 24],
            [5, 8, 1, 12, 17],
            [14, 19, 6, 23, 2],
            [9, 4, 21, 16, 11],
            [20, 15, 10, 3, 22],
        ];

        Assert.True(ValidTour(grid));
    }

    [Fact]
    public void ValidTour_SameTourWithTwoOrdersSwapped_ReturnsFalse()
    {
        int[][] grid =
        [
            [0, 13, 18, 24, 7],
            [5, 8, 1, 12, 17],
            [14, 19, 6, 23, 2],
            [9, 4, 21, 16, 11],
            [20, 15, 10, 3, 22],
        ];

        Assert.False(ValidTour(grid));
    }

    private static bool ValidTour(int[][] grid)
    {
        var n = grid.Length;
        var positionByOrder = new (int Row, int Col)[n * n];

        for (var row = 0; row < n; row++)
        {
            for (var col = 0; col < n; col++)
            {
                positionByOrder[grid[row][col]] = (row, col);
            }
        }

        for (var order = 0; order < (n * n) - 1; order++)
        {
            if (!IsKnightMove(positionByOrder[order], positionByOrder[order + 1]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsKnightMove((int Row, int Col) from, (int Row, int Col) to)
    {
        foreach (var (dRow, dCol) in KnightOffsets)
        {
            if (from.Row + dRow == to.Row && from.Col + dCol == to.Col)
            {
                return true;
            }
        }

        return false;
    }
}
