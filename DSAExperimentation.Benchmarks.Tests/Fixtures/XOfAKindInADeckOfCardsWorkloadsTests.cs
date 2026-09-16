using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for XOfAKindInADeckOfCardsWorkloads (ARCHITECTURE 17.7): LC 914 asks whether a deck can be
// split into equal-sized groups. The fixture's promise is that one always exists - deckSize / groupSize
// distinct values each repeated groupSize times, then shuffled - so every count shares the factor groupSize
// and both strategies run their full counting pass instead of stopping at a GCD of 1. The shuffle is what
// keeps the grouping from being readable straight off the deck's order.
public sealed partial class XOfAKindInADeckOfCardsWorkloadsTests
{
    private const int DeckSize = 100;
    private const int GroupSize = 4;
    private const int Seed = 914; // LC problem number
    private const int SmallestPartitioningFactor = 2;
    private const int NoRemainder = 0;

    [Fact]
    public void BuildPartitionableDeck_SeededShuffle_KeepsOneGroupPerDistinctValueOfTheGroupSize()
    {
        var deck = XOfAKindInADeckOfCardsWorkloads.BuildPartitionableDeck(DeckSize, GroupSize, Seed);
        var counts = deck.GroupBy(value => value).Select(group => group.Count()).ToList();

        Assert.Equal(DeckSize, deck.Length);
        Assert.Equal(DistinctValueCount, counts.Count);
        Assert.All(counts, count => Assert.Equal(GroupSize, count));
    }

    // A partition exists exactly when every count shares a factor above one, which is the guarantee the two
    // strategies are measured against - so the shared factor is asserted, not only the counts.
    [Fact]
    public void BuildPartitionableDeck_EveryCount_SharesAFactorThatAllowsAPartition()
    {
        var deck = XOfAKindInADeckOfCardsWorkloads.BuildPartitionableDeck(DeckSize, GroupSize, Seed);
        var counts = deck.GroupBy(value => value).Select(group => group.Count()).ToList();

        Assert.All(counts, count => Assert.Equal(NoRemainder, count % GroupSize));
        Assert.InRange(GreatestCommonDivisor(counts), SmallestPartitioningFactor, GroupSize);
    }

    [Fact]
    public void BuildPartitionableDeck_SameSeed_ReturnsTheSameShuffle() =>
        Assert.Equal(
            XOfAKindInADeckOfCardsWorkloads.BuildPartitionableDeck(DeckSize, GroupSize, Seed),
            XOfAKindInADeckOfCardsWorkloads.BuildPartitionableDeck(DeckSize, GroupSize, Seed));

    private static int DistinctValueCount => DeckSize / GroupSize;

    private static int GreatestCommonDivisor(List<int> counts) =>
        counts.Aggregate((running, count) => GreatestCommonDivisor(running, count));

    private static int GreatestCommonDivisor(int first, int second) =>
        second == 0 ? first : GreatestCommonDivisor(second, first % second);
}
