using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Prefix and Suffix Search (LC 745): the naive per-query O(numWords * wordLength)
// StartsWith/EndsWith scan vs. a one-time O(numWords * wordLength^2) precompute of
// every (prefix, suffix) substring pair into this repo's own HashMap<string,int>
// (NextGreaterElementIBenchmarks' precompute-once-then-O(1)-query shape), after which
// every query is a single lookup. Words are random fixed-length strings so a query's
// own word is very unlikely to share its prefix+suffix combo with another random
// word, keeping both strategies' answers aligned without affecting either one's cost.
[MemoryDiagnoser]
public class PrefixAndSuffixSearchBenchmarks
{
    private const int RandomSeed = 745; // LC 745
    private const int WordLength = 7;
    private const int AlphabetSize = 26;

    // Length of the prefix/suffix pulled from each word to build a query.
    private const int QueryAffixLength = 2;

    private const string PrefixSuffixSeparator = "#";

    [Params(200, 2_000)]
    public int WordCount;

    private string[] _words = null!;
    private (string Prefix, string Suffix)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _words = Enumerable.Range(0, WordCount)
            .Select(_ => new string(Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray()))
            .ToArray();
        _queries = _words.Select(word => (word[..QueryAffixLength], word[^QueryAffixLength..])).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScanPerQuery()
    {
        var found = -1;

        foreach (var (prefix, suffix) in _queries)
        {
            for (var i = 0; i < _words.Length; i++)
            {
                if (_words[i].StartsWith(prefix, StringComparison.Ordinal)
                    && _words[i].EndsWith(suffix, StringComparison.Ordinal))
                {
                    found = i;
                }
            }
        }

        return found;
    }

    [Benchmark]
    public int PrecomputedHashMapLookup()
    {
        var indexByPrefixAndSuffix = BuildPrefixSuffixIndex();
        return FindLastMatchingIndex(indexByPrefixAndSuffix);
    }

    // One-time O(numWords * wordLength^2) precompute of every (prefix, suffix)
    // substring pair for every word, keyed to that word's index.
    private HashMap<string, int> BuildPrefixSuffixIndex()
    {
        var indexByPrefixAndSuffix = new HashMap<string, int>();

        for (var index = 0; index < _words.Length; index++)
        {
            var word = _words[index];

            for (var prefixLength = 0; prefixLength <= word.Length; prefixLength++)
            {
                for (var suffixLength = 0; suffixLength <= word.Length; suffixLength++)
                {
                    var key = word[..prefixLength] + PrefixSuffixSeparator + word[(word.Length - suffixLength)..];
                    indexByPrefixAndSuffix.Set(key, index);
                }
            }
        }

        return indexByPrefixAndSuffix;
    }

    // Runs every query as a single lookup, keeping the last match like the
    // linear-scan baseline does.
    private int FindLastMatchingIndex(HashMap<string, int> indexByPrefixAndSuffix)
    {
        var found = -1;

        foreach (var (prefix, suffix) in _queries)
        {
            if (indexByPrefixAndSuffix.TryGetValue(prefix + PrefixSuffixSeparator + suffix, out var index))
            {
                found = index;
            }
        }

        return found;
    }
}
