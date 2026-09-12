using DSAExperimentation.LeetCode.PalindromicSubstrings;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromicSubstrings;

// Harness only. Both strategies are PalindromicSubstringsSolution's - this file
// just pins them to LeetCode's published examples plus the mixed and
// single-character cases the original test also covered.
public sealed class PalindromicSubstringsTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "abc", 3 }, // "a", "b", "c"
            { "aaa", 6 }, // "a","a","a","aa","aa","aaa"
            { "aba", 4 }, // "a","b","a","aba"
            { "a", 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSubstringsByExpandAroundCenter_LeetCodeExamples_CountsPalindromicSubstrings(
        string s, int expected) =>
        Assert.Equal(expected, PalindromicSubstringsSolution.CountSubstringsByExpandAroundCenter(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSubstringsByManacher_LeetCodeExamples_CountsPalindromicSubstrings(
        string s, int expected) =>
        Assert.Equal(expected, PalindromicSubstringsSolution.CountSubstringsByManacher(s));
}
