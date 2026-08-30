using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Hand of Straights (LC 846): counting with the BCL's Dictionary and sorting the
// whole hand with Array.Sort vs. counting with this repo's own HashMap<int,int> and
// sorting with this repo's own MergeSort (over an ArrayIndexedSequence<int>) - the
// same "same greedy algorithm, BCL structures vs. repo structures" contrast
// TopKFrequentElementsBenchmarks already draws for counting, extended here to the
// sort step neither of that problem's two benchmarks needed. The hand is built from
// whole, non-overlapping groupSize runs, so it always straightens and both
// benchmarks are forced through their full group-forming sweep instead of one
// returning early on the first missing card.
[MemoryDiagnoser]
public class HandOfStraightsBenchmarks
{
    private const int GroupSize = 5;

    [Params(500, 20_000)]
    public int HandCount;

    private int[] _hand = null!;

    [GlobalSetup]
    public void Setup() => _hand = BuildStraightableHand(HandCount, GroupSize, seed: 846);

    [Benchmark(Baseline = true)]
    public bool DictionaryThenArraySort() => IsNStraightHandBcl(_hand, GroupSize);

    [Benchmark]
    public bool HashMapThenMergeSort() => IsNStraightHandRepo(_hand, GroupSize);

    private static bool IsNStraightHandBcl(int[] hand, int groupSize)
    {
        var counts = new Dictionary<int, int>();

        foreach (var card in hand)
        {
            counts[card] = counts.GetValueOrDefault(card) + 1;
        }

        var sortedHand = (int[])hand.Clone();
        Array.Sort(sortedHand);

        foreach (var card in sortedHand)
        {
            if (counts[card] == 0)
            {
                continue;
            }

            for (var next = card; next < card + groupSize; next++)
            {
                if (!counts.TryGetValue(next, out var nextCount) || nextCount == 0)
                {
                    return false;
                }

                counts[next] = nextCount - 1;
            }
        }

        return true;
    }

    private static bool IsNStraightHandRepo(int[] hand, int groupSize)
    {
        var counts = new HashMap<int, int>();

        foreach (var card in hand)
        {
            counts.TryGetValue(card, out var count);
            counts.Set(card, count + 1);
        }

        var sortedHand = (int[])hand.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedHand));

        foreach (var card in sortedHand)
        {
            counts.TryGetValue(card, out var remaining);

            if (remaining == 0)
            {
                continue;
            }

            for (var next = card; next < card + groupSize; next++)
            {
                if (!counts.TryGetValue(next, out var nextCount) || nextCount == 0)
                {
                    return false;
                }

                counts.Set(next, nextCount - 1);
            }
        }

        return true;
    }

    // groupCount whole runs of groupSize consecutive values, each run starting at a
    // random, widely-spaced multiple of groupSize (so runs never overlap), then
    // shuffled - guarantees the hand always straightens.
    private static int[] BuildStraightableHand(int handCount, int groupSize, int seed)
    {
        var random = new Random(seed);
        var groupCount = handCount / groupSize;
        var hand = new int[groupCount * groupSize];
        var index = 0;

        for (var g = 0; g < groupCount; g++)
        {
            var start = g * groupSize;

            for (var offset = 0; offset < groupSize; offset++)
            {
                hand[index++] = start + offset;
            }
        }

        for (var i = hand.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (hand[i], hand[j]) = (hand[j], hand[i]);
        }

        return hand;
    }
}
