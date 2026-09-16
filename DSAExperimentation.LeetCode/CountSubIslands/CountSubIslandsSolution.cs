using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.CountSubIslands;

// LeetCode 1905. Count Sub Islands: count grid2's 4-directionally connected land
// components that lie entirely on land in grid1. A component is disqualified the
// moment any one of its cells is water in grid1, so the whole component has to be
// walked before it can be counted - which makes this the border-agnostic flood
// fill Number Of Islands and Max Area Of Island already use, with a per-island
// predicate instead of a per-island size.
//
// Both strategies use "zero the cell out in grid2" as their visited set, so both
// work on a private copy of grid2 rather than the caller's array - the harnesses
// hand the same rows to every strategy, and LeetCode's own contract does not say
// the input survives.
internal static class CountSubIslandsSolution
{
    private const int Land = 1;

    private static readonly (int DeltaRow, int DeltaCol)[] Orthogonal = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    // The textbook answer: a hand-rolled recursive flood fill that carries the
    // "still covered by grid1?" flag along with it, so the sub-island test happens
    // during the walk rather than after it. Deliberately written without this
    // repo's primitives - it is the arm the composed strategy below has to justify
    // itself against.
    public static int CountByRecursiveFloodFill(int[][] grid1, int[][] grid2)
    {
        var remaining = CloneGrid(grid2);
        var count = 0;

        for (var row = 0; row < remaining.Length; row++)
        {
            for (var col = 0; col < remaining[row].Length; col++)
            {
                if (IsSubIsland(grid1, remaining, row, col))
                {
                    count++;
                }
            }
        }

        return count;
    }

    // One cell's worth of the scan: skip water, otherwise flood the island it belongs to
    // and report what the flood learned. The flood zeroes `remaining` as it goes, so a
    // cell already walked can never seed a second, truncated island.
    private static bool IsSubIsland(int[][] grid1, int[][] remaining, int row, int col)
    {
        if (remaining[row][col] != Land)
        {
            return false;
        }

        var coverage = Coverage.Covered;
        Flood((grid1, remaining), (row, col), ref coverage);

        return coverage == Coverage.Covered;
    }

    // DepthFirstSearch.Traverse (Algorithms.Traversal.DepthFirst) is already
    // "collect every node reachable from a root through an arbitrary successor
    // function", and "this land cell's land neighbours" is exactly that relation,
    // so one call collects a whole island and the grid1 coverage test becomes a
    // pass over the returned cells. Grid/GridTopology (DataStructures.Graph.Grids)
    // is not the fit: IGraphTopology.GetChildren is static-abstract, so it cannot
    // close over the runtime pair of grids one walk has to consult - the
    // "arbitrary successor relation" case DepthFirstSearch's own doc comment
    // carves a Func-based engine out for.
    public static int CountByDepthFirstSearchTraverse(int[][] grid1, int[][] grid2)
    {
        var remaining = CloneGrid(grid2);
        var count = 0;

        for (var row = 0; row < remaining.Length; row++)
        {
            for (var col = 0; col < remaining[row].Length; col++)
            {
                if (IsSubIslandByTraversal(grid1, remaining, row, col))
                {
                    count++;
                }
            }
        }

        return count;
    }

    // The same one-cell step as IsSubIsland, with the walk handed to the traversal
    // primitive and the grid1 coverage test deferred to a pass over the cells it collected.
    private static bool IsSubIslandByTraversal(int[][] grid1, int[][] remaining, int row, int col)
    {
        if (remaining[row][col] != Land)
        {
            return false;
        }

        var island = DepthFirstSearch.Traverse<(int Row, int Col)>(
            (row, col), cell => LandNeighbors(remaining, cell));

        return IsIslandCovered(grid1, remaining, island);
    }

    private static IEnumerable<(int Row, int Col)> LandNeighbors(int[][] remaining, (int Row, int Col) cell)
    {
        foreach (var (deltaRow, deltaCol) in Orthogonal)
        {
            var row = cell.Row + deltaRow;
            var col = cell.Col + deltaCol;

            if (IsLand(remaining, row, col))
            {
                yield return (row, col);
            }
        }
    }

    // Runs after the traversal, never during it: the walk reads `remaining` lazily
    // through LandNeighbors, so zeroing cells mid-walk would truncate the island.
    // Clears the island out of `remaining` as it checks each cell against grid1.
    private static bool IsIslandCovered(
        int[][] grid1, int[][] remaining, List<(int Row, int Col)> island)
    {
        var isSubIsland = true;

        foreach (var (row, col) in island)
        {
            if (grid1[row][col] != Land)
            {
                isSubIsland = false;
            }

            remaining[row][col] = 0;
        }

        return isSubIsland;
    }

    // The walk consults two grids at once - the reference grid it is testing against
    // and the working copy it zeroes as it goes - so they travel as the pair the
    // strategy comment above already calls them. Its answer for the component it is
    // walking is the enum rather than a bare flag: Covered until the first cell that
    // is water in grid1, Disqualified from then on.
    private static void Flood(
        (int[][] Grid1, int[][] Remaining) grids, (int Row, int Col) cell, ref Coverage coverage)
    {
        if (!IsLand(grids.Remaining, cell.Row, cell.Col))
        {
            return;
        }

        grids.Remaining[cell.Row][cell.Col] = 0;

        if (grids.Grid1[cell.Row][cell.Col] != Land)
        {
            coverage = Coverage.Disqualified;
        }

        Flood(grids, (cell.Row + 1, cell.Col), ref coverage);
        Flood(grids, (cell.Row - 1, cell.Col), ref coverage);
        Flood(grids, (cell.Row, cell.Col + 1), ref coverage);
        Flood(grids, (cell.Row, cell.Col - 1), ref coverage);
    }

    // What the flood has learned about the component it is walking, named where a
    // rewritten `true`/`false` at the call site said it only by position.
    private enum Coverage
    {
        Covered,
        Disqualified,
    }

    private static bool IsLand(int[][] grid, int row, int col)
        => row >= 0 && row < grid.Length && col >= 0 && col < grid[row].Length && grid[row][col] == Land;

    private static int[][] CloneGrid(int[][] source)
    {
        var clone = new int[source.Length][];

        for (var row = 0; row < source.Length; row++)
        {
            clone[row] = (int[])source[row].Clone();
        }

        return clone;
    }
}
