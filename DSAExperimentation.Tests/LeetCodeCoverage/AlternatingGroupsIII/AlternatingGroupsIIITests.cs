using DSAExperimentation.LeetCode.AlternatingGroupsIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AlternatingGroupsIII;

// Harness only. Both strategies are AlternatingGroupsIIISolution's (the run
// bookkeeping itself is AlternatingRunLedger's) - this file just pins them to
// LeetCode's published examples, including the query that repaints a tile to
// the color it already has (a no-op the ledger must not mistake for a wall
// change).
public sealed class AlternatingGroupsIIITests
{
    public static TheoryData<int[], int[][], int[]> Examples =>
        new()
        {
            { [0, 1, 1, 0, 1], [[2, 1, 0], [1, 4]], [2] },
            { [0, 0, 1, 0, 1, 1], [[1, 3], [2, 3, 0], [1, 5]], [2, 0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfAlternatingGroupsByBruteForce_LeetCodeExamples_ReturnsAnswersInQueryOrder(
        int[] colors, int[][] queries, int[] expected)
    {
        var actual = AlternatingGroupsIIISolution.NumberOfAlternatingGroupsByBruteForce(colors, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfAlternatingGroupsByRunLengthFenwick_LeetCodeExamples_ReturnsAnswersInQueryOrder(
        int[] colors, int[][] queries, int[] expected)
    {
        var actual = AlternatingGroupsIIISolution.NumberOfAlternatingGroupsByRunLengthFenwick(colors, queries);

        Assert.Equal(expected, actual);
    }
}
