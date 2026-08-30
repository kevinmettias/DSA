using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Shift2DGrid;

// LeetCode 1260. Shift 2D Grid: flatten the grid in row-major order, right-rotate
// it k times using this repo's own Deque<int> (TryPopBack + PushFront is exactly
// one right-shift step - the last element wraps around to the front), then reshape
// the rotated sequence back into rows x cols.
public sealed partial class Shift2DGridTests
{
    [Fact]
    public void ShiftGrid_ClassicExample_ShiftsElementsByK()
    {
        int[][] grid = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];

        var shifted = ShiftGrid(grid, k: 1);

        Assert.Equal([[9, 1, 2], [3, 4, 5], [6, 7, 8]], shifted);
    }

    [Fact]
    public void ShiftGrid_KLargerThanGridSize_WrapsAroundUsingModulo()
    {
        int[][] grid = [[1, 2], [3, 4]];

        var shifted = ShiftGrid(grid, k: 6);

        Assert.Equal([[3, 4], [1, 2]], shifted);
    }

    private static int[][] ShiftGrid(int[][] grid, int k)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var total = rows * cols;

        var deque = new RepoDeque();

        foreach (var row in grid)
        {
            foreach (var value in row)
            {
                deque.PushBack(value);
            }
        }

        var shifts = k % total;

        for (var i = 0; i < shifts; i++)
        {
            deque.TryPopBack(out var last);
            deque.PushFront(last);
        }

        var result = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            result[r] = new int[cols];

            for (var c = 0; c < cols; c++)
            {
                deque.TryPopFront(out result[r][c]);
            }
        }

        return result;
    }
}
