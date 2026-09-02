using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Check If a Word Occurs As a Prefix of Any Word in a Sentence (LC 1455): a direct
// string.StartsWith scan over the split sentence vs. this repo's own Trie<bool> -
// one word inserted per candidate, then HasPrefix(searchWord) (ImplementTrieTests'
// exact Insert+HasPrefix pairing, LC 208), re-run until the first match.
// searchWord never matches, so both strategies scan every word.
[MemoryDiagnoser]
public class CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceBenchmarks
{
    private const string SearchWord = "zzzunmatched";
    private const int RandomSeed = 1455; // LC problem number
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
        for (var i = 0; i < _words.Length; i++)
        {
            if (_words[i].StartsWith(SearchWord, StringComparison.Ordinal))
            {
                return i + 1;
            }
        }

        return -1;
    }

    [Benchmark]
    public int TriePerWordHasPrefix()
    {
        for (var i = 0; i < _words.Length; i++)
        {
            var trie = new Trie<bool>();
            trie.Set(_words[i], true);

            if (trie.HasPrefix(SearchWord))
            {
                return i + 1;
            }
        }

        return -1;
    }
}
