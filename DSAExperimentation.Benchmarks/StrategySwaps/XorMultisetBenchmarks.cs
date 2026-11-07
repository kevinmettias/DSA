using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// A sliding window over a XOR multiset (IXorMultiset), through each strategy for that layer:
// PooledCountedBitTrie, the hand-specialized multiset whose nodes are slots in shared arrays,
// against the one trie's CountedBitTrie, whose nodes are linked objects, through its view
// CountedBitTrieMultiset (ARCHITECTURE 13.9). Each step does what the four users do between them:
// it inserts the next value, removes the value that leaves the window, and asks for the largest
// XOR with the new value and for how many held values have an XOR with it below Limit. Both arms
// run the same script and answer the sum of those answers, so the ratio is the layer's walks and
// node layout and nothing else. Values and sizes are LC 2935's, the largest of the users' bounds;
// the window holds a quarter of them. The hand-specialized multiset is the baseline, as
// TwoFieldBitTrie is in MaxXorSetQueryBenchmarks.
public class XorMultisetBenchmarks
{
    private const int RandomSeed = 1803; // LC problem number, the range count's
    private const int ValueBound = 1 << 20;
    private const int WindowShare = 4;

    // Half the values' range: a held value counts when its XOR with the new one leaves bit 19
    // clear, so the range count walks the whole path rather than stopping at its first bit.
    private const int Limit = ValueBound / 2;

    private int[] _values = [];

    [Params(500, 20_000)]
    public int ValueCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _values = SeededDraws.Values(ValueCount, 0, ValueBound, new Random(RandomSeed));

    [Benchmark(Baseline = true)]
    public long PooledArrays() => Slide<PooledCountedBitTrieMultiset>();

    [Benchmark]
    public long LinkedNodes() => Slide<CountedBitTrieMultiset>();

    // The one script, compiled once for each multiset: each is a struct, so each call is direct.
    private long Slide<TSet>()
        where TSet : struct, IXorMultiset<TSet>
    {
        var window = TSet.Empty();
        var width = ValueCount / WindowShare;
        long sum = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            var value = _values[i];
            window.Insert(value);

            if (i >= width)
            {
                window.TryRemove(_values[i - width]);
            }

            window.TryMaxXor(value, out var maxXor);
            sum += maxXor + window.CountXorBelow(value, Limit);
        }

        return sum;
    }
}
