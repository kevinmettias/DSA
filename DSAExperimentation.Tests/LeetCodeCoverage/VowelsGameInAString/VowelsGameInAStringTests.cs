using DSAExperimentation.LeetCode.VowelsGameInAString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.VowelsGameInAString;

// Harness only. Both strategies are VowelsGameInAStringSolution's - this file
// pins them to LeetCode's published examples.
public sealed class VowelsGameInAStringTests
{
    public static TheoryData<GameCase> Examples =>
        new()
        {
            { new GameCase(S: "leetcoder", Expected: true) },
            { new GameCase(S: "bbcd", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByGameSearch_LeetCodeExamples_ReturnsWhetherAliceWins(GameCase example)
    {
        var aliceWins = VowelsGameInAStringSolution.CanAliceWinByGameSearch(example.S);

        Assert.Equal(example.Expected, aliceWins);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByVowelExistence_LeetCodeExamples_ReturnsWhetherAliceWins(GameCase example)
    {
        var aliceWins = VowelsGameInAStringSolution.CanAliceWinByVowelExistence(example.S);

        Assert.Equal(example.Expected, aliceWins);
    }

    // One LeetCode example: the starting string, and whether Alice wins the vowel-erasing
    // game played over it. Nested because it is only ever used inside this test class -
    // it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct GameCase(string S, bool Expected);
}
