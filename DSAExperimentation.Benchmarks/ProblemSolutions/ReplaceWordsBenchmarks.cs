using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Replace Words (LC 648): scanning the whole dictionary and testing StartsWith for
// every sentence word - the naive approach has to check every root anyway, since
// stopping at the first match found doesn't guarantee that match is the SHORTEST one
// - vs. this repo's own LowercaseTrie<bool>, where the shortest root falls out for
// free by walking the word once and stopping at the first HasValue node reached.
// O(words * dictionarySize * rootLength) vs. O(words * wordLength). Half the
// sentence words are built by prepending a real dictionary root (forcing genuine
// prefix-match work in both strategies), the other half are fully random (forcing a
// full, unmatched dictionary scan in the baseline) - the same "don't let either
// strategy short-circuit trivially" intent TwoSumBenchmarks' unreachable target uses.
[MemoryDiagnoser]
public class ReplaceWordsBenchmarks
{
    private const int RootLength = 4;
    private const int WordLength = 9;

    // LC problem number, reused as the Random seed for reproducible benchmark input.
    private const int RandomSeed = 648;

    // Alternates sentence words between "prefixed with a real dictionary root" and
    // "fully random" so half the words exercise each strategy.
    private const int AlternationModulus = 2;

    private const int LowercaseAlphabetSize = 26;

    [Params(50, 1_000)]
    public int DictionarySize;

    private string[] _dictionary = null!;
    private string[] _sentence = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _dictionary = Enumerable.Range(0, DictionarySize)
            .Select(_ => RandomWord(random, RootLength))
            .Distinct()
            .ToArray();
        _sentence = Enumerable.Range(0, DictionarySize)
            .Select(i => i % AlternationModulus == 0
                ? _dictionary[random.Next(_dictionary.Length)] + RandomWord(random, WordLength - RootLength)
                : RandomWord(random, WordLength))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public string DictionaryScanPerWord()
    {
        var replaced = new string[_sentence.Length];

        for (var i = 0; i < _sentence.Length; i++)
        {
            replaced[i] = ShortestMatchingRoot(_sentence[i]);
        }

        return string.Join(' ', replaced);
    }

    [Benchmark]
    public string LowercaseTrieWalk()
    {
        var trie = new LowercaseTrie<bool>();

        foreach (var root in _dictionary)
        {
            trie.Set(root, true);
        }

        var replaced = new string[_sentence.Length];

        for (var i = 0; i < _sentence.Length; i++)
        {
            replaced[i] = ShortestRootPrefix(trie, _sentence[i]);
        }

        return string.Join(' ', replaced);
    }

    private string ShortestMatchingRoot(string word)
    {
        string? best = null;

        foreach (var root in _dictionary)
        {
            if (word.StartsWith(root, StringComparison.Ordinal) && (best is null || root.Length < best.Length))
            {
                best = root;
            }
        }

        return best ?? word;
    }

    private static string ShortestRootPrefix(LowercaseTrie<bool> trie, string word)
    {
        var current = trie.Root;

        for (var i = 0; i < word.Length; i++)
        {
            var next = current.Children[word[i] - 'a'];

            if (next is null)
            {
                return word;
            }

            current = next;

            if (current.HasValue)
            {
                return word[..(i + 1)];
            }
        }

        return word;
    }

    private static string RandomWord(Random random, int length)
        => new(Enumerable.Range(0, length).Select(_ => (char)('a' + random.Next(LowercaseAlphabetSize))).ToArray());
}
