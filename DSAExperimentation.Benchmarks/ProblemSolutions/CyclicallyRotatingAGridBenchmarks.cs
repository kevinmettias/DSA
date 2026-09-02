using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Deque;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Cyclically Rotating a Grid (LC 1914): k can be up to 1e9 while a ring's own length
// tops out around 2*(Size-1), so looping the ring "one step at a time, k times" (a
// natural first-draft mistake) is O(k) per ring no matter how few cells are in it.
// This repo's own Deque<T> makes a single rotation step O(1) either way - the
// asymmetry benchmarked here is purely the missing `k % ringLength` reduction, not a
// different data structure, echoing ARCHITECTURE.md's point (Sec. 8) that a
// primitive's own cost contract doesn't save an algorithm that fails to use it well.
[MemoryDiagnoser]
public class CyclicallyRotatingAGridBenchmarks
{
    private const int UnreducedSteps = 1_000_003;

    // A ring layer consumes one row/column off each opposing side of the shorter
    // dimension, so the layer count is half of it.
    private const int SidesPerRingLayer = 2;

    [Params(10, 40)]
    public int Size;

    private int[][] _template = null!;

    [GlobalSetup]
    public void Setup()
    {
        _template = new int[Size][];
        var value = 0;

        for (var row = 0; row < Size; row++)
        {
            _template[row] = new int[Size];
            for (var col = 0; col < Size; col++)
            {
                _template[row][col] = value++;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] BruteForce() => RotateGrid(CloneTemplate(), reduceStepsByRingLength: false);

    [Benchmark]
    public int[][] DequeRotation() => RotateGrid(CloneTemplate(), reduceStepsByRingLength: true);

    private int[][] CloneTemplate()
    {
        var grid = new int[Size][];
        for (var row = 0; row < Size; row++)
        {
            grid[row] = (int[])_template[row].Clone();
        }

        return grid;
    }

    private static int[][] RotateGrid(int[][] grid, bool reduceStepsByRingLength)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var layers = Math.Min(rows, cols) / SidesPerRingLayer;

        for (var layer = 0; layer < layers; layer++)
        {
            var ringCells = CollectRingCells(rows, cols, layer);
            RotateLayer(grid, ringCells, reduceStepsByRingLength);
        }

        return grid;
    }

    private static List<(int Row, int Col)> CollectRingCells(int rows, int cols, int layer)
    {
        var bounds = new RingBounds(layer, rows - 1 - layer, layer, cols - 1 - layer);
        var cells = new List<(int Row, int Col)>();

        AddTopEdge(cells, bounds);
        AddRightEdge(cells, bounds);
        AddBottomEdge(cells, bounds);
        AddLeftEdge(cells, bounds);

        return cells;
    }

    private static void AddTopEdge(List<(int Row, int Col)> cells, RingBounds bounds)
    {
        for (var c = bounds.Left; c <= bounds.Right; c++)
        {
            cells.Add((bounds.Top, c));
        }
    }

    private static void AddRightEdge(List<(int Row, int Col)> cells, RingBounds bounds)
    {
        for (var r = bounds.Top + 1; r <= bounds.Bottom; r++)
        {
            cells.Add((r, bounds.Right));
        }
    }

    private static void AddBottomEdge(List<(int Row, int Col)> cells, RingBounds bounds)
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

    private static void AddLeftEdge(List<(int Row, int Col)> cells, RingBounds bounds)
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

    private static void RotateLayer(int[][] grid, List<(int Row, int Col)> cells, bool reduceStepsByRingLength)
    {
        var ring = new Deque<int>();
        foreach (var (row, col) in cells)
        {
            ring.PushBack(grid[row][col]);
        }

        var steps = reduceStepsByRingLength ? UnreducedSteps % cells.Count : UnreducedSteps;
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

    private readonly record struct RingBounds(int Top, int Bottom, int Left, int Right);
}
