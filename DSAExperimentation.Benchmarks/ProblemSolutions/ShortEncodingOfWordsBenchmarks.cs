using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Short Encoding of Words (LC 820): the textbook O(n^2 * L) pairwise EndsWith scan
// vs. two genuinely different repo-primitive strategies for the same "drop every
// word that's a suffix of some other word" question - this repo's own Set<string>
// (evict every proper suffix of each word as it's scanned) and its bounded-alphabet
// LowercaseTrie<TValue> (insert every REVERSED word; a word only needs its own
// encoding when its reversed node is a trie leaf, i.e. no other reversed word
// extends past it). Words are generated from a small shared-suffix pool so real
// suffix redundancy exists for all three strategies to exploit, not just
// coincidental overlap.
[MemoryDiagnoser]
public class ShortEncodingOfWordsBenchmarks
{
    private static readonly string[] SuffixPool =
        ["e", "me", "time", "bell", "ing", "ation", "tion", "er", "ed", "s"];

    [Params(500, 5_000)]
    public int Length;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(820);
        var words = new HashSet<string>();

        while (words.Count < Length)
        {
            var suffix = SuffixPool[random.Next(SuffixPool.Length)];
            var prefixLength = random.Next(0, 5);
            var prefix = new char[prefixLength];

            for (var i = 0; i < prefixLength; i++)
            {
                prefix[i] = (char)('a' + random.Next(26));
            }

            words.Add(new string(prefix) + suffix);
        }

        _words = [.. words];
    }

    [Benchmark(Baseline = true)]
    public int NestedLoopSuffixCheck()
    {
        var length = 0;

        for (var i = 0; i < _words.Length; i++)
        {
            var isSuffixOfAnother = false;

            for (var j = 0; j < _words.Length; j++)
            {
                if (i != j && _words[j].Length > _words[i].Length && _words[j].EndsWith(_words[i], StringComparison.Ordinal))
                {
                    isSuffixOfAnother = true;
                    break;
                }
            }

            if (!isSuffixOfAnother)
            {
                length += _words[i].Length + 1;
            }
        }

        return length;
    }

    [Benchmark]
    public int SetBasedSuffixRemoval()
    {
        var remaining = new Set<string>();

        foreach (var word in _words)
        {
            remaining.TryAdd(word);
        }

        foreach (var word in _words)
        {
            for (var i = 1; i < word.Length; i++)
            {
                remaining.TryRemove(word[i..]);
            }
        }

        var length = 0;

        foreach (var word in _words)
        {
            if (remaining.Has(word))
            {
                length += word.Length + 1;
            }
        }

        return length;
    }

    [Benchmark]
    public int LowercaseTrieLeafCount()
    {
        var trie = new LowercaseTrie<bool>();

        foreach (var word in _words)
        {
            trie.Set(Reverse(word), true);
        }

        var length = 0;

        foreach (var word in _words)
        {
            var node = trie.Root;

            for (var i = word.Length - 1; i >= 0; i--)
            {
                node = node.Children[word[i] - 'a']!;
            }

            if (IsLeaf(node))
            {
                length += word.Length + 1;
            }
        }

        return length;
    }

    private static string Reverse(string word)
    {
        var chars = word.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    private static bool IsLeaf(LowercaseTrieNode<bool> node)
    {
        foreach (var child in node.Children)
        {
            if (child is not null)
            {
                return false;
            }
        }

        return true;
    }
}
