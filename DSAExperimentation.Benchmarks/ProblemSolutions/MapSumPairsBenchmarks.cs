using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Map Sum Pairs (LC 677): scanning every inserted key with string.StartsWith for
// every prefix query (same ReplaceWordsBenchmarks/ImplementMagicDictionaryBenchmarks
// precedent - both benchmark methods process a full batch of prefix queries per
// call, not just one, so the trie's one-time build cost is amortized the same way)
// vs. this repo's own LowercaseTrie<int> plus TreeFold.Fold(SumValuesAlgebra) over
// the prefix node's subtree. O(prefixes * keyCount * keyLength) vs. O(keyCount *
// keyLength) to build the trie once plus O(prefixes * (prefixLength +
// matchingSubtreeSize)) to answer every query. Each query prefix is a real leading
// substring of one of the inserted keys, guaranteeing at least one match instead of
// letting either strategy bail out early on "no keys share this prefix."
[MemoryDiagnoser]
public class MapSumPairsBenchmarks
{
    private const int KeyLength = 8;
    private const int PrefixLength = 3;
    private const int RandomSeed = 677; // LC problem number
    private const int MaxValueExclusive = 100;
    private const int AlphabetSize = 26;

    [Params(5_000, 20_000)]
    public int KeyCount;

    private string[] _keys = null!;
    private int[] _values = null!;
    private string[] _prefixes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _keys = Enumerable.Range(0, KeyCount).Select(_ => RandomWord(random)).Distinct().ToArray();
        _values = _keys.Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _prefixes = _keys.Select(key => key[..PrefixLength]).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long DictionaryScan()
    {
        var total = 0L;

        foreach (var prefix in _prefixes)
        {
            for (var i = 0; i < _keys.Length; i++)
            {
                if (_keys[i].StartsWith(prefix, StringComparison.Ordinal))
                {
                    total += _values[i];
                }
            }
        }

        return total;
    }

    [Benchmark]
    public long TrieFoldSum()
    {
        var trie = new LowercaseTrie<int>();

        for (var i = 0; i < _keys.Length; i++)
        {
            trie.Set(_keys[i], _values[i]);
        }

        var total = 0L;

        foreach (var prefix in _prefixes)
        {
            total += Sum(trie.Root, prefix);
        }

        return total;
    }

    private static int Sum(LowercaseTrieNode<int> root, string prefix)
    {
        var subtreeRoot = WalkTo(root, prefix);

        return TreeFold.Fold<
            LowercaseTrieNode<int>, LowercaseTrieTopology<int>, SparseArrayChildren<LowercaseTrieNode<int>>,
            NaturalChildOrder<LowercaseTrieNode<int>, SparseArrayChildren<LowercaseTrieNode<int>>>,
            SparseArrayChildren<LowercaseTrieNode<int>>, SumValuesAlgebra, int>(subtreeRoot);
    }

    private static LowercaseTrieNode<int>? WalkTo(LowercaseTrieNode<int> root, string prefix)
    {
        var current = root;

        foreach (var ch in prefix)
        {
            current = current.Children[ch - 'a'];

            if (current is null)
            {
                return null;
            }
        }

        return current;
    }

    private static string RandomWord(Random random)
        => new(Enumerable.Range(0, KeyLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());

    // See MapSumPairsTests.SumValuesAlgebra for the full explanation - repeated here
    // rather than shared because TwoSumBenchmarks/MedianOfTwoSortedArraysBenchmarks
    // establish this project keeps its own copy of the solution rather than
    // depending on the Tests project.
    private readonly struct SumValuesAlgebra : IFoldAlgebra<LowercaseTrieNode<int>, int>
    {
        public static int Empty => 0;

        public static int Combine(LowercaseTrieNode<int> node, IReadOnlyList<int> children)
            => (node.HasValue ? node.Value : 0) + children.Sum();
    }
}
