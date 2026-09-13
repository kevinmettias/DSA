using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ShortEncodingOfWords;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are ShortEncodingOfWordsSolution's, the same methods
// ShortEncodingOfWordsTests proves correct - the textbook O(n^2 * L) pairwise
// EndsWith scan vs. this repo's own Set<string> (evict every proper suffix of each
// word as it is scanned) and its bounded-alphabet LowercaseTrie<TValue> (insert
// every REVERSED word; a word only needs its own encoding when its reversed node is
// a trie leaf). Words are generated from a small shared-suffix pool so real suffix
// redundancy exists for all three strategies to exploit, not just coincidental
// overlap. The input is LeetCode's own string[] shape, so there is nothing to hoist
// beyond generating it.
[MemoryDiagnoser]
public class ShortEncodingOfWordsBenchmarks
{
    private static readonly string[] SuffixPool =
        ["e", "me", "time", "bell", "ing", "ation", "tion", "er", "ed", "s"];

    // LC problem number, used as the RNG seed.
    private const int RandomSeed = 820;
    private const int MaxPrefixLength = 5;
    private const int AlphabetSize = 26;

    [Params(500, 5_000)]
    public int Length;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var words = new HashSet<string>();

        while (words.Count < Length)
        {
            var suffix = SuffixPool[random.Next(SuffixPool.Length)];
            var prefixLength = random.Next(0, MaxPrefixLength);
            var prefix = new char[prefixLength];

            for (var i = 0; i < prefixLength; i++)
            {
                prefix[i] = (char)('a' + random.Next(AlphabetSize));
            }

            words.Add(new string(prefix) + suffix);
        }

        _words = [.. words];
    }

    [Benchmark(Baseline = true)]
    public int MinimumLengthByPairwiseSuffixScan() =>
        ShortEncodingOfWordsSolution.MinimumLengthByPairwiseSuffixScan(_words);

    [Benchmark]
    public int MinimumLengthBySuffixEviction() =>
        ShortEncodingOfWordsSolution.MinimumLengthBySuffixEviction(_words);

    [Benchmark]
    public int MinimumLengthByReversedTrieLeaves() =>
        ShortEncodingOfWordsSolution.MinimumLengthByReversedTrieLeaves(_words);
}
