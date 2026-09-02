using DSAExperimentation.LeetCode.VowelsGameInAString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.VowelsGameInAString;

// Harness only. Both strategies are VowelsGameInAStringSolution's - this file
// pins them to LeetCode's published examples.
public sealed class VowelsGameInAStringTests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            { "leetcoder", true },
            { "bbcd", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DoesAliceWinByGameSearch_LeetCodeExamples_ReturnsWhetherAliceWins(string s, bool expected) =>
        Assert.Equal(expected, VowelsGameInAStringSolution.DoesAliceWinByGameSearch(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DoesAliceWinByVowelExistence_LeetCodeExamples_ReturnsWhetherAliceWins(string s, bool expected) =>
        Assert.Equal(expected, VowelsGameInAStringSolution.DoesAliceWinByVowelExistence(s));
}
