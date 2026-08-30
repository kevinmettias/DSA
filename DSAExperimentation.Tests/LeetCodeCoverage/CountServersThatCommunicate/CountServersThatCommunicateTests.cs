using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountServersThatCommunicate;

// LeetCode 1267. Count Servers that Communicate: two of this repo's own
// HashMap<int,int> instances counting servers per row and per column (the same
// row/column-tracking shape SetMatrixZeroesTests uses via Set<int>, upgraded to
// counts since "communicates" needs more-than-one, not merely present) - one pass
// to tally, one pass to count every server whose row or column tally exceeds one.
public sealed class CountServersThatCommunicateTests
{
    [Fact]
    public void CountServers_NoServerSharesARowOrColumn_ReturnsZero()
    {
        int[][] grid = [[1, 0], [0, 1]];

        Assert.Equal(0, CountServers(grid));
    }

    [Fact]
    public void CountServers_AllServersShareARowOrColumn_ReturnsAllOfThem()
    {
        int[][] grid = [[1, 0], [1, 1]];

        Assert.Equal(3, CountServers(grid));
    }

    [Fact]
    public void CountServers_MixOfConnectedAndIsolatedServers_CountsOnlyConnectedOnes()
    {
        int[][] grid = [[1, 1, 0, 0], [0, 0, 1, 0], [0, 0, 1, 0], [0, 0, 0, 1]];

        Assert.Equal(4, CountServers(grid));
    }

    private static int CountServers(int[][] grid)
    {
        var rowCounts = new HashMap<int, int>();
        var colCounts = new HashMap<int, int>();

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (grid[r][c] == 1)
                {
                    Increment(rowCounts, r);
                    Increment(colCounts, c);
                }
            }
        }

        var communicating = 0;

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (grid[r][c] != 1)
                {
                    continue;
                }

                rowCounts.TryGetValue(r, out var rowCount);
                colCounts.TryGetValue(c, out var colCount);

                if (rowCount > 1 || colCount > 1)
                {
                    communicating++;
                }
            }
        }

        return communicating;
    }

    private static void Increment(HashMap<int, int> counts, int key)
    {
        counts.TryGetValue(key, out var current);
        counts.Set(key, current + 1);
    }
}
