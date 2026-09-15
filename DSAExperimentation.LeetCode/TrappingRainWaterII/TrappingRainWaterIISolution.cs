using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.TrappingRainWaterII;

// LeetCode 407. Trapping Rain Water II: total water volume a 2D elevation map
// traps, given every interior cell can only hold as much water as the lowest
// wall on every path out to the boundary allows.
//
// Both strategies compute the same fixed point - "each cell's water level is
// the smallest, over every path to the boundary, of the tallest obstacle on
// that path" - the classic min-max/bottleneck-path characterization. They
// differ only in how they reach it: RelaxationSweep is the Bellman-Ford-shaped
// alternative - repeated whole-grid relax passes with no priority ordering at
// all - while HeapFloodFill is the Dijkstra-shaped boundary flood-fill that
// composes this repo's own Heap<Element,TOrder>, ordered by
// ByPriorityOrder<TNode,TWeight> - the exact frontier shape ShortestPath.cs's
// own SearchState.Queue already uses, with TNode a (row, col) cell and TWeight
// the water level reached so far. Same Bellman-Ford-vs-Dijkstra complexity
// contrast ARCHITECTURE.md documents for 1D shortest paths, here for a 2D grid.
internal static class TrappingRainWaterIISolution
{
    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // Deliberately written without this repo's primitives - a plain int[,]
    // relaxed to a fixed point is what you would write without this repo.
    public static int TrapRainWaterByRelaxationSweep(int[][] heightMap)
    {
        var rows = heightMap.Length;
        var cols = heightMap[0].Length;

        if (rows < 3 || cols < 3)
        {
            return 0;
        }

        var water = InitializeWaterLevels(heightMap, rows, cols);

        RelaxToFixedPoint(heightMap, water, rows, cols);

        return SumTrappedWater(heightMap, water, rows, cols);
    }

    private static int[,] InitializeWaterLevels(int[][] heightMap, int rows, int cols)
    {
        var water = new int[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                water[r, c] = IsBoundary(r, c, rows, cols) ? TerrainHeight(heightMap, r, c) : int.MaxValue;
            }
        }

        return water;
    }

    // A cell's own terrain height - the level a boundary cell seeds the flood at, since
    // nothing can wall a boundary cell in.
    private static int TerrainHeight(int[][] heightMap, int row, int col) => heightMap[row][col];

    // Repeat whole-grid relax passes until a pass changes nothing: the fixed point
    // is reached once every interior cell already equals the lowest wall on its
    // cheapest way out to the boundary. No priority ordering at all - the
    // Bellman-Ford shape, against the heap flood fill's Dijkstra shape below.
    private static void RelaxToFixedPoint(int[][] heightMap, int[,] water, int rows, int cols)
    {
        var changed = true;

        while (changed)
        {
            changed = false;

            for (var r = 1; r < rows - 1; r++)
            {
                for (var c = 1; c < cols - 1; c++)
                {
                    changed = RelaxCell(heightMap, r, c, water) || changed;
                }
            }
        }
    }

    private static bool RelaxCell(int[][] heightMap, int r, int c, int[,] water)
    {
        var floor = heightMap[r][c];
        var best = water[r, c];

        var north = Math.Max(floor, water[r - 1, c]);
        best = Math.Min(best, north);

        var south = Math.Max(floor, water[r + 1, c]);
        best = Math.Min(best, south);

        var west = Math.Max(floor, water[r, c - 1]);
        best = Math.Min(best, west);

        var east = Math.Max(floor, water[r, c + 1]);
        best = Math.Min(best, east);

        if (best >= water[r, c])
        {
            return false;
        }

        water[r, c] = best;
        return true;
    }

    private static int SumTrappedWater(int[][] heightMap, int[,] water, int rows, int cols)
    {
        var total = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                total += water[r, c] - heightMap[r][c];
            }
        }

        return total;
    }

    // Every boundary cell seeds the heap at its own height; popping the
    // globally lowest wall first guarantees each interior cell is first
    // reached at the true water level that actually floods it (never a higher
    // one) - the same settle-once invariant Dijkstra's own first-pop-is-final
    // argument relies on.
    public static int TrapRainWaterByHeapFloodFill(int[][] heightMap)
    {
        var rows = heightMap.Length;
        var cols = heightMap[0].Length;

        if (rows < 3 || cols < 3)
        {
            return 0;
        }

        var state = SeedBoundary(heightMap, rows, cols);

        return FloodFill(state);
    }

    private static FloodState SeedBoundary(int[][] heightMap, int rows, int cols)
    {
        var state = new FloodState(heightMap, new bool[rows, cols], new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>());

        PopulateBoundary(state, rows, cols);

        return state;
    }

    private static void PopulateBoundary(FloodState state, int rows, int cols)
    {
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (IsBoundary(r, c, rows, cols))
                {
                    state.Boundary.Push(((r, c), state.HeightMap[r][c]));
                    state.Visited[r, c] = true;
                }
            }
        }
    }

    private static int FloodFill(FloodState state)
    {
        var water = 0;

        while (state.Boundary.TryPop(out var entry))
        {
            water += ExploreNeighbors(entry, state);
        }

        return water;
    }

    private static int ExploreNeighbors(((int Row, int Col) Node, int Priority) entry, FloodState state)
    {
        var (row, col) = entry.Node;
        var height = entry.Priority;
        var addedWater = 0;

        foreach (var (dr, dc) in Directions)
        {
            addedWater += VisitNeighbor((row + dr, col + dc), height, state);
        }

        return addedWater;
    }

    private static int VisitNeighbor((int Row, int Col) neighbor, int height, FloodState state)
    {
        if (IsNeighborBlocked(state, neighbor))
        {
            return 0;
        }

        state.Visited[neighbor.Row, neighbor.Col] = true;
        var neighborHeight = state.HeightMap[neighbor.Row][neighbor.Col];
        state.Boundary.Push((neighbor, Math.Max(height, neighborHeight)));

        return Math.Max(0, height - neighborHeight);
    }

    // Whether a neighbor is one the flood has already settled or would fall off the
    // board - the board's own extent is read here rather than threaded in from above.
    private static bool IsNeighborBlocked(FloodState state, (int Row, int Col) neighbor)
    {
        var rows = state.HeightMap.Length;
        var cols = state.HeightMap[0].Length;

        return IsOffBoard(neighbor.Row, neighbor.Col, rows, cols)
            || state.Visited[neighbor.Row, neighbor.Col];
    }

    // Both coordinates outside the map is one idea, and the flood asks it of every
    // neighbor before anything else.
    private static bool IsOffBoard(int row, int col, int rows, int cols)
        => row < 0 || row >= rows || col < 0 || col >= cols;

    private static bool IsBoundary(int r, int c, int rows, int cols)
        => r == 0 || r == rows - 1 || c == 0 || c == cols - 1;

    private sealed record FloodState(
        int[][] HeightMap,
        bool[,] Visited,
        Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> Boundary);
}
