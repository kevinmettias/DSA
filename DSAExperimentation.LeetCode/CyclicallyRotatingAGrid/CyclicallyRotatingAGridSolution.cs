using DSAExperimentation.DataStructures.Deque;

namespace DSAExperimentation.LeetCode.CyclicallyRotatingAGrid;

// LeetCode 1914. Cyclically Rotating a Grid: every concentric layer's boundary
// cells form one ring, and each ring rotates counter-clockwise by k independently
// of every other ring.
//
// Both strategies share the same decomposition - walk a layer's boundary clockwise
// into a flat cell list, rotate the values along it, write them back in the same
// order - because that geometry is the problem, not a strategy. No repo primitive
// models it: Grid/GridTopology describe unordered orthogonal adjacency for graph
// walks, not a fixed clockwise ring order, the same reason SpiralMatrixSolution
// keeps its own boundary walk.
//
// What separates the two arms is only how far each rotates. k reaches 1e9 while a
// ring's own length tops out near 2 * (rows + cols), so stepping one position at a
// time k times is O(k) per ring however few cells it holds - the natural
// first-draft mistake - while reducing k modulo the ring length first is O(ring).
internal static class CyclicallyRotatingAGridSolution
{
    // A ring layer consumes one row/column off each opposing side of the shorter
    // dimension, so the layer count is half of it.
    private const int SidesPerRingLayer = 2;

    // The textbook simulation: a BCL Queue rotated one position at a time, k times,
    // with no reduction against the ring's own length. Deliberately written without
    // this repo's primitives - it is the arm the deque strategy below has to justify
    // itself against, and Queue<int> is a circular buffer just like Deque<T>'s, so
    // the gap the comparison shows is the missing reduction rather than a change of
    // container.
    public static int[][] RotateGridByStepwiseQueue(int[][] grid, int k)
    {
        var rotated = CopyOf(grid);

        foreach (var cells in RingsOf(rotated))
        {
            var ring = LoadQueue(rotated, cells);
            StepFrontToBack(ring, k);
            WriteBack(rotated, cells, ring);
        }

        return rotated;
    }

    private static Queue<int> LoadQueue(int[][] grid, List<(int Row, int Col)> cells)
    {
        var ring = new Queue<int>();

        foreach (var (row, col) in cells)
        {
            ring.Enqueue(grid[row][col]);
        }

        return ring;
    }

    private static void StepFrontToBack(Queue<int> ring, int steps)
    {
        for (var step = 0; step < steps; step++)
        {
            var moved = ring.Dequeue();
            ring.Enqueue(moved);
        }
    }

    private static void WriteBack(int[][] grid, List<(int Row, int Col)> cells, Queue<int> ring)
    {
        foreach (var (row, col) in cells)
        {
            grid[row][col] = ring.Dequeue();
        }
    }

    // This repo's own Deque<T> holds the ring, and k is reduced modulo the ring's
    // length before any value moves - so each ring costs one pass to load, at most
    // ringLength - 1 O(1) TryPopFront/PushBack steps, and one pass to write back.
    public static int[][] RotateGridByDequeRings(int[][] grid, int k)
    {
        var rotated = CopyOf(grid);

        foreach (var cells in RingsOf(rotated))
        {
            var ring = LoadDeque(rotated, cells);
            RotateFrontToBack(ring, k % cells.Count);
            WriteBack(rotated, cells, ring);
        }

        return rotated;
    }

    private static Deque<int> LoadDeque(int[][] grid, List<(int Row, int Col)> cells)
    {
        var ring = new Deque<int>();

        foreach (var (row, col) in cells)
        {
            ring.PushBack(grid[row][col]);
        }

        return ring;
    }

    private static void RotateFrontToBack(Deque<int> ring, int steps)
    {
        for (var step = 0; step < steps; step++)
        {
            ring.TryPopFront(out var moved);
            ring.PushBack(moved);
        }
    }

    private static void WriteBack(int[][] grid, List<(int Row, int Col)> cells, Deque<int> ring)
    {
        foreach (var (row, col) in cells)
        {
            ring.TryPopFront(out var value);
            grid[row][col] = value;
        }
    }

    private static int[][] CopyOf(int[][] grid)
    {
        var copy = new int[grid.Length][];

        for (var row = 0; row < grid.Length; row++)
        {
            copy[row] = (int[])grid[row].Clone();
        }

        return copy;
    }

    // Outermost layer first; the innermost layer of an odd-sided grid is the single
    // centre cell, which has no ring and stays where it is.
    private static IEnumerable<List<(int Row, int Col)>> RingsOf(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var layers = Math.Min(rows, cols) / SidesPerRingLayer;

        for (var layer = 0; layer < layers; layer++)
        {
            yield return CollectRingCells(rows, cols, layer);
        }
    }

    // Standard clockwise boundary walk - top row left to right, right column top to
    // bottom, bottom row right to left, left column bottom to top - bounded to one
    // fixed layer instead of shrinking across the whole matrix.
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
        for (var col = bounds.Left; col <= bounds.Right; col++)
        {
            cells.Add((bounds.Top, col));
        }
    }

    private static void AppendRightColumn(List<(int Row, int Col)> cells, RingBounds bounds)
    {
        for (var row = bounds.Top + 1; row <= bounds.Bottom; row++)
        {
            cells.Add((row, bounds.Right));
        }
    }

    private static void AppendBottomRow(List<(int Row, int Col)> cells, RingBounds bounds)
    {
        if (bounds.Bottom <= bounds.Top)
        {
            return;
        }

        for (var col = bounds.Right - 1; col >= bounds.Left; col--)
        {
            cells.Add((bounds.Bottom, col));
        }
    }

    private static void AppendLeftColumn(List<(int Row, int Col)> cells, RingBounds bounds)
    {
        if (bounds.Right <= bounds.Left)
        {
            return;
        }

        for (var row = bounds.Bottom - 1; row > bounds.Top; row--)
        {
            cells.Add((row, bounds.Left));
        }
    }

    private readonly record struct RingBounds(int Top, int Bottom, int Left, int Right);
}
