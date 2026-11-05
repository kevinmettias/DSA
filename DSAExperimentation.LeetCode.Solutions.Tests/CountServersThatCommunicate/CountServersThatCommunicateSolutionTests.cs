using DSAExperimentation.LeetCode.Solutions.CountServersThatCommunicate;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountServersThatCommunicate;

// Harness only. Both strategies are CountServersThatCommunicateSolution's - the
// per-server row/column rescan baseline and the two-pass HashMap<int,int> tally -
// and this file just pins them to the
// degenerate grids (no servers at all, a lone server, a single full row) that
// separate "present" from "has a companion".
public sealed partial class CountServersThatCommunicateSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // No servers at all.
            { [[0, 0], [0, 0]], 0 },

            // One full row, and the same servers as one column.
            { [[1, 1, 1]], 3 },
            { [[1], [1], [1]], 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountServersByRowAndColumnRescan_Cases_CountsOnlyServersWithACompanion(
        int[][] grid, int expected) =>
        Assert.Equal(expected, CountServersThatCommunicateSolution.CountServersByRowAndColumnRescan(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountServersByRowAndColumnCounts_Cases_CountsOnlyServersWithACompanion(
        int[][] grid, int expected) =>
        Assert.Equal(expected, CountServersThatCommunicateSolution.CountServersByRowAndColumnCounts(grid));
}
