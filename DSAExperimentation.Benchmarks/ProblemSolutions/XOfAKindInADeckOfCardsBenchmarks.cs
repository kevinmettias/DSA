using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// X of a Kind in a Deck of Cards (LC 914): counting each card value with the BCL's
// Dictionary vs. counting with this repo's own HashMap<int,int> - the same
// "same algorithm, BCL structures vs. repo structures" contrast
// HandOfStraightsBenchmarks already draws - then folding GCD across every count
// either way. The deck is built from whole groups of a fixed size so a partition
// always exists and both benchmarks are forced through their full counting pass.
[MemoryDiagnoser]
public class XOfAKindInADeckOfCardsBenchmarks
{
    private const int GroupSize = 4;
    private const int RandomSeed = 914; // LC problem number
    private const int MinPartitionSize = 2; // X of a kind requires groups of at least 2

    [Params(400, 20_000)]
    public int DeckSize;

    private int[] _deck = null!;

    [GlobalSetup]
    public void Setup() => _deck = BuildPartitionableDeck(DeckSize, GroupSize, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public bool DictionaryCount()
    {
        var counts = new Dictionary<int, int>();

        foreach (var card in _deck)
        {
            counts[card] = counts.GetValueOrDefault(card) + 1;
        }

        var gcd = 0;
        foreach (var count in counts.Values)
        {
            gcd = Gcd(gcd, count);
        }

        return gcd >= MinPartitionSize;
    }

    [Benchmark]
    public bool HashMapCount()
    {
        var counts = new HashMap<int, int>();

        foreach (var card in _deck)
        {
            counts.TryGetValue(card, out var count);
            counts.Set(card, count + 1);
        }

        var gcd = 0;
        foreach (var count in counts.Values)
        {
            gcd = Gcd(gcd, count);
        }

        return gcd >= MinPartitionSize;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    // deckSize / groupSize distinct card values, each repeated groupSize times, then
    // shuffled - guarantees every group's count shares the common factor groupSize.
    private static int[] BuildPartitionableDeck(int deckSize, int groupSize, int seed)
    {
        var random = new Random(seed);
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

        for (var i = deck.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }

        return deck;
    }
}
