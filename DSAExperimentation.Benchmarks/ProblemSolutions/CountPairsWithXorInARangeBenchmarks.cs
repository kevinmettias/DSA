using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Pairs With XOR in a Range (LC 1803): the textbook O(n^2) pairwise scan vs.
// this repo's own BitTrie (DataStructures/Graph/Engines/Dags/Trees/BitTrie.cs) plus
// HashMap<TKey,TValue> keyed by BitTrieNode reference identity, annotating each
// trie node with how many inserted values pass through it - the classic trie
// range-count technique, O(32) per insert/query instead of the O(n) per-value
// pairwise rescan (CountPairsWithXorInARangeTests precedent).
[MemoryDiagnoser]
public class CountPairsWithXorInARangeBenchmarks
{
    private const int Low = 100;
    private const int High = 5_000;
    private const int ValueUpperBound = 20_000;
    private const int TopBitIndex = 31;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScan()
    {
        var count = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            for (var j = i + 1; j < _values.Length; j++)
            {
                var xor = _values[i] ^ _values[j];
                if (xor >= Low && xor <= High)
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Benchmark]
    public int BitTrieRangeCount()
    {
        var trie = new BitTrie();
        var subtreeCount = new HashMap<BitTrieNode, int>();
        var count = 0;

        foreach (var value in _values)
        {
            count += CountLessThan(trie, subtreeCount, value, High + 1) - CountLessThan(trie, subtreeCount, value, Low);
            Insert(trie, subtreeCount, value);
        }

        return count;
    }

    private static void Insert(BitTrie trie, HashMap<BitTrieNode, int> subtreeCount, int value)
    {
        trie.Insert(value);

        var current = trie.Root;
        var bits = unchecked((uint)value);

        for (var i = TopBitIndex; i >= 0; i--)
        {
            var bit = (bits >> i) & 1u;
            current = bit == 0 ? current.Zero! : current.One!;
            subtreeCount.TryGetValue(current, out var existing);
            subtreeCount.Set(current, existing + 1);
        }
    }

    private readonly record struct XorSearchKeys(uint ValueBits, uint LimitBits);

    private static int CountLessThan(BitTrie trie, HashMap<BitTrieNode, int> subtreeCount, int value, int limit)
    {
        if (limit <= 0 || trie.Count == 0)
        {
            return 0;
        }

        BitTrieNode? current = trie.Root;
        var keys = new XorSearchKeys(unchecked((uint)value), unchecked((uint)limit));
        var count = 0;

        for (var i = TopBitIndex; i >= 0 && current is not null; i--)
        {
            var (next, delta) = AdvanceBit(current, subtreeCount, keys, i);
            current = next;
            count += delta;
        }

        return count;
    }

    private static (BitTrieNode? Next, int Delta) AdvanceBit(
        BitTrieNode current, HashMap<BitTrieNode, int> subtreeCount, XorSearchKeys keys, int bitIndex)
    {
        var valueBit = (keys.ValueBits >> bitIndex) & 1u;
        var limitBit = (keys.LimitBits >> bitIndex) & 1u;
        var sameChild = valueBit == 0 ? current.Zero : current.One;
        var oppositeChild = valueBit == 0 ? current.One : current.Zero;

        if (limitBit != 1)
        {
            return (sameChild, 0);
        }

        var delta = 0;
        if (sameChild is not null && subtreeCount.TryGetValue(sameChild, out var sameCount))
        {
            delta = sameCount;
        }

        return (oppositeChild, delta);
    }
}
