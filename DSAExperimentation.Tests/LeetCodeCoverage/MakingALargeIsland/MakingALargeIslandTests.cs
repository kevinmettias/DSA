using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MakingALargeIsland;

// LeetCode 827. Making A Large Island: the same border-agnostic flood-fill shape
// MaxAreaOfIslandTests/NumberOfIslandsTests already use - this repo's own
// DepthFirstSearch.Traverse labels each island (writing a distinct id >= 2 back
// into the grid in place of the traversed 1s) while a HashMap<int,int> records
// that island's area by id. For every water cell, its up-to-4 neighboring ids are
// deduped through this repo's own Set<int> (backed by HashMap<T,bool>, the same
// role HashMap plays for TwoSumTests) and their areas summed with the flipped
// cell itself, to find the best possible merge. Scanning the recorded areas
// directly covers the "grid is already all land" case with no separate branch.
public sealed partial class MakingALargeIslandTests
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Fact]
    public void LargestIsland_OneWaterCellBetweenTwoDiagonalIslands_MergesBothPlusFlip()
    {
        int[][] grid = [[1, 0], [0, 1]];

        Assert.Equal(3, LargestIsland(grid));
    }

    [Fact]
    public void LargestIsland_FlippingCornerJoinsLShapeIntoFullSquare()
    {
        int[][] grid = [[1, 1], [1, 0]];

        Assert.Equal(4, LargestIsland(grid));
    }

    [Fact]
    public void LargestIsland_AlreadyEntirelyLand_ReturnsFullGridArea()
    {
        int[][] grid = [[1, 1], [1, 1]];

        Assert.Equal(4, LargestIsland(grid));
    }

    [Fact]
    public void LargestIsland_EntirelyWater_FlippingSingleCellGivesAreaOne()
    {
        int[][] grid = [[0, 0], [0, 0]];

        Assert.Equal(1, LargestIsland(grid));
    }

    private static int LargestIsland(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var areaById = new HashMap<int, int>();
        var nextId = 2;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 1)
                {
                    continue;
                }

                var island = DepthFirstSearch.Traverse((r, c), UnlabeledLandNeighbors);
                areaById.Set(nextId, island.Count);

                foreach (var (row, col) in island)
                {
                    grid[row][col] = nextId;
                }

                nextId++;
            }
        }

        var best = 0;

        foreach (var area in areaById.Values)
        {
            best = Math.Max(best, area);
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 0)
                {
                    continue;
                }

                best = Math.Max(best, FlippedCellArea(grid, areaById, r, c, rows, cols));
            }
        }

        return best;

        IEnumerable<(int Row, int Col)> UnlabeledLandNeighbors((int Row, int Col) p)
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = p.Row + dRow;
                var nextCol = p.Col + dCol;

                if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols)
                {
                    continue;
                }

                if (grid[nextRow][nextCol] != 1)
                {
                    continue;
                }

                yield return (nextRow, nextCol);
            }
        }
    }

    private static int FlippedCellArea(int[][] grid, HashMap<int, int> areaById, int row, int col, int rows, int cols)
    {
        var seenIslandIds = new Set<int>();
        var merged = 1;

        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = row + dRow;
            var nextCol = col + dCol;

            if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols)
            {
                continue;
            }

            var neighborId = grid[nextRow][nextCol];

            if (neighborId < 2 || !seenIslandIds.TryAdd(neighborId))
            {
                continue;
            }

            if (areaById.TryGetValue(neighborId, out var area))
            {
                merged += area;
            }
        }

        return merged;
    }
}
