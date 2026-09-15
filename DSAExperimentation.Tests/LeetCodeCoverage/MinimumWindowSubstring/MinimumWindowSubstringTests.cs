using DSAExperimentation.LeetCode.MinimumWindowSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumWindowSubstring;

// Harness only. Both strategies are MinimumWindowSubstringSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class MinimumWindowSubstringTests
{
    public static TheoryData<string, string, string> Examples =>
        new()
        {
            { "ADOBECODEBANC", "ABC", "BANC" },
            { "a", "a", "a" },
            { "a", "aa", "" },
            { "ab", "b", "b" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinWindowByBruteForce_LeetCodeExamples_ReturnsSmallestCoveringSubstring(
        string s, string t, string expected) =>
        Assert.Equal(
            expected,
            MinimumWindowSubstringSolution.MinWindowByBruteForce(
                new MinimumWindowSubstringSolution.SearchedText(s),
                new MinimumWindowSubstringSolution.RequiredCharacters(t)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinWindowBySlidingWindowHashMap_LeetCodeExamples_ReturnsSmallestCoveringSubstring(
        string s, string t, string expected) =>
        Assert.Equal(
            expected,
            MinimumWindowSubstringSolution.MinWindowBySlidingWindowHashMap(
                new MinimumWindowSubstringSolution.SearchedText(s),
                new MinimumWindowSubstringSolution.RequiredCharacters(t)));
}
