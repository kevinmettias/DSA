using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.DetectCyclesIn2DGrid;

// LeetCode 1559. Detect Cycles in 2D Grid: does the grid contain a cycle of length
// four or more made entirely of one repeated character, moving orthogonally and
// never stepping straight back onto the cell you came from?
//
// Both strategies answer the same question, they just track "have I been here
// before by another route" differently: the baseline carries its own visited/parent
// bookkeeping through an explicit stack, while the composed arm hands that job to
// this repo's own DisjointSet.
//
// The grid graph is bipartite, so it has no cycles of length two or three at all -
// which is why neither arm has to check the problem's "length >= 4" clause
// separately. It is satisfied by the shape of the input.
internal static class DetectCyclesIn2DGridSolution
{
    // Sentinel row/column for the root of a traversal, which has no parent cell.
    private const int NoParent = -1;

    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The textbook baseline: one explicit-stack DFS per unvisited cell, with
    // visited marks and the parent cell tracked by hand, so a neighbor that is
    // already visited and is not the cell we arrived from closes a cycle.
    // Deliberately written on BCL types alone - it is the arm the DisjointSet
    // strategy below has to justify itself against.
    public static bool HasCycleByParentTrackedDepthFirstSearch(char[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var visited = new bool[rows, cols];

        for (var startRow = 0; startRow < rows; startRow++)
        {
            for (var startCol = 0; startCol < cols; startCol++)
            {
                if (!visited[startRow, startCol] && HasCycleFrom(grid, startRow, startCol, visited))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool HasCycleFrom(char[][] grid, int startRow, int startCol, bool[,] visited)
    {
        var walk = new CycleWalk(grid, visited, new Stack<Step>());
        walk.Stack.Push(new Step(startRow, startCol, NoParent, NoParent));
        visited[startRow, startCol] = true;

        while (walk.Stack.Count > 0)
        {
            var current = walk.Stack.Pop();

            foreach (var direction in Directions)
            {
                if (HasCycleAtNeighbor(walk, current, direction))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool HasCycleAtNeighbor(CycleWalk walk, Step current, (int DRow, int DCol) direction)
    {
        var nextRow = current.Row + direction.DRow;
        var nextCol = current.Col + direction.DCol;

        if (IsOutsideGrid(nextRow, nextCol, walk.Grid.Length, walk.Grid[0].Length))
        {
            return false;
        }

        if (IsBlockedNeighbor(walk.Grid, nextRow, nextCol, current))
        {
            return false;
        }

        if (walk.Visited[nextRow, nextCol])
        {
            return true;
        }

        walk.Visited[nextRow, nextCol] = true;
        walk.Stack.Push(new Step(nextRow, nextCol, current.Row, current.Col));
        return false;
    }

    // A neighbor only continues the walk when it carries the same character and is not
    // the cell we arrived from - stepping straight back along the parent edge is not a
    // cycle, it is the way we came.
    private static bool IsBlockedNeighbor(char[][] grid, int nextRow, int nextCol, Step current) =>
        grid[nextRow][nextCol] != grid[current.Row][current.Col]
        || (nextRow == current.ParentRow && nextCol == current.ParentCol);

    // This repo's own DisjointSet: union each cell with its same-character
    // right/down neighbor exactly once per edge - the "already connected before the
    // union means a cycle" shape RedundantConnection and RegionsCutBySlashes also
    // use, gated here on same-character adjacency. Visiting only right and down
    // (never left or up) means every union crosses a genuinely new edge, so
    // IsConnected can only already be true when a path closes back into the same
    // component.
    public static bool HasCycleByDisjointSetEdgeUnion(char[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var context = new GridComponents(grid, new DisjointSet(rows * cols), cols);

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                if (HasCycleThroughNeighbor(context, (row, col), (row, col + 1)))
                {
                    return true;
                }

                if (HasCycleThroughNeighbor(context, (row, col), (row + 1, col)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private readonly record struct Step(int Row, int Col, int ParentRow, int ParentCol);

    private readonly record struct CycleWalk(char[][] Grid, bool[,] Visited, Stack<Step> Stack);

    private static bool HasCycleThroughNeighbor(
        GridComponents context, (int Row, int Col) cell, (int Row, int Col) neighbor)
    {
        if (IsOutsideGrid(neighbor.Row, neighbor.Col, context.Grid.Length, context.Cols)
            || context.Grid[neighbor.Row][neighbor.Col] != context.Grid[cell.Row][cell.Col])
        {
            return false;
        }

        var id = (cell.Row * context.Cols) + cell.Col;
        var neighborId = (neighbor.Row * context.Cols) + neighbor.Col;

        if (context.Components.IsConnected(id, neighborId))
        {
            return true;
        }

        context.Components.Union(id, neighborId);
        return false;
    }

    private static bool IsOutsideGrid(int row, int col, int rows, int cols) =>
        row < 0 || row >= rows || col < 0 || col >= cols;

    private readonly record struct GridComponents(char[][] Grid, DisjointSet Components, int Cols);
}
