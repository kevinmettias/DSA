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

        var visited = new bool[rows, cols];
        var boundary = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (r == 0 || r == rows - 1 || c == 0 || c == cols - 1)
                {
                    boundary.Push(((r, c), heightMap[r][c]));
                    visited[r, c] = true;
                }
            }
        }

        var water = 0;

        while (boundary.TryPop(out var entry))
        {
            var (row, col) = entry.Node;
            var height = entry.Priority;

            foreach (var (dr, dc) in Directions)
            {
                var nr = row + dr;
                var nc = col + dc;

                if (nr < 0 || nr >= rows || nc < 0 || nc >= cols || visited[nr, nc])
                {
                    continue;
                }

                visited[nr, nc] = true;
                var neighborHeight = heightMap[nr][nc];
                water += Math.Max(0, height - neighborHeight);
                boundary.Push(((nr, nc), Math.Max(height, neighborHeight)));
            }
        }

        return water;
    }
}
