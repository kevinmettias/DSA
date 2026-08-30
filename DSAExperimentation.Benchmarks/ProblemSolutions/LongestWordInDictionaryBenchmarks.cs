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
    [Params(50, 500)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(720);
        var words = new List<string>(WordCount);
        var current = "";

        while (words.Count < WordCount)
        {
            current = current.Length == 0 || random.Next(4) == 0
                ? ((char)('a' + random.Next(26))).ToString()
                : current + (char)('a' + random.Next(26));

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

        return best ?? "";
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
        var trie = new LowercaseTrie<bool>();

        foreach (var word in _words)
        {
            trie.Set(word, true);
        }

        var best = "";

        Walk(trie.Root, "");

        return best;

        void Walk(LowercaseTrieNode<bool> node, string prefix)
        {
            if (prefix.Length > 0 &&
                (prefix.Length > best.Length || (prefix.Length == best.Length && string.CompareOrdinal(prefix, best) < 0)))
            {
                best = prefix;
            }

            for (var i = 0; i < LowercaseTrieNode<bool>.AlphabetSize; i++)
            {
                var child = node.Children[i];

                if (child is not null && child.HasValue)
                {
                    Walk(child, prefix + (char)('a' + i));
                }
            }
        }
    }
}
