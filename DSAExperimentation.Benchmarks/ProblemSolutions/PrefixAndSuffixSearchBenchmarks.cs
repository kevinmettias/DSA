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
    [Params(200, 2_000)]
    public int WordCount;

    private string[] _words = null!;
    private (string Prefix, string Suffix)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(745);
        _words = Enumerable.Range(0, WordCount)
            .Select(_ => new string(Enumerable.Range(0, 7).Select(_ => (char)('a' + random.Next(26))).ToArray()))
            .ToArray();
        _queries = _words.Select(word => (word[..2], word[^2..])).ToArray();
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
        var indexByPrefixAndSuffix = new HashMap<string, int>();

        for (var index = 0; index < _words.Length; index++)
        {
            var word = _words[index];

            for (var prefixLength = 0; prefixLength <= word.Length; prefixLength++)
            {
                for (var suffixLength = 0; suffixLength <= word.Length; suffixLength++)
                {
                    var key = word[..prefixLength] + "#" + word[(word.Length - suffixLength)..];
                    indexByPrefixAndSuffix.Set(key, index);
                }
            }
        }

        var found = -1;

        foreach (var (prefix, suffix) in _queries)
        {
            if (indexByPrefixAndSuffix.TryGetValue(prefix + "#" + suffix, out var index))
            {
                found = index;
            }
        }

        return found;
    }
}
