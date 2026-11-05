using DSAExperimentation.LeetCode.Solutions.StringMatchingInAnArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StringMatchingInAnArray;

// Harness only. Both strategies are StringMatchingInAnArraySolution's -
// FindContainedWordsByNaiveScan (previously untested scaffolding inlined in the
// benchmark as its baseline arm, and only counting the contained words rather than
// building LC 1408's list) now gets the same cases as
// FindContainedWordsByPrefixFunctionSearch (previously the test's own private
// helper), so a failure names the strategy that broke.
public sealed partial class StringMatchingInAnArraySolutionTests
{
    public static TheoryData<string[], string[]> Cases =>
        new()
        {
            { ["ab", "abc", "abcd"], ["ab", "abc"] },
            { ["a", "a", "a"], ["a", "a", "a"] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindContainedWordsByNaiveScan_Cases_ReturnsWordsContainedInAnotherWord(
        string[] words, string[] expected) =>
        Assert.Equal(expected, StringMatchingInAnArraySolution.FindContainedWordsByNaiveScan(words));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindContainedWordsByPrefixFunctionSearch_Cases_ReturnsWordsContainedInAnotherWord(
        string[] words, string[] expected) =>
        Assert.Equal(expected, StringMatchingInAnArraySolution.FindContainedWordsByPrefixFunctionSearch(words));
}
