namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 914 - deckSize / groupSize distinct card values,
// each repeated groupSize times, then shuffled. Every count then shares the common
// factor groupSize, so a partition always exists and both strategies are forced
// through their full counting pass rather than stopping at a GCD of 1.
internal static class XOfAKindInADeckOfCardsWorkloads
{
    public static int[] BuildPartitionableDeck(int deckSize, int groupSize, int seed)
    {
        var random = new Random(seed);
        var deck = BuildGroupedDeck(deckSize, groupSize);

        Shuffle(deck, random);

        return deck;
    }

    // groupSize consecutive copies of each of the deckSize / groupSize distinct values.
    private static int[] BuildGroupedDeck(int deckSize, int groupSize)
    {
        var distinctValues = deckSize / groupSize;
        var deck = new int[distinctValues * groupSize];
        var index = 0;

        for (var value = 0; value < distinctValues; value++)
        {
            for (var copy = 0; copy < groupSize; copy++)
            {
                deck[index++] = value;
            }
        }

        return deck;
    }

    private static void Shuffle(int[] deck, Random random)
    {
        for (var i = deck.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }
    }
}
