using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TrappingRainWaterII;

// LeetCode 407. Trapping Rain Water II: a Dijkstra-shaped boundary flood-fill
// over this repo's own Heap<Element,TOrder>, ordered by
// ByPriorityOrder<TNode,TWeight> - the exact frontier shape ShortestPath.cs's
// own SearchState.Queue field already uses, just with TNode as a (row, col)
// cell and TWeight as the water level reached so far. Every boundary cell
// seeds the heap at its own height; popping the globally lowest wall first
// guarantees each interior cell is first reached at the true water level that
// actually floods it (never a higher one) - the same settle-once invariant
// Dijkstra's own first-pop-is-final argument relies on.
public sealed partial class TrappingRainWaterIITests
{
    [Fact]
    public void TrapRainWater_ClassicExample_ReturnsTotalTrappedVolume()
    {
        int[][] heightMap =
        [
            [1, 4, 3, 1, 3, 2],
            [3, 2, 1, 3, 2, 4],
            [2, 3, 3, 2, 3, 1],
        ];

        Assert.Equal(4, TrapRainWater(heightMap));
    }

    [Fact]
    public void TrapRainWater_BasinSurroundedByTallerWalls_ReturnsFilledVolume()
    {
        int[][] heightMap =
        [
            [3, 3, 3, 3, 3],
            [3, 2, 2, 2, 3],
            [3, 2, 1, 2, 3],
            [3, 2, 2, 2, 3],
            [3, 3, 3, 3, 3],
        ];

        Assert.Equal(10, TrapRainWater(heightMap));
    }

    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    private static int TrapRainWater(int[][] heightMap)
    {
        var rows = heightMap.Length;
        var cols = heightMap[0].Length;

        if (rows < 3 || cols < 3)
        {
            return 0;
        }

        var state = SeedBoundary(heightMap);

        return FloodFill(state);
    }

    private static FloodState SeedBoundary(int[][] heightMap)
    {
        var rows = heightMap.Length;
        var cols = heightMap[0].Length;
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
                if (r == 0 || r == rows - 1 || c == 0 || c == cols - 1)
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
        var rows = state.HeightMap.Length;
        var cols = state.HeightMap[0].Length;

        if (neighbor.Row < 0 || neighbor.Row >= rows || neighbor.Col < 0 || neighbor.Col >= cols || state.Visited[neighbor.Row, neighbor.Col])
        {
            return 0;
        }

        state.Visited[neighbor.Row, neighbor.Col] = true;
        var neighborHeight = state.HeightMap[neighbor.Row][neighbor.Col];
        state.Boundary.Push((neighbor, Math.Max(height, neighborHeight)));

        return Math.Max(0, height - neighborHeight);
    }

    private sealed class FloodState(
        int[][] heightMap,
        bool[,] visited,
        Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> boundary)
    {
        public int[][] HeightMap { get; } = heightMap;

        public bool[,] Visited { get; } = visited;

        public Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> Boundary { get; } = boundary;
    }
}
