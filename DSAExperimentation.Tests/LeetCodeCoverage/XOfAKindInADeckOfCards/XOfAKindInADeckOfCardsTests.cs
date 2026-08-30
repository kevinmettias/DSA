using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.XOfAKindInADeckOfCards;

// LeetCode 914. X of a Kind in a Deck of Cards: a HashMap<int,int> counts each card
// value's occurrences (the same "TryGetValue then Set the incremented count" shape
// SortCharactersByFrequencyTests/ContainsDuplicateTests already use), then a partition
// of size x > 1 exists for every group iff the GCD of every count is at least 2.
public sealed partial class XOfAKindInADeckOfCardsTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 4, 3, 2, 1 }, true)]
    [InlineData(new[] { 1, 1, 1, 2, 2, 2, 3, 3 }, false)]
    [InlineData(new[] { 1 }, false)]
    [InlineData(new[] { 1, 1 }, true)]
    [InlineData(new[] { 1, 1, 2, 2, 2, 2 }, true)]
    public void HasGroupsSizeX_LeetCodeExamples_MatchesExpectedPartitionability(int[] deck, bool expected)
        => Assert.Equal(expected, HasGroupsSizeX(deck));

    private static bool HasGroupsSizeX(int[] deck)
    {
        var counts = new HashMap<int, int>();

        foreach (var card in deck)
        {
            counts.TryGetValue(card, out var count);
            counts.Set(card, count + 1);
        }

        var gcd = 0;
        foreach (var count in counts.Values)
        {
            gcd = Gcd(gcd, count);
        }

        return gcd >= 2;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
