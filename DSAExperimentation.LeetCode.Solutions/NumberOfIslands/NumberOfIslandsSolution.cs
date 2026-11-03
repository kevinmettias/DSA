using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.Solutions.NumberOfIslands;

// LeetCode 200. Number of Islands: count 4-connected components of land ('1')
// cells in a grid.
//
// An island is a connected component of the land, so the count is this repo's
// ConnectedComponents over GridTopology: the land becomes a Grid whose passable
// cells are the '1's, and every land cell the walks have not yet reached starts
// one more island. Neither strategy writes to the grid - the pre-migration
// version's destructive '1' -> '0' mutation is gone - so the same grid can be
// measured repeatedly without rebuilding it between benchmark invocations.
//
// The baseline is the textbook flood fill the component count stands for: one scan
// that starts an explicit-queue fill from each still-unclaimed land cell, with a
// HashSet of claimed coordinates standing in for the walks' shared visited set.
[LeetCodeQuestion("number-of-islands", LeetCodeAnswerOrder.AsWritten)]
internal static class NumberOfIslandsSolution
{
    // The textbook answer: the scan starts one flood fill per still-unclaimed land
    // cell, its frontier a BCL Queue drained from the front, so a long thin island
    // cannot deepen any recursion. Deliberately written without this repo's
    // primitives - it is the arm the component count below has to justify itself
    // against.
    public static int CountIslandsByBreadthFirstSink(char[][] grid)
    {
        var claimed = new HashSet<(int Row, int Col)>();
        var count = 0;

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[0].Length; col++)
            {
                if (grid[row][col] != '1' || !claimed.Add((row, col)))
                {
                    continue;
                }

                count++;
                SinkFrom((row, col), grid, claimed);
            }
        }

        return count;
    }

    // One island's own flood: the start is already claimed, and every unclaimed land
    // neighbor Neighbors yields is claimed and queued in turn, so the frontier never
    // carries a cell twice.
    private static void SinkFrom((int Row, int Col) start, char[][] grid, HashSet<(int Row, int Col)> claimed)
    {
        var frontier = new Queue<(int Row, int Col)>();
        frontier.Enqueue(start);

        while (frontier.Count > 0)
        {
            foreach (var neighbor in Neighbors(frontier.Dequeue(), grid, claimed))
            {
                claimed.Add(neighbor);
                frontier.Enqueue(neighbor);
            }
        }
    }

    // The baseline's successor function: a cell's four orthogonal candidates, minus the
    // ones that are off the grid, no longer hold land, or were already covered by an
    // earlier island's fill.
    private static IEnumerable<(int Row, int Col)> Neighbors(
        (int Row, int Col) cell, char[][] grid, HashSet<(int Row, int Col)> claimed)
    {
        (int Row, int Col)[] candidates =
        [
            (cell.Row + 1, cell.Col),
            (cell.Row - 1, cell.Col),
            (cell.Row, cell.Col + 1),
            (cell.Row, cell.Col - 1),
        ];

        foreach (var candidate in candidates)
        {
            if (IsUnclaimedLandNeighbor(candidate, grid, claimed))
            {
                yield return candidate;
            }
        }
    }

    // A neighbor joins the island when it is on the grid, still holds a '1', and
    // has not already been claimed by an earlier island's traversal.
    private static bool IsUnclaimedLandNeighbor(
        (int Row, int Col) candidate, char[][] grid, HashSet<(int Row, int Col)> claimed)
        => candidate.Row >= 0 && candidate.Row < grid.Length
            && candidate.Col >= 0 && candidate.Col < grid[0].Length
            && grid[candidate.Row][candidate.Col] == '1'
            && !claimed.Contains(candidate);

    // ConnectedComponents walks each component depth-first through the multi-root
    // Reduce.Graph overload, whose one visited set is what keeps a land cell from
    // starting a second island once an earlier walk has reached it.
    public static int CountIslandsByConnectedComponents(char[][] grid)
    {
        var land = Grid.Where(GridSize.Of(grid), new LandCell(grid)).PassableNodes<NodePerCell>();

        return ConnectedComponents.Count<GridNode, GridTopology<NodePerCell>, GridChildren<NodePerCell>, DepthFirstReduceOrder<GridNode>>(land);
    }

    // The cells an island is made of.
    private readonly struct LandCell(char[][] grid) : IGridCellFilter
    {
        private readonly char[][] _grid = grid;

        public bool CanEnter(int row, int col) => _grid[row][col] == '1';
    }
}
