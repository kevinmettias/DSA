using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Word in Dictionary (LC 720): checking, for every word, whether each of its
// prefixes exists ANYWHERE in the word list via a linear scan (no index at all) - vs.
// this repo's own LowercaseTrie<bool>, where "is this prefix a complete word" is a
// single O(1) HasValue check per step of one root-to-leaf walk. O(words^2 * wordLength)
// vs. O(totalCharacters). Setup grows each word from the previous one character at a
// time (occasionally starting a fresh chain), the same "build real matches, not
// coincidental collisions" intent ReplaceWordsBenchmarks' half-real-root generator
// uses - which guarantees most words are genuinely buildable, forcing both strategies
// through their full prefix-chain walk instead of an early mismatch.
[MemoryDiagnoser]
public class LongestWordInDictionaryBenchmarks
{
    // LC problem number, used as the deterministic setup seed.
    private const int RandomSeed = 720;
    private const int AlphabetSize = 26;
    private const int FreshChainChance = 4;
    private const string EmptyPrefix = "";
    private const string NoQualifyingWord = "";

    [Params(50, 500)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var words = new List<string>(WordCount);
        var current = EmptyPrefix;

        while (words.Count < WordCount)
        {
            current = current.Length == 0 || random.Next(FreshChainChance) == 0
                ? ((char)('a' + random.Next(AlphabetSize))).ToString()
                : current + (char)('a' + random.Next(AlphabetSize));

            words.Add(current);
        }

        _words = words.ToArray();
    }

    [Benchmark(Baseline = true)]
    public string DictionaryScanPerPrefix()
    {
        string? best = null;

        foreach (var word in _words)
        {
            if (!IsBuildable(word))
            {
                continue;
            }

            if (best is null || word.Length > best.Length || (word.Length == best.Length && string.CompareOrdinal(word, best) < 0))
            {
                best = word;
            }
        }

        return best ?? NoQualifyingWord;
    }

    private bool IsBuildable(string word)
    {
        for (var len = 1; len <= word.Length; len++)
        {
            var prefix = word[..len];
            var found = false;

            foreach (var candidate in _words)
            {
                if (candidate == prefix)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public string LowercaseTrieWalk()
    {
        var trie = BuildTrie(_words);

        return FindLongestWord(trie.Root, EmptyPrefix, NoQualifyingWord);
    }

    private static LowercaseTrie<bool> BuildTrie(string[] words)
    {
        var trie = new LowercaseTrie<bool>();

        foreach (var word in words)
        {
            trie.Set(word, true);
        }

        return trie;
    }

    private static string FindLongestWord(LowercaseTrieNode<bool> node, string prefix, string best)
    {
        best = BetterOf(prefix, best);

        for (var i = 0; i < LowercaseTrieNode<bool>.AlphabetSize; i++)
        {
            best = VisitChild(node, i, prefix, best);
        }

        return best;
    }

    private static string VisitChild(LowercaseTrieNode<bool> node, int childIndex, string prefix, string best)
    {
        var child = node.Children[childIndex];

        if (child is null || !child.HasValue)
        {
            return best;
        }

        return FindLongestWord(child, prefix + (char)('a' + childIndex), best);
    }

    private static string BetterOf(string prefix, string best)
    {
        if (prefix.Length == 0)
        {
            return best;
        }

        var isBetter = prefix.Length > best.Length || (prefix.Length == best.Length && string.CompareOrdinal(prefix, best) < 0);

        return isBetter ? prefix : best;
    }
}
