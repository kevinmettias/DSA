using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.HandOfStraights;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HandOfStraightsSolution's, the same methods
// HandOfStraightsTests proves correct - counting with the BCL's Dictionary and
// sorting with Array.Sort vs. counting with this repo's own HashMap<int,int> and
// sorting with this repo's own MergeSort, the same "same greedy algorithm, BCL
// structures vs. repo structures" contrast TopKFrequentElementsBenchmarks already
// draws for counting, extended here to the sort step. The hand is built from
// whole, non-overlapping groupSize runs, so it always straightens and both arms
// are forced through their full group-forming sweep instead of one returning early
// on the first missing card.
[MemoryDiagnoser]
public class HandOfStraightsBenchmarks
{
    private const int GroupSize = 5;

    // LeetCode problem number, reused as the RNG seed for reproducible benchmark input.
    private const int HandSeed = 846;

    private int[] _hand = [];

    [Params(500, 20_000)]
    public int HandCount { get; set; }

    [GlobalSetup]
    public void Setup() => _hand = BuildStraightableHand(HandCount, GroupSize, seed: HandSeed);

    // The hand is shuffled with a seeded RNG; BuildConsecutiveRuns lays down the runs it
    // shuffles, and the draw sequence below is what pins every benchmark's input.
    private static int[] BuildStraightableHand(int handCount, int groupSize, int seed)
    {
        var random = new Random(seed);
        var hand = BuildConsecutiveRuns(handCount, groupSize);

        for (var i = hand.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (hand[i], hand[j]) = (hand[j], hand[i]);
        }

        return hand;
    }

    // groupCount whole runs of groupSize consecutive values, each run starting at a
    // widely-spaced multiple of groupSize so runs never overlap.
    private static int[] BuildConsecutiveRuns(int handCount, int groupSize)
    {
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

        return hand;
    }

    [Benchmark(Baseline = true)]
    public bool DictionaryThenArraySort() =>
        HandOfStraightsSolution.IsNStraightHandByBclDictionary(_hand, GroupSize);

    [Benchmark]
    public bool HashMapThenMergeSort() =>
        HandOfStraightsSolution.IsNStraightHandByHashMapMergeSort(_hand, GroupSize);
}
