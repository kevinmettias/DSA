using DSAExperimentation.LeetCode.XOfAKindInADeckOfCards;

namespace DSAExperimentation.Tests.LeetCodeCoverage.XOfAKindInADeckOfCards;

// Harness only. Both tallies are XOfAKindInADeckOfCardsSolution's; this file pins
// them to LeetCode's published examples plus the cases the GCD fold has to get
// right - a single card, a lone pair, unequal-but-commensurable counts, and counts
// whose only common divisor is 1.
public sealed partial class XOfAKindInADeckOfCardsTests
{
    public static TheoryData<DeckCase> Examples =>
        new()
        {
            { new DeckCase(Deck: [1, 2, 3, 4, 4, 3, 2, 1], Expected: true) },
            { new DeckCase(Deck: [1, 1, 1, 2, 2, 2, 3, 3], Expected: false) },
            { new DeckCase(Deck: [1], Expected: false) },
            { new DeckCase(Deck: [1, 1], Expected: true) },
            { new DeckCase(Deck: [1, 1, 2, 2, 2, 2], Expected: true) },
            { new DeckCase(Deck: [1, 1, 1, 2, 2, 2], Expected: true) },
            { new DeckCase(Deck: [1, 1, 1, 1, 2, 2, 2], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasGroupsSizeXByDictionaryCount_LeetCodeExamples_MatchesExpectedPartitionability(DeckCase example)
    {
        var hasGroups = XOfAKindInADeckOfCardsSolution.HasGroupsSizeXByDictionaryCount(example.Deck);

        Assert.Equal(example.Expected, hasGroups);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasGroupsSizeXByHashMapCount_LeetCodeExamples_MatchesExpectedPartitionability(DeckCase example)
    {
        var hasGroups = XOfAKindInADeckOfCardsSolution.HasGroupsSizeXByHashMapCount(example.Deck);

        Assert.Equal(example.Expected, hasGroups);
    }

    // One LeetCode example: the deck, and whether it can be partitioned into groups of
    // the same size X >= 2 whose members all share a value. Nested because it is only
    // ever used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct DeckCase(int[] Deck, bool Expected);
}
