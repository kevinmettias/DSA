using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// One fill of a maximum-XOR set (IMaxXorSet) from empty, through each strategy for that layer:
// TwoFieldBitTrie, the hand-specialized set, against the one trie's BitTrie through its view
// BitTrieSet (ARCHITECTURE 13.9). Both insert the same distinct values in the same order and answer
// how many they hold, so the ratio is the insert walk, two named fields against indexed child
// slots, and nothing else. The sizes are LC 2932's bound on n and LC 2935's, the two problems that
// pick between these sets. The hand-specialized set is the baseline, as LC 208's textbook trie is
// (ARCHITECTURE 17.5): what a LeetCode answer writes without this repository.
public class MaxXorSetInsertBenchmarks
{
    private const int RandomSeed = 139; // ARCHITECTURE 13.9, where the two sets are compared

    private int[] _values = [];

    [Params(50, 50_000)]
    public int ValueCount { get; set; }

    [GlobalSetup]
    public void Setup() => _values = SeededDraws.DistinctValues(ValueCount, 0, int.MaxValue, new Random(RandomSeed));

    [Benchmark(Baseline = true)]
    public int TwoFields() => Fill<TwoFieldBitTrie>();

    [Benchmark]
    public int IndexedSlots() => Fill<BitTrieSet>();

    // The one fill, compiled once for each set: each set is a struct, so each call is direct.
    private int Fill<TSet>()
        where TSet : struct, IMaxXorSet<TSet>
    {
        var set = TSet.Empty();

        foreach (var value in _values)
        {
            set.Insert(value);
        }

        return set.Count;
    }
}
