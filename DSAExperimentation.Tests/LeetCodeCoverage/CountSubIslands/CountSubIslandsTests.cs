using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubIslands;

// LeetCode 1905. Count Sub Islands: the same border-agnostic flood-fill shape
// MaxAreaOfIslandTests/NumberOfIslandsTests already use - this repo's own
// DepthFirstSearch.Traverse walks one grid2 island's full land component from each
// unvisited land cell, and that island only counts as a "sub island" if every one of
// its cells is also land in grid1 (a sub-island can never straddle a cell that's
// water in grid1). Traversed cells are zeroed in grid2 in place afterward so the
// outer scan's own "still land?" check doubles as the visited set, the same
// in-place-mutation trick MaxAreaOfIslandTests uses.
public sealed partial class CountSubIslandsTests
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Fact]
    public void CountSubIslands_EveryGrid2IslandFullyCoveredByGrid1_CountsAllOfThem()
    {
        int[][] grid1 =
        [
            [1, 1, 0],
            [0, 1, 1],
            [1, 0, 1],
        ];
        int[][] grid2 =
        [
            [1, 1, 0],
            [0, 1, 0],
            [1, 0, 1],
        ];

        // grid2's three islands - {(0,0),(0,1),(1,1)}, {(2,0)}, {(2,2)} - each land
        // straight onto grid1 land, so all three qualify as sub-islands.
        var actual = CountSubIslandsCount(grid1, grid2);
        Assert.Equal(3, actual);
    }

    [Fact]
    public void CountSubIslands_OneIslandStraddlesGrid1Water_ExcludesOnlyThatIsland()
    {
        int[][] grid1 =
        [
            [0, 1, 1],
            [1, 1, 0],
            [0, 0, 1],
        ];
        int[][] grid2 =
        [
            [1, 1, 1],
            [1, 1, 0],
            [0, 0, 1],
        ];

        // grid2's larger island {(0,0),(0,1),(0,2),(1,0),(1,1)} touches grid1[0][0] =
        // 0, so it's excluded; the isolated {(2,2)} island lands on grid1 land and
        // still counts, for a total of 1.
        var actual = CountSubIslandsCount(grid1, grid2);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void CountSubIslands_NoLandInGrid2_ReturnsZero()
    {
        int[][] grid1 = [[1, 1], [1, 1]];
        int[][] grid2 = [[0, 0], [0, 0]];

        var actual = CountSubIslandsCount(grid1, grid2);
        Assert.Equal(0, actual);
    }

    private static int CountSubIslandsCount(int[][] grid1, int[][] grid2)
    {
        var rows = grid2.Length;
        var cols = grid2[0].Length;
        var count = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (IsSubIslandRootedAt(grid1, grid2, r, c))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsSubIslandRootedAt(int[][] grid1, int[][] grid2, int r, int c)
    {
        if (grid2[r][c] != 1)
        {
            return false;
        }

        var island = TraverseIsland(grid2, r, c);
        return MarkVisitedAndCheckSubIsland(island, grid1, grid2);
    }

    private static List<(int Row, int Col)> TraverseIsland(int[][] grid2, int r, int c)
    {
        var rows = grid2.Length;
        var cols = grid2[0].Length;

        return DepthFirstSearch.Traverse((r, c), Neighbors);

        IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p)
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var next = (Row: p.Row + dRow, Col: p.Col + dCol);

                if (IsUnvisitedLand(next, rows, cols, grid2))
                {
                    yield return next;
                }
            }
        }
    }

    private static bool MarkVisitedAndCheckSubIsland(
        List<(int Row, int Col)> island, int[][] grid1, int[][] grid2)
    {
        var isSubIsland = true;

        foreach (var (row, col) in island)
        {
            if (grid1[row][col] != 1)
            {
                isSubIsland = false;
            }

            grid2[row][col] = 0;
        }

        return isSubIsland;
    }

    private static bool IsUnvisitedLand((int Row, int Col) cell, int rows, int cols, int[][] grid2)
        => cell.Row >= 0 && cell.Row < rows && cell.Col >= 0 && cell.Col < cols && grid2[cell.Row][cell.Col] == 1;
}
