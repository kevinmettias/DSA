using DSAExperimentation.LeetCode.ShortestUncommonSubstringInAnArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestUncommonSubstringInAnArray;

// Harness only. Both strategies are
// ShortestUncommonSubstringInAnArraySolution's - this file just pins them to
// LeetCode's published examples.
public sealed partial class ShortestUncommonSubstringInAnArrayTests
{
    public static TheoryData<string[], string[]> Examples =>
        new()
        {
            { ["cab", "ad", "bad", "c"], ["ab", "", "ba", ""] },
            { ["abc", "bcd", "abcd"], ["", "", "abcd"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindShortestUncommonSubstringsByBruteForce_LeetCodeExamples_ReturnsShortestUncommonSubstrings(
        string[] arr, string[] expected) =>
        Assert.Equal(expected, ShortestUncommonSubstringInAnArraySolution.FindShortestUncommonSubstringsByBruteForce(arr));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindShortestUncommonSubstringsByAhoCorasick_LeetCodeExamples_ReturnsShortestUncommonSubstrings(
        string[] arr, string[] expected) =>
        Assert.Equal(expected, ShortestUncommonSubstringInAnArraySolution.FindShortestUncommonSubstringsByAhoCorasick(arr));
}
