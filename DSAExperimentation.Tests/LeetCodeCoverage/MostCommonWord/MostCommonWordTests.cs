using DSAExperimentation.LeetCode.MostCommonWord;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MostCommonWord;

// Harness only. Both tallying strategies are MostCommonWordSolution's - this file
// just pins them to LeetCode's published examples, including the mixed-case and
// punctuation-heavy cases that decide whether tokenization agrees with LeetCode's
// own "separated by spaces and/or punctuation" definition.
public sealed class MostCommonWordTests
{
    public static TheoryData<string, string[], string> Examples =>
        new()
        {
            { "Bob hit a ball, the hit BALL flew far after it was hit.", ["hit"], "ball" },
            { "a.", [], "a" },
            { "Bob. hIt, baLl", ["bob", "hit"], "ball" },
            { "a, a, a, b, b.", ["a"], "b" },
            { "Bob hit a ball", ["bob", "hit", "a"], "ball" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MostCommonByDictionaryScan_LeetCodeExamples_ReturnsMostFrequentUnbannedWord(
        string paragraph, string[] banned, string expected)
    {
        var actual = MostCommonWordSolution.MostCommonByDictionaryScan(paragraph, banned);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MostCommonByHashMapTally_LeetCodeExamples_ReturnsMostFrequentUnbannedWord(
        string paragraph, string[] banned, string expected)
    {
        var actual = MostCommonWordSolution.MostCommonByHashMapTally(paragraph, banned);

        Assert.Equal(expected, actual);
    }
}
