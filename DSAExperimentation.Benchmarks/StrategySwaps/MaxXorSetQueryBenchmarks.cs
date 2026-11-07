using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// Maximum-XOR queries against a filled maximum-XOR set (IMaxXorSet), through each strategy for that
// layer: TwoFieldBitTrie, the hand-specialized set, against the one trie's BitTrie through its view
// BitTrieSet (ARCHITECTURE 13.9). Setup fills both sets with the same values, and each arm asks the
// same queries and answers the sum of their maximum XORs, so the ratio is the query walk, a branch
// on each bit to pick a named field against an indexed child slot, and nothing else. The sizes are
// LC 2932's bound on n and LC 2935's, the two problems that pick between these sets; values and
// queries alike spread over every non-negative int, as LC 421's do. The hand-specialized set is the
// baseline, as LC 208's textbook trie is (ARCHITECTURE 17.5).
public class MaxXorSetQueryBenchmarks
{
    private const int RandomSeed = 421; // LC problem number

    private int[] _queries = [];
    private TwoFieldBitTrie _twoFields;
    private BitTrieSet _indexedSlots;

    [Params(50, 50_000)]
    public int ValueCount { get; set; }

    // One stream draws the values, then the queries, so the two are not the same draw.
    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var values = SeededDraws.Values(ValueCount, 0, int.MaxValue, random);
        _queries = SeededDraws.Values(ValueCount, 0, int.MaxValue, random);
        _twoFields = Filled<TwoFieldBitTrie>(values);
        _indexedSlots = Filled<BitTrieSet>(values);
    }

    [Benchmark(Baseline = true)]
    public long TwoFields() => SumOfMaxXors(_twoFields);

    [Benchmark]
    public long IndexedSlots() => SumOfMaxXors(_indexedSlots);

    private static TSet Filled<TSet>(int[] values)
        where TSet : struct, IMaxXorSet<TSet>
    {
        var set = TSet.Empty();

        foreach (var value in values)
        {
            set.Insert(value);
        }

        return set;
    }

    // The one replay, compiled once for each set: each set is a struct, so each call is direct.
    private long SumOfMaxXors<TSet>(TSet set)
        where TSet : struct, IMaxXorSet<TSet>
    {
        long sum = 0;

        foreach (var query in _queries)
        {
            set.TryMaxXor(query, out var maxXor);
            sum += maxXor;
        }

        return sum;
    }
}
