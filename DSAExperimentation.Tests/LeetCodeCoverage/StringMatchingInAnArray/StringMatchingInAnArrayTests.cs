using DSAExperimentation.LeetCode.StringMatchingInAnArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StringMatchingInAnArray;

// Harness only. Both strategies are StringMatchingInAnArraySolution's -
// FindContainedWordsByNaiveScan (previously untested scaffolding inlined in the
// benchmark as its baseline arm, and only counting the contained words rather than
// building LC 1408's list) now gets the same examples as
// FindContainedWordsByPrefixFunctionSearch (previously the test's own private
// helper), so a failure names the strategy that broke.
public sealed class StringMatchingInAnArrayTests
{
    public static TheoryData<string[], string[]> Examples =>
        new()
        {
            { ["mass", "as", "hero", "superhero"], ["as", "hero"] },
            { ["leetcode", "et", "code"], ["et", "code"] },
            { ["blue", "green", "bu"], [] },
            { ["ab", "abc", "abcd"], ["ab", "abc"] },
            { ["a", "a", "a"], ["a", "a", "a"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindContainedWordsByNaiveScan_LeetCodeExamples_ReturnsWordsContainedInAnotherWord(
        string[] words, string[] expected) =>
        Assert.Equal(expected, StringMatchingInAnArraySolution.FindContainedWordsByNaiveScan(words));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindContainedWordsByPrefixFunctionSearch_LeetCodeExamples_ReturnsWordsContainedInAnotherWord(
        string[] words, string[] expected) =>
        Assert.Equal(expected, StringMatchingInAnArraySolution.FindContainedWordsByPrefixFunctionSearch(words));
}
