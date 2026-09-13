using DSAExperimentation.LeetCode.CountServersThatCommunicate;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountServersThatCommunicate;

// Harness only. Both strategies are CountServersThatCommunicateSolution's - the
// per-server row/column rescan baseline and the two-pass HashMap<int,int> tally -
// and this file just pins them to LeetCode's published examples, plus the
// degenerate grids (no servers at all, a lone server, a single full row) that
// separate "present" from "has a companion".
public sealed class CountServersThatCommunicateTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LeetCode example 1: neither server shares a row or a column.
            { [[1, 0], [0, 1]], 0 },

            // LeetCode example 2: every server has a companion.
            { [[1, 0], [1, 1]], 3 },

            // LeetCode example 3: the bottom-right server is isolated.
            { [[1, 1, 0, 0], [0, 0, 1, 0], [0, 0, 1, 0], [0, 0, 0, 1]], 4 },

            // No servers at all.
            { [[0, 0], [0, 0]], 0 },

            // A lone server communicates with nothing, including itself.
            { [[1]], 0 },

            // One full row, and the same servers as one column.
            { [[1, 1, 1]], 3 },
            { [[1], [1], [1]], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountServersByRowAndColumnRescan_LeetCodeExamples_CountsOnlyServersWithACompanion(
        int[][] grid, int expected) =>
        Assert.Equal(expected, CountServersThatCommunicateSolution.CountServersByRowAndColumnRescan(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountServersByRowAndColumnCounts_LeetCodeExamples_CountsOnlyServersWithACompanion(
        int[][] grid, int expected) =>
        Assert.Equal(expected, CountServersThatCommunicateSolution.CountServersByRowAndColumnCounts(grid));
}
