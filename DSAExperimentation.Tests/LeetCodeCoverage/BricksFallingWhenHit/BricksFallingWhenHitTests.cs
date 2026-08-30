using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BricksFallingWhenHit;

// LeetCode 803. Bricks Falling When Hit: process hits in reverse, "un-hitting"
// bricks back into the grid and unioning each newly-restored brick with any
// already-standing neighbor (and a virtual roof node, for row 0) via this repo's
// own DisjointSet. A brick's component size is caller-side bookkeeping threaded
// beside Find/Union - DisjointSet itself has no size query, the same "own scratch
// state next to a composed primitive" shape TopologicalSort.cs's in-degree
// dictionary and ShortestPath.cs's Distances dictionary already establish - so no
// new production primitive is needed, only a size[] array local to this solution.
public sealed partial class BricksFallingWhenHitTests
{
    [Fact]
    public void HitBricks_ClassicExample_FallingBricksExcludeTheHitBrickItself()
    {
        int[][] grid = [[1, 0, 0, 0], [1, 1, 1, 0]];
        int[][] hits = [[1, 0]];

        var fallen = HitBricks(grid, hits);

        Assert.Equal([2], fallen);
    }

    [Fact]
    public void HitBricks_HitsThatNeverReconnectToTheRoof_ReturnZero()
    {
        int[][] grid = [[1, 0, 0, 0], [1, 1, 0, 0]];
        int[][] hits = [[1, 1], [1, 0]];

        var fallen = HitBricks(grid, hits);

        Assert.Equal([0, 0], fallen);
    }

    private static int[] HitBricks(int[][] grid, int[][] hits)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var roof = rows * cols;
        var present = new bool[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                present[r, c] = grid[r][c] == 1;
            }
        }

        foreach (var hit in hits)
        {
            present[hit[0], hit[1]] = false;
        }

        var components = new DisjointSet(roof + 1);
        var size = new int[roof + 1];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (present[r, c])
                {
                    size[(r * cols) + c] = 1;
                }
            }
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (present[r, c])
                {
                    ConnectToStandingNeighbors(components, size, present, rows, cols, r, c, roof);
                }
            }
        }

        var fallenReversed = new int[hits.Length];

        for (var i = hits.Length - 1; i >= 0; i--)
        {
            var row = hits[i][0];
            var col = hits[i][1];

            if (grid[row][col] == 0)
            {
                continue;
            }

            var beforeSize = size[components.Find(roof)];
            present[row, col] = true;
            size[(row * cols) + col] = 1;
            ConnectToStandingNeighbors(components, size, present, rows, cols, row, col, roof);
            var afterSize = size[components.Find(roof)];

            fallenReversed[i] = afterSize > beforeSize ? afterSize - beforeSize - 1 : 0;
        }

        return fallenReversed;
    }

    private static void ConnectToStandingNeighbors(
        DisjointSet components, int[] size, bool[,] present, int rows, int cols, int row, int col, int roof)
    {
        var cellId = (row * cols) + col;

        if (row == 0)
        {
            Union(components, size, cellId, roof);
        }

        (int Row, int Col)[] neighbors = [(row - 1, col), (row + 1, col), (row, col - 1), (row, col + 1)];

        foreach (var (neighborRow, neighborCol) in neighbors)
        {
            if (neighborRow >= 0 && neighborRow < rows && neighborCol >= 0 && neighborCol < cols
                && present[neighborRow, neighborCol])
            {
                Union(components, size, cellId, (neighborRow * cols) + neighborCol);
            }
        }
    }

    private static void Union(DisjointSet components, int[] size, int first, int second)
    {
        var firstRoot = components.Find(first);
        var secondRoot = components.Find(second);

        if (firstRoot == secondRoot)
        {
            return;
        }

        components.Union(first, second);
        var mergedRoot = components.Find(first);
        size[mergedRoot] = size[firstRoot] + size[secondRoot];
    }
}
