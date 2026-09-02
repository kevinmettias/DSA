using DSAExperimentation.LeetCode.PalindromePairs;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromePairs;

// Harness only. Both strategies are PalindromePairsSolution's - this file just pins
// them to LeetCode's published examples, expressed as sets of index pairs since a
// word may pair with more than one other word and pair order isn't specified.
public sealed class PalindromePairsTests
{
    public static TheoryData<string[], (int First, int Second)[]> Examples =>
        new()
        {
            {
                // "dcba"+"abcd", "abcd"+"dcba", "s"+"lls", "lls"+"sssll" (llssssll) all read the same forwards and backwards.
                ["abcd", "dcba", "lls", "s", "sssll"],
                [(0, 1), (1, 0), (3, 2), (2, 4)]
            },
            { ["bat", "tab", "cat"], [(0, 1), (1, 0)] },
            { ["a", ""], [(0, 1), (1, 0)] },
            { ["abc", "xyz"], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindPairsByBruteForce_LeetCodeExamples_ReturnsAllValidPairs(
        string[] words, (int First, int Second)[] expected) =>
        Assert.Equal(expected.ToHashSet(), PalindromePairsSolution.FindPairsByBruteForce(words).ToHashSet());

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindPairsByHashMapComplementLookup_LeetCodeExamples_ReturnsAllValidPairs(
        string[] words, (int First, int Second)[] expected) =>
        Assert.Equal(
            expected.ToHashSet(), PalindromePairsSolution.FindPairsByHashMapComplementLookup(words).ToHashSet());
}
