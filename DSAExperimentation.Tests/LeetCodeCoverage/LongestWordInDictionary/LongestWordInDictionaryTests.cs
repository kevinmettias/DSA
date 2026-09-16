using DSAExperimentation.LeetCode.LongestWordInDictionary;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestWordInDictionary;

// Harness only. Both strategies are LongestWordInDictionarySolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class LongestWordInDictionaryTests
{
    public static TheoryData<string[], string> Examples =>
        new()
        {
            { ["w", "wo", "wor", "worl", "world"], "world" },
            { ["a", "banana", "app", "appl", "ap", "apply", "apple"], "apple" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestWordByDictionaryScan_LeetCodeExamples_ReturnsLongestBuildableWord(
        string[] words, string expected) =>
        Assert.Equal(expected, LongestWordInDictionarySolution.LongestWordByDictionaryScan(words));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestWordByLowercaseTrieWalk_LeetCodeExamples_ReturnsLongestBuildableWord(
        string[] words, string expected) =>
        Assert.Equal(expected, LongestWordInDictionarySolution.LongestWordByLowercaseTrieWalk(words));
}
