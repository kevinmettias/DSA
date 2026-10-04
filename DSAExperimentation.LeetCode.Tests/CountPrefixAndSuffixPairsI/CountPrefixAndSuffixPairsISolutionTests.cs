using DSAExperimentation.LeetCode.CountPrefixAndSuffixPairsI;

namespace DSAExperimentation.LeetCode.Tests.CountPrefixAndSuffixPairsI;

// Harness only: both counting strategies live in
// CountPrefixAndSuffixPairsISolution - this file pins them to LeetCode's published
// examples. The per-word hashes the rolling-hash strategy is handed are asserted on
// their own: their values sit on ~10^9-scale default lanes no hand can follow, so the
// test pins each hash's length and which of its spans must agree.
public sealed partial class CountPrefixAndSuffixPairsISolutionTests
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

    // LeetCode's first example: one hash per word, in word order, as long as its word -
    // 1, 3, 5 and 2. "aba" opens "ababa" and closes it (at 5 - 3 = 2), and "a" opens
    // "aba", so those spans hash alike; "ab", the head of "aba", is not "aa", so those
    // two do not.
    [Fact]
    public void BuildHashes_LeetCodeFirstExample_HashesEachWordSoEqualSpansAgree()
    {
        var hashes = CountPrefixAndSuffixPairsISolution.BuildHashes(["a", "aba", "ababa", "aa"]);
        var lengths = hashes.Select(hash => hash.Length);
        var wholeA = hashes[0].Hash(0, 1);
        var headOfAba = hashes[1].Hash(0, 1);
        var wholeAba = hashes[1].Hash(0, 3);
        var headOfAbaba = hashes[2].Hash(0, 3);
        var tailOfAbaba = hashes[2].Hash(2, 3);
        var firstTwoOfAba = hashes[1].Hash(0, 2);
        var wholeAa = hashes[3].Hash(0, 2);

        Assert.Equal([1, 3, 5, 2], lengths);
        Assert.Equal(wholeA, headOfAba);
        Assert.Equal(wholeAba, headOfAbaba);
        Assert.Equal(wholeAba, tailOfAbaba);
        Assert.NotEqual(firstTwoOfAba, wholeAa);
    }
}
