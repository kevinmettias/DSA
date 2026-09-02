using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sum of Prefix Scores of Strings (LC 2416): scoring every word by re-scanning the
// whole word list with string.StartsWith for each of its own prefixes (same
// MapSumPairsBenchmarks/ImplementMagicDictionaryBenchmarks precedent) vs. this
// repo's own LowercaseTrie<int>, walked directly through its already-public
// Root/Children (MapSumPairsBenchmarks.WalkTo's own navigation) to increment every
// node's Value while inserting instead of only setting the end-of-word node. Scoring
// a word then just sums the counts already stored along its root-to-leaf path.
// O(wordCount^2 * wordLength) vs. O(wordCount * wordLength) to build the trie once
// plus O(wordCount * wordLength) to score every word.
[MemoryDiagnoser]
public class SumOfPrefixScoresOfStringsBenchmarks
{
    private const int WordLength = 8;
    private const int RandomSeed = 2416; // LC problem number
    private const int AlphabetSize = 26;

    [Params(300, 1_500)]
    public int WordCount;

    private string[] _words = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _words = Enumerable.Range(0, WordCount).Select(_ => RandomWord(random)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long StartsWithScan()
    {
        long total = 0;

        foreach (var word in _words)
        {
            for (var len = 1; len <= word.Length; len++)
            {
                var prefix = word[..len];

                foreach (var candidate in _words)
                {
                    if (candidate.StartsWith(prefix, StringComparison.Ordinal))
                    {
                        total++;
                    }
                }
            }
        }

        return total;
    }

    [Benchmark]
    public long PrefixCountingTrie()
    {
        var trie = new LowercaseTrie<int>();

        foreach (var word in _words)
        {
            Insert(trie.Root, word);
        }

        long total = 0;

        foreach (var word in _words)
        {
            total += ScoreOf(trie.Root, word);
        }

        return total;
    }

    private static void Insert(LowercaseTrieNode<int> root, string word)
    {
        var current = root;

        foreach (var ch in word)
        {
            current = current.Children[ch - 'a'] ??= new LowercaseTrieNode<int>();
            current.Value++;
        }
    }

    private static int ScoreOf(LowercaseTrieNode<int> root, string word)
    {
        var current = root;
        var score = 0;

        foreach (var ch in word)
        {
            current = current.Children[ch - 'a']!;
            score += current.Value;
        }

        return score;
    }

    private static string RandomWord(Random random)
        => new(Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
}
