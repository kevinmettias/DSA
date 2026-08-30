using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum XOR of Two Numbers in an Array (LC 421): the textbook O(n^2) pairwise
// scan vs. this repo's own BitTrie (DataStructures/Graph/Engines/Dags/Trees/
// BitTrie.cs) - insert every value's 32-bit pattern once (O(32) each), then for
// each value greedily walk toward the OPPOSITE bit at every level to find its best
// achievable XOR in O(32), for O(n) total instead of O(n^2).
[MemoryDiagnoser]
public class MaximumXOROfTwoNumbersInAnArrayBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, int.MaxValue)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScan()
    {
        var best = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            for (var j = i + 1; j < _values.Length; j++)
            {
                best = Math.Max(best, _values[i] ^ _values[j]);
            }
        }

        return best;
    }

    [Benchmark]
    public int BitTrieGreedy()
    {
        var trie = new BitTrie();

        foreach (var value in _values)
        {
            trie.Insert(value);
        }

        var best = 0;

        foreach (var value in _values)
        {
            if (trie.TryMaxXor(value, out var candidate))
            {
                best = Math.Max(best, candidate);
            }
        }

        return best;
    }
}
