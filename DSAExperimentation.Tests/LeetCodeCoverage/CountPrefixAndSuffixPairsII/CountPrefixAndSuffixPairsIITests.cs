using DSAExperimentation.LeetCode.CountPrefixAndSuffixPairsII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountPrefixAndSuffixPairsII;

// Harness only: both counting strategies live in
// CountPrefixAndSuffixPairsIISolution - this file just pins them to
// LeetCode's published examples, the same three CountPrefixAndSuffixPairsI
// uses for LC 3042's easy variant.
public sealed class CountPrefixAndSuffixPairsIITests
{
    public static TheoryData<string[], long> Examples =>
        new()
        {
            { ["a", "aba", "ababa", "aa"], 4 },
            { ["pa", "papa", "ma", "mama"], 2 },
            { ["abab", "ab"], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByBruteForce_LeetCodeExamples_ReturnsPrefixAndSuffixPairCount(
        string[] words, long expected) =>
        Assert.Equal(expected, CountPrefixAndSuffixPairsIISolution.CountPairsByBruteForce(words));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByLowercaseTrie_LeetCodeExamples_ReturnsPrefixAndSuffixPairCount(
        string[] words, long expected) =>
        Assert.Equal(expected, CountPrefixAndSuffixPairsIISolution.CountPairsByLowercaseTrie(words));
}
