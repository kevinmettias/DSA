using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.XOfAKindInADeckOfCards;

// LeetCode 914. X of a Kind in a Deck of Cards: can the deck be split into groups
// of the same size x > 1, every group holding one card value?
//
// Such a partition exists iff some x >= 2 divides every value's occurrence count,
// i.e. iff the GCD of all the counts is at least 2. Both strategies fold that same
// GCD; they differ only in what counts the occurrences - the BCL's Dictionary or
// this repo's own HashMap<int, int>.
internal static class XOfAKindInADeckOfCardsSolution
{
    // X of a kind requires groups of at least two cards.
    private const int MinPartitionSize = 2;

    // The textbook baseline: BCL Dictionary tally, written without this repo's
    // primitives - the arm the HashMap strategy below has to justify itself
    // against.
    public static bool HasGroupsSizeXByDictionaryCount(int[] deck)
    {
        var counts = new Dictionary<int, int>();

        foreach (var card in deck)
        {
            counts[card] = counts.GetValueOrDefault(card) + 1;
        }

        return IsPartitionable(counts.Values);
    }

    // The same tally in this repo's own HashMap<int, int>, whose read surface is
    // TryGetValue-then-Set rather than an indexer - the shape ContainsDuplicate
    // and SortCharactersByFrequency already use.
    public static bool HasGroupsSizeXByHashMapCount(int[] deck)
    {
        var counts = new HashMap<int, int>();

        foreach (var card in deck)
        {
            counts.TryGetValue(card, out var count);
            counts.Set(card, count + 1);
        }

        return IsPartitionable(counts.Values);
    }

    // GCD folded from 0, whose GCD with anything is that thing - so an empty deck
    // folds to 0 and correctly reports no partition.
    private static bool IsPartitionable(IEnumerable<int> counts)
    {
        var divisor = 0;

        foreach (var count in counts)
        {
            divisor = Gcd(divisor, count);
        }

        return divisor >= MinPartitionSize;
    }

    private static int Gcd(int first, int second) => second == 0 ? first : Gcd(second, first % second);
}
