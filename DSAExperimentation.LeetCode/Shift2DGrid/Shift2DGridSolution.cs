using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.Shift2DGrid;

// LeetCode 1260. Shift 2D Grid: shift every cell shiftCount places forward in
// row-major order, the last cell of the last row wrapping around to the first cell
// of the first row.
//
// Both strategies see the grid as one flat, circular sequence of rows*cols cells
// and differ only in how they express the rotation: destination index arithmetic
// straight into a fresh array, or this repo's own Deque<int>, where TryPopBack +
// PushFront *is* one right-shift step.
internal static class Shift2DGridSolution
{
    // The textbook answer: compute each source cell's shifted (row, col)
    // destination and write it straight into a fresh grid. Deliberately written
    // without this repo's primitives - it is the arm the composed solution below
    // has to justify itself against, and it never touches more than one cell per
    // cell regardless of shiftCount.
    public static int[][] ShiftGridByIndexArithmetic(int[][] grid, int shiftCount)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var total = rows * cols;
        var result = NewGrid(rows, cols);

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var flatIndex = ((r * cols) + c + shiftCount) % total;
                result[flatIndex / cols][flatIndex % cols] = grid[r][c];
            }
        }

        return result;
    }

    private static int[][] NewGrid(int rows, int cols)
    {
        var result = new int[rows][];

        for (var r = 0; r < rows; r++)
        {
            result[r] = new int[cols];
        }

        return result;
    }

    // This repo's own Deque<int>: flatten the grid into it in row-major order,
    // right-rotate it shiftCount mod (rows*cols) times - each step pops the last
    // element and pushes it to the front, which is exactly one shift - then drain it
    // back out into the grid shape.
    public static int[][] ShiftGridByDequeRotation(int[][] grid, int shiftCount)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var total = rows * cols;

        var deque = Flatten(grid);
        RotateRight(deque, shiftCount % total);

        return Reshape(deque, rows, cols);
    }

    private static RepoDeque Flatten(int[][] grid)
    {
        var deque = new RepoDeque();

        foreach (var row in grid)
        {
            foreach (var value in row)
            {
                deque.PushBack(value);
            }
        }

        return deque;
    }

    // The last element wraps around to the front - one right-shift step.
    private static void RotateRight(RepoDeque deque, int shifts)
    {
        for (var i = 0; i < shifts; i++)
        {
            deque.TryPopBack(out var last);
            deque.PushFront(last);
        }
    }

    private static int[][] Reshape(RepoDeque deque, int rows, int cols)
    {
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
