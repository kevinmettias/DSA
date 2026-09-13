using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.CountServersThatCommunicate;

// LeetCode 1267. Count Servers that Communicate: a server (a 1 in the grid)
// communicates when some other server shares its row or its column, and the answer
// is how many servers do.
//
// Both strategies ask the same question of every server - "is there a second server
// on my row, or on my column?" - and differ only in whether the row and column
// tallies are recomputed per server or counted once up front.
internal static class CountServersThatCommunicateSolution
{
    private const int Server = 1;

    // "Communicates" means a second server, not merely a present one.
    private const int CompanionThreshold = 1;

    // The textbook baseline this composition has to justify itself against: every
    // server independently re-scans its whole row and its whole column from scratch,
    // O(rows*cols*(rows+cols)) overall. The scans deliberately never exit early on
    // the first companion found - they count the whole row or column every time - so
    // the cost reflects the redundant recomputation rather than how quickly a
    // companion happens to turn up in one particular grid. Deliberately written
    // without this repo's primitives.
    public static int CountServersByRowAndColumnRescan(int[][] grid)
    {
        var communicating = 0;

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (grid[r][c] == Server && (HasCompanionInRow(grid, r) || HasCompanionInColumn(grid, c)))
                {
                    communicating++;
                }
            }
        }

        return communicating;
    }

    private static bool HasCompanionInRow(int[][] grid, int row)
    {
        var count = 0;

        for (var c = 0; c < grid[0].Length; c++)
        {
            count += grid[row][c];
        }

        return count > CompanionThreshold;
    }

    private static bool HasCompanionInColumn(int[][] grid, int col)
    {
        var count = 0;

        for (var r = 0; r < grid.Length; r++)
        {
            count += grid[r][col];
        }

        return count > CompanionThreshold;
    }

    // Two of this repo's own HashMap<int,int> instances tally servers per row and
    // per column in one pass, and a second pass reads those tallies back -
    // O(rows*cols) overall. This is the same row/column-tracking shape
    // SetMatrixZeroesSolution uses via Set<int>, upgraded to counts because
    // "communicates" needs more-than-one rather than merely present.
    public static int CountServersByRowAndColumnCounts(int[][] grid)
    {
        var counts = Tally(grid);
        var communicating = 0;

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (Communicates(grid, r, c, counts))
                {
                    communicating++;
                }
            }
        }

        return communicating;
    }

    private static ServerCounts Tally(int[][] grid)
    {
        var counts = new ServerCounts(new HashMap<int, int>(), new HashMap<int, int>());

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (grid[r][c] == Server)
                {
                    Increment(counts.RowCounts, r);
                    Increment(counts.ColCounts, c);
                }
            }
        }

        return counts;
    }

    private static bool Communicates(int[][] grid, int row, int col, ServerCounts counts)
    {
        if (grid[row][col] != Server)
        {
            return false;
        }

        counts.RowCounts.TryGetValue(row, out var rowCount);
        counts.ColCounts.TryGetValue(col, out var colCount);

        return rowCount > CompanionThreshold || colCount > CompanionThreshold;
    }

    private static void Increment(HashMap<int, int> counts, int key)
    {
        counts.TryGetValue(key, out var current);
        counts.Set(key, current + 1);
    }

    private readonly record struct ServerCounts(HashMap<int, int> RowCounts, HashMap<int, int> ColCounts);
}
