using DSAExperimentation.LeetCode.MinimumDeletionsToMakeAlternatingSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumDeletionsToMakeAlternatingSubstring;

// Harness only. Both strategies are
// MinimumDeletionsToMakeAlternatingSubstringSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class MinimumDeletionsToMakeAlternatingSubstringTests
{
    public static TheoryData<string, int[][], int[]> Examples =>
        new()
        {
            { "ABA", [[2, 1, 2], [1, 1], [2, 0, 2]], [0, 2] },
            { "ABB", [[2, 0, 2], [1, 2], [2, 0, 2]], [1, 0] },
            { "BABA", [[2, 0, 3], [1, 1], [2, 1, 3]], [0, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerQueriesByDirectScan_LeetCodeExamples_ReturnsDeletionCounts(
        string text, int[][] queries, int[] expected)
    {
        var actual = MinimumDeletionsToMakeAlternatingSubstringSolution.AnswerQueriesByDirectScan(text, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerQueriesByFenwickAdjacency_LeetCodeExamples_ReturnsDeletionCounts(
        string text, int[][] queries, int[] expected)
    {
        var actual = MinimumDeletionsToMakeAlternatingSubstringSolution.AnswerQueriesByFenwickAdjacency(text, queries);

        Assert.Equal(expected, actual);
    }
}
