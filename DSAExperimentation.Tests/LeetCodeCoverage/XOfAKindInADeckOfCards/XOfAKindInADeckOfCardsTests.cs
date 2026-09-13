using DSAExperimentation.LeetCode.XOfAKindInADeckOfCards;

namespace DSAExperimentation.Tests.LeetCodeCoverage.XOfAKindInADeckOfCards;

// Harness only. Both tallies are XOfAKindInADeckOfCardsSolution's; this file pins
// them to LeetCode's published examples plus the cases the GCD fold has to get
// right - a single card, a lone pair, unequal-but-commensurable counts, and counts
// whose only common divisor is 1.
public sealed class XOfAKindInADeckOfCardsTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [1, 2, 3, 4, 4, 3, 2, 1], true },
            { [1, 1, 1, 2, 2, 2, 3, 3], false },
            { [1], false },
            { [1, 1], true },
            { [1, 1, 2, 2, 2, 2], true },
            { [1, 1, 1, 2, 2, 2], true },
            { [1, 1, 1, 1, 2, 2, 2], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasGroupsSizeXByDictionaryCount_LeetCodeExamples_MatchesExpectedPartitionability(
        int[] deck, bool expected) =>
        Assert.Equal(expected, XOfAKindInADeckOfCardsSolution.HasGroupsSizeXByDictionaryCount(deck));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasGroupsSizeXByHashMapCount_LeetCodeExamples_MatchesExpectedPartitionability(
        int[] deck, bool expected) =>
        Assert.Equal(expected, XOfAKindInADeckOfCardsSolution.HasGroupsSizeXByHashMapCount(deck));
}
