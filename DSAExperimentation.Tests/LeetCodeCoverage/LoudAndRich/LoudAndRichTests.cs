using DSAExperimentation.LeetCode.LoudAndRich;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LoudAndRich;

// Harness only. Both strategies are LoudAndRichSolution's - this file pins them
// to LeetCode's published examples, stated in LeetCode's own (richer, quiet)
// input shape. The per-person walk was previously a benchmark-only baseline and
// had never been asserted against anything.
public sealed class LoudAndRichTests
{
    public static TheoryData<int[][], int[], int[]> Examples =>
        new()
        {
            {
                [[1, 0], [2, 1], [3, 1], [3, 7], [4, 3], [5, 3], [6, 3]],
                [3, 2, 5, 4, 6, 1, 7, 0],
                [5, 5, 2, 5, 4, 5, 6, 7]
            },
            { [], [0], [0] },
            { [], [4, 1, 3], [0, 1, 2] },
            { [[0, 1]], [1, 0], [0, 1] },
            { [[1, 0]], [1, 0], [1, 1] },
            { [[0, 1], [1, 2]], [0, 1, 2], [0, 0, 0] },
            { [[2, 0], [2, 1]], [5, 4, 9], [0, 1, 2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void QuietestByTopologicalDpPass_LeetCodeExamples_ReturnsQuietestRicherOrEqualPersonPerPerson(
        int[][] richer, int[] quiet, int[] expected)
    {
        var actual = LoudAndRichSolution.QuietestByTopologicalDpPass(richer, quiet);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void QuietestByPerPersonWalk_LeetCodeExamples_ReturnsQuietestRicherOrEqualPersonPerPerson(
        int[][] richer, int[] quiet, int[] expected)
    {
        var actual = LoudAndRichSolution.QuietestByPerPersonWalk(richer, quiet);
        Assert.Equal(expected, actual);
    }
}
