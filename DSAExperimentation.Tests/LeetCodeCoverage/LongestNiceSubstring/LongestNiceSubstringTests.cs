using DSAExperimentation.LeetCode.LongestNiceSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestNiceSubstring;

// Harness only. Both strategies are LongestNiceSubstringSolution's -
// FindLongestNiceSubstringByBruteForceSubstrings (previously untested scaffolding
// inlined in the benchmark as its baseline arm) now gets the same examples as
// FindLongestNiceSubstringByDivideAndConquer (previously this file's own private
// helper), so a failure names the strategy that broke.
public sealed class LongestNiceSubstringTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "YazaAay", "aAa" },
            { "Bb", "Bb" },
            { "c", string.Empty },
            { "dDzeE", "dD" },
            { "abABB", "abABB" },
            { "cAaBb", "AaBb" },
            { "aaaa", string.Empty },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLongestNiceSubstringByBruteForceSubstrings_LeetCodeExamples_ReturnsEarliestLongestNiceRun(
        string s, string expected) =>
        Assert.Equal(expected, LongestNiceSubstringSolution.FindLongestNiceSubstringByBruteForceSubstrings(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLongestNiceSubstringByDivideAndConquer_LeetCodeExamples_ReturnsEarliestLongestNiceRun(
        string s, string expected) =>
        Assert.Equal(expected, LongestNiceSubstringSolution.FindLongestNiceSubstringByDivideAndConquer(s));
}
