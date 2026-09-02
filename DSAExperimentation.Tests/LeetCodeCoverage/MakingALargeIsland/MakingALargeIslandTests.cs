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
        var context = new GridContext(grid, grid.Length, grid[0].Length);
        var areaById = LabelIslands(context);

        var best = 0;

        foreach (var area in areaById.Values)
        {
            best = Math.Max(best, area);
        }

        var bestFlipped = BestFlippedCellArea(context, areaById);
        return Math.Max(best, bestFlipped);
    }

    private static HashMap<int, int> LabelIslands(GridContext context)
    {
        var areaById = new HashMap<int, int>();
        var nextId = 2;

        for (var r = 0; r < context.Rows; r++)
        {
            for (var c = 0; c < context.Cols; c++)
            {
                if (context.Grid[r][c] != 1)
                {
                    continue;
                }

                LabelIslandAt(context, areaById, (r, c), nextId);
                nextId++;
            }
        }

        return areaById;
    }

    private static void LabelIslandAt(GridContext context, HashMap<int, int> areaById, (int Row, int Col) start, int islandId)
    {
        var island = DepthFirstSearch.Traverse(start, p => UnlabeledLandNeighbors(context, p));
        areaById.Set(islandId, island.Count);

        foreach (var (row, col) in island)
        {
            context.Grid[row][col] = islandId;
        }
    }

    private static IEnumerable<(int Row, int Col)> UnlabeledLandNeighbors(GridContext context, (int Row, int Col) p)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = p.Row + dRow;
            var nextCol = p.Col + dCol;

            if (nextRow < 0 || nextRow >= context.Rows || nextCol < 0 || nextCol >= context.Cols)
            {
                continue;
            }

            if (context.Grid[nextRow][nextCol] != 1)
            {
                continue;
            }

            yield return (nextRow, nextCol);
        }
    }

    private static int BestFlippedCellArea(GridContext context, HashMap<int, int> areaById)
    {
        var best = 0;

        for (var r = 0; r < context.Rows; r++)
        {
            for (var c = 0; c < context.Cols; c++)
            {
                if (context.Grid[r][c] != 0)
                {
                    continue;
                }

                var area = FlippedCellArea(context, areaById, (r, c));
                best = Math.Max(best, area);
            }
        }

        return best;
    }

    private static int FlippedCellArea(GridContext context, HashMap<int, int> areaById, (int Row, int Col) cell)
    {
        var seenIslandIds = new Set<int>();
        var merged = 1;

        foreach (var (dRow, dCol) in Directions)
        {
            var neighbor = (cell.Row + dRow, cell.Col + dCol);
            merged += NeighborIslandArea(context, areaById, seenIslandIds, neighbor);
        }

        return merged;
    }

    private static int NeighborIslandArea(GridContext context, HashMap<int, int> areaById, Set<int> seenIslandIds, (int Row, int Col) neighbor)
    {
        if (neighbor.Row < 0 || neighbor.Row >= context.Rows || neighbor.Col < 0 || neighbor.Col >= context.Cols)
        {
            return 0;
        }

        var neighborId = context.Grid[neighbor.Row][neighbor.Col];

        if (neighborId < 2 || !seenIslandIds.TryAdd(neighborId))
        {
            return 0;
        }

        return areaById.TryGetValue(neighborId, out var area) ? area : 0;
    }

    private readonly record struct GridContext(int[][] Grid, int Rows, int Cols);
}
