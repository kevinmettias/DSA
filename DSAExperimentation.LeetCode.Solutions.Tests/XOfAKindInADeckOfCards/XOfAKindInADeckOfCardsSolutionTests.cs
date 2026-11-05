using DSAExperimentation.LeetCode.Solutions.XOfAKindInADeckOfCards;

namespace DSAExperimentation.LeetCode.Solutions.Tests.XOfAKindInADeckOfCards;

// Harness only. Both tallies are XOfAKindInADeckOfCardsSolution's; this file pins
// them to the cases the GCD fold has to get
// right - a single card, a lone pair, unequal-but-commensurable counts, and counts
// whose only common divisor is 1.
public sealed partial class XOfAKindInADeckOfCardsSolutionTests
{
    public static TheoryData<DeckCase> Cases =>
        new()
        {
            { new DeckCase(Deck: [1, 1, 1, 2, 2, 2], Expected: true) },
            { new DeckCase(Deck: [1, 1, 1, 1, 2, 2, 2], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasGroupsSizeXByDictionaryCount_Cases_MatchesExpectedPartitionability(DeckCase testCase)
    {
        var hasGroups = XOfAKindInADeckOfCardsSolution.HasGroupsSizeXByDictionaryCount(testCase.Deck);

        Assert.Equal(testCase.Expected, hasGroups);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasGroupsSizeXByHashMapCount_Cases_MatchesExpectedPartitionability(DeckCase testCase)
    {
        var hasGroups = XOfAKindInADeckOfCardsSolution.HasGroupsSizeXByHashMapCount(testCase.Deck);

        Assert.Equal(testCase.Expected, hasGroups);
    }

    // One case: the deck, and whether it can be partitioned into groups of
    // the same size X >= 2 whose members all share a value. Nested because it is only
    // ever used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct DeckCase(int[] Deck, bool Expected);
}
