using DSAExperimentation.DataStructures.Deque;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CyclicallyRotatingAGrid;

// LeetCode 1914. Cyclically Rotating a Grid: each concentric layer's boundary cells,
// walked in clockwise order (SpiralMatrix's own boundary-walk shape), form one ring
// that has to rotate independently of every other ring. This repo's own Deque<T>
// rotates that ring with O(1) TryPopFront/PushBack per step - the same "sequence
// with both-ends access" primitive RotateImage's row reversal composes over Stack<T>,
// just needed at both ends here instead of one.
public sealed partial class CyclicallyRotatingAGridTests
{
    [Fact]
    public void RotateGrid_TwoByTwoGridRotatedByOne_MatchesLeetCodeExample()
    {
        int[][] grid = [[40, 10], [30, 20]];

        var rotated = RotateGrid(grid, k: 1);

        Assert.Equal([[10, 20], [40, 30]], rotated);
    }

    [Fact]
    public void RotateGrid_ThreeByThreeGridRotatedByTwo_LeavesCenterUntouched()
    {
        int[][] grid = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];

        var rotated = RotateGrid(grid, k: 2);

        Assert.Equal([[3, 6, 9], [2, 5, 8], [1, 4, 7]], rotated);
    }

    private static int[][] RotateGrid(int[][] grid, int k)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var layers = Math.Min(rows, cols) / 2;

        for (var layer = 0; layer < layers; layer++)
        {
            var ringCells = CollectRingCells(rows, cols, layer);
            RotateLayer(grid, ringCells, k);
        }

        return grid;
    }

    private readonly record struct RingBounds(int Top, int Bottom, int Left, int Right);

    // Standard clockwise boundary walk (top row left->right, right column
    // top->bottom, bottom row right->left, left column bottom->top), the same shape
    // SpiralMatrixTests already uses - just bounded to one fixed layer instead of
    // shrinking bounds across the whole matrix.
    private static List<(int Row, int Col)> CollectRingCells(int rows, int cols, int layer)
    {
        var bounds = new RingBounds(layer, rows - 1 - layer, layer, cols - 1 - layer);
        var cells = new List<(int Row, int Col)>();

        AppendTopRow(cells, bounds);
        AppendRightColumn(cells, bounds);
        AppendBottomRow(cells, bounds);
        AppendLeftColumn(cells, bounds);

        return cells;
    }

    private static void AppendTopRow(List<(int Row, int Col)> cells, RingBounds bounds)
    {
        for (var c = bounds.Left; c <= bounds.Right; c++)
        {
            cells.Add((bounds.Top, c));
        }
    }

    private static void AppendRightColumn(List<(int Row, int Col)> cells, RingBounds bounds)
    {
        for (var r = bounds.Top + 1; r <= bounds.Bottom; r++)
        {
            cells.Add((r, bounds.Right));
        }
    }

    private static void AppendBottomRow(List<(int Row, int Col)> cells, RingBounds bounds)
    {
        if (bounds.Bottom <= bounds.Top)
        {
            return;
        }

        for (var c = bounds.Right - 1; c >= bounds.Left; c--)
        {
            cells.Add((bounds.Bottom, c));
        }
    }

    private static void AppendLeftColumn(List<(int Row, int Col)> cells, RingBounds bounds)
    {
        if (bounds.Right <= bounds.Left)
        {
            return;
        }

        for (var r = bounds.Bottom - 1; r > bounds.Top; r--)
        {
            cells.Add((r, bounds.Left));
        }
    }

    private static void RotateLayer(int[][] grid, List<(int Row, int Col)> cells, int k)
    {
        var ring = new Deque<int>();
        foreach (var (row, col) in cells)
        {
            ring.PushBack(grid[row][col]);
        }

        var steps = k % cells.Count;
        for (var i = 0; i < steps; i++)
        {
            ring.TryPopFront(out var moved);
            ring.PushBack(moved);
        }

        foreach (var (row, col) in cells)
        {
            ring.TryPopFront(out var value);
            grid[row][col] = value;
        }
    }
}
