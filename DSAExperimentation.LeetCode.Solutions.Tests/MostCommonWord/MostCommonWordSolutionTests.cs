using DSAExperimentation.LeetCode.Solutions.MostCommonWord;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MostCommonWord;

// Harness only. Both tallying strategies are MostCommonWordSolution's - this file
// just pins them to the mixed-case and punctuation-heavy cases that decide whether
// tokenization agrees with LeetCode's own "separated by spaces and/or punctuation"
// definition, and to a paragraph with no punctuation at all. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class MostCommonWordSolutionTests
{
    public static TheoryData<string, string[], string> Cases =>
        new()
        {
            { "Bob. hIt, baLl", ["bob", "hit"], "ball" },
            { "a, a, a, b, b.", ["a"], "b" },
            { "Bob hit a ball", ["bob", "hit", "a"], "ball" },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MostCommonByDictionaryScan_Cases_ReturnsMostFrequentUnbannedWord(
        string paragraph, string[] banned, string expected)
    {
        var actual = MostCommonWordSolution.MostCommonByDictionaryScan(paragraph, banned);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MostCommonByHashMapTally_Cases_ReturnsMostFrequentUnbannedWord(
        string paragraph, string[] banned, string expected)
    {
        var actual = MostCommonWordSolution.MostCommonByHashMapTally(paragraph, banned);

        Assert.Equal(expected, actual);
    }
}
