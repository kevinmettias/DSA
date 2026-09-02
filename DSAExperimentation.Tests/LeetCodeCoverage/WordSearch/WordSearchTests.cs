using DSAExperimentation.LeetCode.WordSearch;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordSearch;

// Harness only. Both search strategies are WordSearchSolution's - this file just
// pins them to LeetCode's published examples over its own board.
public sealed class WordSearchTests
{
    private static readonly char[][] Board =
        [['A', 'B', 'C', 'E'], ['S', 'F', 'C', 'S'], ['A', 'D', 'E', 'E']];

    public static TheoryData<string, bool> Examples =>
        new()
        {
            { "ABCCED", true },
            { "SEE", true },
            { "ABCB", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ExistByBruteForceDfs_LeetCodeExamples_ReturnsWhetherWordCanBeTraced(string word, bool expected) =>
        Assert.Equal(expected, WordSearchSolution.ExistByBruteForceDfs(Board, word));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ExistByBacktrack_LeetCodeExamples_ReturnsWhetherWordCanBeTraced(string word, bool expected) =>
        Assert.Equal(expected, WordSearchSolution.ExistByBacktrack(Board, word));
}
