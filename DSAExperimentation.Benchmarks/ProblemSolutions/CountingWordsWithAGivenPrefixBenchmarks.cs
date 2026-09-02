using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Counting Words With a Given Prefix (LC 2185): a direct string.StartsWith scan over
// every word vs. this repo's own Trie<bool> - one word inserted per candidate, then
// HasPrefix(pref) (CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarks' exact
// per-word Insert+HasPrefix pairing), counting matches instead of stopping at the
// first one. Words are randomly generated single-repeated-character strings (same
// generator CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarks uses), so a
// single-letter prefix gives a realistic, non-trivial ~1/26 match rate.
[MemoryDiagnoser]
public class CountingWordsWithAGivenPrefixBenchmarks
{
    private const string Prefix = "a";
    private const int RandomSeed = 2185; // LC problem number
    private const int AlphabetSize = 26;
    private const int MinWordLength = 3;
    private const int MaxWordLengthExclusive = 8;

    [Params(200, 5_000)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _words = Enumerable.Range(0, WordCount)
            .Select(_ => new string((char)('a' + random.Next(0, AlphabetSize)), random.Next(MinWordLength, MaxWordLengthExclusive)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int StartsWithScan()
    {
        var count = 0;

        for (var i = 0; i < _words.Length; i++)
        {
            if (_words[i].StartsWith(Prefix, StringComparison.Ordinal))
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int TriePerWordHasPrefix()
    {
        var count = 0;

        for (var i = 0; i < _words.Length; i++)
        {
            var trie = new Trie<bool>();
            trie.Set(_words[i], true);

            if (trie.HasPrefix(Prefix))
            {
                count++;
            }
        }

        return count;
    }
}
