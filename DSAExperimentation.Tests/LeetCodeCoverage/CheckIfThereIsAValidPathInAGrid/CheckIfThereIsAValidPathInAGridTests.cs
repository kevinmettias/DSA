using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfThereIsAValidPathInAGrid;

// LeetCode 1391. Check if There is a Valid Path in a Grid: each street piece
// (1-6) opens toward a fixed pair of the four directions; a move from one cell to
// an orthogonal neighbor is only valid when both cells open toward each other.
// That "successor" relation is arbitrary per-cell logic, not a fixed passable/
// impassable check GridTopology models, so this composes this repo's own
// DepthFirstSearch.Traverse over a bespoke successor function - the same
// implicit-graph shape PacificAtlanticWaterFlowTests already uses for grid
// problems whose adjacency isn't just "in bounds" - and checks whether the
// bottom-right corner appears in the reachable set from (0,0).
public sealed partial class CheckIfThereIsAValidPathInAGridTests
{
    private static readonly Dictionary<int, (int DRow, int DCol)[]> Openings = new()
    {
        [1] = [(0, -1), (0, 1)],
        [2] = [(-1, 0), (1, 0)],
        [3] = [(0, -1), (1, 0)],
        [4] = [(0, 1), (1, 0)],
        [5] = [(0, -1), (-1, 0)],
        [6] = [(0, 1), (-1, 0)],
    };

    [Fact]
    public void HasValidPath_StreetsTurnTwiceOnTheWayToTheCorner_ReturnsTrue()
    {
        // (0,0)-R->(0,1)-D->(1,1)-D->(2,1)-R->(2,2): two turns via a right-down
        // street (3), a straight vertical street (2), then a right-up street (6).
        int[][] grid = [[1, 3, 1], [2, 2, 2], [1, 6, 1]];

        Assert.True(HasValidPath(grid));
    }

    [Fact]
    public void HasValidPath_LeetCodeExampleTwo_ReturnsFalse()
    {
        int[][] grid = [[1, 2, 1], [1, 2, 1]];

        Assert.False(HasValidPath(grid));
    }

    [Fact]
    public void HasValidPath_SingleRowOfHorizontalStreets_ReturnsTrue()
    {
        int[][] grid = [[1, 1, 1]];

        Assert.True(HasValidPath(grid));
    }

    [Fact]
    public void HasValidPath_LastStreetOpensTheWrongWay_ReturnsFalse()
    {
        int[][] grid = [[1, 1, 2]];

        Assert.False(HasValidPath(grid));
    }

    private static bool HasValidPath(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var target = (Row: rows - 1, Col: cols - 1);

        return DepthFirstSearch.Traverse((Row: 0, Col: 0), Neighbors).Contains(target);

        IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) cell)
        {
            foreach (var (dRow, dCol) in Openings[grid[cell.Row][cell.Col]])
            {
                var next = (Row: cell.Row + dRow, Col: cell.Col + dCol);

                if (IsValidNeighbor(next, (dRow, dCol), grid))
                {
                    yield return next;
                }
            }
        }
    }

    private static bool IsValidNeighbor((int Row, int Col) next, (int DRow, int DCol) direction, int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        if (next.Row < 0 || next.Row >= rows || next.Col < 0 || next.Col >= cols)
        {
            return false;
        }

        return Array.IndexOf(Openings[grid[next.Row][next.Col]], (-direction.DRow, -direction.DCol)) >= 0;
    }
}
