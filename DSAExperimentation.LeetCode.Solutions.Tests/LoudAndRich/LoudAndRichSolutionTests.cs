using DSAExperimentation.LeetCode.Solutions.LoudAndRich;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LoudAndRich;

// Harness only. Both strategies are LoudAndRichSolution's - this file pins them,
// in LeetCode's own (richer, quiet) input shape, to small graphs: no richer
// relations at all, a single edge whose richer end is louder, a single edge whose
// richer end is quieter, a chain whose quietest person is at the top, and one
// richest person louder than both people it outranks. The per-person walk was
// previously a benchmark-only baseline and had never been asserted against
// anything. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class LoudAndRichSolutionTests
{
    public static TheoryData<int[][], int[], int[]> Cases =>
        new()
        {
            { [], [4, 1, 3], [0, 1, 2] },
            { [[0, 1]], [1, 0], [0, 1] },
            { [[1, 0]], [1, 0], [1, 1] },
            { [[0, 1], [1, 2]], [0, 1, 2], [0, 0, 0] },
            { [[2, 0], [2, 1]], [5, 4, 9], [0, 1, 2] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void QuietestByTopologicalDpPass_Cases_ReturnsQuietestRicherOrEqualPersonPerPerson(
        int[][] richer, int[] quiet, int[] expected)
    {
        var actual = LoudAndRichSolution.QuietestByTopologicalDpPass(richer, quiet);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void QuietestByPerPersonWalk_Cases_ReturnsQuietestRicherOrEqualPersonPerPerson(
        int[][] richer, int[] quiet, int[] expected)
    {
        var actual = LoudAndRichSolution.QuietestByPerPersonWalk(richer, quiet);
        Assert.Equal(expected, actual);
    }
}
