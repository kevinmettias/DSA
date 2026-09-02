using DSAExperimentation.LeetCode.CountPrefixAndSuffixPairsI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountPrefixAndSuffixPairsI;

// Harness only: both counting strategies live in
// CountPrefixAndSuffixPairsISolution - this file just pins them to LeetCode's
// published examples.
public sealed class CountPrefixAndSuffixPairsITests
{
    public static TheoryData<string[], int> Examples =>
        new()
        {
            { ["a", "aba", "ababa", "aa"], 4 },
            { ["pa", "papa", "ma", "mama"], 2 },
            { ["abab", "ab"], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByBruteForce_LeetCodeExamples_ReturnsPrefixAndSuffixPairCount(
        string[] words, int expected) =>
        Assert.Equal(expected, CountPrefixAndSuffixPairsISolution.CountPairsByBruteForce(words));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByRollingHash_LeetCodeExamples_ReturnsPrefixAndSuffixPairCount(
        string[] words, int expected) =>
        Assert.Equal(expected, CountPrefixAndSuffixPairsISolution.CountPairsByRollingHash(words));
}
