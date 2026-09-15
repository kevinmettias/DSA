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
internal static class NumberOfIslandsSolution
{
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
