using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.NumberOfIslands;

// LeetCode 200. Number of Islands: count 4-connected components of land ('1')
// cells in a grid.
//
// DepthFirstSearch.Traverse already returns every cell reachable from a start
// through an arbitrary successor function, so counting islands is one scan over
// every cell that starts a fresh traversal from each still-unclaimed land cell.
// A HashSet of claimed coordinates - not the pre-migration version's destructive
// '1' -> '0' grid mutation - is what lets Neighbors skip cells a previous
// island's traversal already covered; it also means the same grid can be
// measured repeatedly without rebuilding it between benchmark invocations,
// which the mutating version could not survive.
//
// Two strategies share that scan and that claimed set: the breadth-first sink below
// drains an explicit queue in FIFO order, while the depth-first sink reaches the
// same cells through DepthFirstSearch.Traverse's recursive walk. Both claim each
// island's cells exactly once, so they must count the same islands.
internal static class NumberOfIslandsSolution
{
    // The explicit-queue counterpart to the recursive sink below: the same scan starts
    // one flood fill per still-unclaimed land cell, but the frontier is a Queue drained
    // from the front rather than a call stack, so a long thin island cannot deepen the
    // recursion. It shares Neighbors and the claimed set, so it observes exactly the
    // cells the depth-first arm does; only the order it drains them in differs, which
    // is what the pair exists to time.
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

    public static int CountIslandsByDepthFirstSink(char[][] grid)
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

                foreach (var cell in DepthFirstSearch.Traverse((row, col), cell => Neighbors(cell, grid, claimed)))
                {
                    claimed.Add(cell);
                }
            }
        }

        return count;
    }

    // The successor function the scan hands to DepthFirstSearch.Traverse: a cell's four
    // orthogonal candidates, minus the ones that are off the grid, no longer hold land,
    // or were already covered by an earlier island's traversal.
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
}
