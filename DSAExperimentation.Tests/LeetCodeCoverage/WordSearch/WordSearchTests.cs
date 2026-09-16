using DSAExperimentation.LeetCode.WordSearch;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordSearch;

// Harness only. Both search strategies are WordSearchSolution's - this file just
// pins them to LeetCode's published examples over its own board.
public sealed class WordSearchTests
{
    private static readonly char[][] Board =
        [['A', 'B', 'C', 'E'], ['S', 'F', 'C', 'S'], ['A', 'D', 'E', 'E']];

    public static TheoryData<WordCase> Examples =>
        new()
        {
            { new WordCase(Word: "ABCCED", Expected: true) },
            { new WordCase(Word: "SEE", Expected: true) },
            { new WordCase(Word: "ABCB", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ExistByBruteForceDfs_LeetCodeExamples_ReturnsWhetherWordCanBeTraced(WordCase example)
    {
        var actual = WordSearchSolution.ExistByBruteForceDfs(Board, example.Word);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ExistByBacktrack_LeetCodeExamples_ReturnsWhetherWordCanBeTraced(WordCase example)
    {
        var actual = WordSearchSolution.ExistByBacktrack(Board, example.Word);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the word to trace over the board above, and whether it can
    // be spelled by a path of adjacent cells that never reuses a cell. Nested because it
    // is only ever used inside this test class - it is this harness's own vocabulary,
    // not a type another file would import.
    public readonly record struct WordCase(string Word, bool Expected);
}
