using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.LeetCode.PrefixAndSuffixSearch;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Prefix and Suffix Search (LC 745): the naive per-query O(numWords * wordLength)
// StartsWith/EndsWith scan vs. a one-time O(numWords * wordLength^2) precompute of
// every (prefix, suffix) substring pair into this repo's own HashMap<string,int>,
// after which every query is a single lookup. Both arms are
// PrefixAndSuffixSearchSolution's own strategies, the same methods
// PrefixAndSuffixSearchTests proves correct; the precomputed arm is handed the
// index its hoisted overload takes, built once in [GlobalSetup], so the one-time
// precompute LeetCode's own constructor/query split represents isn't charged to
// every measured query. Words are random fixed-length strings so a query's own word
// is very unlikely to share its prefix+suffix combo with another random word,
// keeping both strategies' answers aligned without affecting either one's cost.
[MemoryDiagnoser]
public class PrefixAndSuffixSearchBenchmarks
{
    private const int RandomSeed = 745; // LC 745
    private const int WordLength = 7;
    private const int AlphabetSize = 26;

    // Length of the prefix/suffix pulled from each word to build a query.
    private const int QueryAffixLength = 2;

    private string[] _words = [];

    private (string Prefix, string Suffix)[] _queries = [];
    private HashMap<string, int> _index = new();
    [Params(200, 2_000)]
    public int WordCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _words = Enumerable.Range(0, WordCount)
            .Select(_ => new string(Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray()))
            .ToArray();
        _queries = _words.Select(word => (word[..QueryAffixLength], word[^QueryAffixLength..])).ToArray();
        _index = PrefixAndSuffixSearchSolution.BuildPrefixSuffixIndex(_words);
    }

    [Benchmark(Baseline = true)]
    public int LinearScanPerQuery()
    {
        var found = -1;

        foreach (var (prefix, suffix) in _queries)
        {
            found = PrefixAndSuffixSearchSolution.SearchByLinearScan(
                _words, new SearchPrefix(prefix), new SearchSuffix(suffix));
        }

        return found;
    }

    [Benchmark]
    public int PrecomputedHashMapLookup()
    {
        var found = -1;

        foreach (var (prefix, suffix) in _queries)
        {
            found = PrefixAndSuffixSearchSolution.SearchByPrecomputedHashMap(
                _index, new SearchPrefix(prefix), new SearchSuffix(suffix));
        }

        return found;
    }
}
