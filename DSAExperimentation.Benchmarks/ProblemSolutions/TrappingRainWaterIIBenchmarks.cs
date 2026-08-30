using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Trapping Rain Water II (LC 407): RelaxationSweep is the Bellman-Ford-shaped
// alternative - repeated whole-grid relax passes toward a fixed point, no
// priority ordering at all - vs. HeapFloodFill, the Dijkstra-shaped boundary
// flood-fill that composes this repo's own Heap<Element,TOrder> ordered by
// ByPriorityOrder<TNode,TWeight>, the exact frontier shape ShortestPath.cs's
// own SearchState.Queue already uses. Same Bellman-Ford-vs-Dijkstra
// complexity contrast ARCHITECTURE.md documents for 1D shortest paths, here
// for a 2D grid.
[MemoryDiagnoser]
public class TrappingRainWaterIIBenchmarks
{
    [Params(15, 40)]
    public int Size;

    private int[][] _heightMap = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(407);
        _heightMap = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, 50)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RelaxationSweep()
    {
        var rows = _heightMap.Length;
        var cols = _heightMap[0].Length;
        var water = new int[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                water[r, c] = IsBoundary(r, c, rows, cols) ? _heightMap[r][c] : int.MaxValue;
            }
        }

        var changed = true;

        while (changed)
        {
            changed = false;

            for (var r = 1; r < rows - 1; r++)
            {
                for (var c = 1; c < cols - 1; c++)
                {
                    var floor = _heightMap[r][c];
                    var best = water[r, c];

                    best = Math.Min(best, Math.Max(floor, water[r - 1, c]));
                    best = Math.Min(best, Math.Max(floor, water[r + 1, c]));
                    best = Math.Min(best, Math.Max(floor, water[r, c - 1]));
                    best = Math.Min(best, Math.Max(floor, water[r, c + 1]));

                    if (best < water[r, c])
                    {
                        water[r, c] = best;
                        changed = true;
                    }
                }
            }
        }

        var total = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                total += water[r, c] - _heightMap[r][c];
            }
        }

        return total;
    }

    [Benchmark]
    public int HeapFloodFill()
    {
        var rows = _heightMap.Length;
        var cols = _heightMap[0].Length;
        var visited = new bool[rows, cols];
        var boundary = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (IsBoundary(r, c, rows, cols))
                {
                    boundary.Push(((r, c), _heightMap[r][c]));
                    visited[r, c] = true;
                }
            }
        }

        (int Row, int Col)[] directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];
        var water = 0;

        while (boundary.TryPop(out var entry))
        {
            var (row, col) = entry.Node;
            var height = entry.Priority;

            foreach (var (dr, dc) in directions)
            {
                var nr = row + dr;
                var nc = col + dc;

                if (nr < 0 || nr >= rows || nc < 0 || nc >= cols || visited[nr, nc])
                {
                    continue;
                }

                visited[nr, nc] = true;
                var neighborHeight = _heightMap[nr][nc];
                water += Math.Max(0, height - neighborHeight);
                boundary.Push(((nr, nc), Math.Max(height, neighborHeight)));
            }
        }

        return water;
    }

    private static bool IsBoundary(int r, int c, int rows, int cols)
        => r == 0 || r == rows - 1 || c == 0 || c == cols - 1;
}
