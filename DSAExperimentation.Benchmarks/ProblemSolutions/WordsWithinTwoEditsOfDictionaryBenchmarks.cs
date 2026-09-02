using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Words Within Two Edits of Dictionary (LC 2452): scanning the whole dictionary and
// counting mismatched characters for every query (same per-call batch shape
// ImplementMagicDictionaryBenchmarks uses) vs. this repo's own LowercaseTrie<bool>,
// walked via a remaining-edit-budget DFS over its already-public Root/Children -
// ImplementMagicDictionaryBenchmarks' one-substitution walk generalized from a
// budget of exactly 1 to "at most 2". O(queries * dictionarySize * wordLength) vs.
// O(dictionarySize * wordLength) to build the trie once plus a budgeted walk per
// query. Each query is exactly one dictionary word with 0, 1, or 2 characters
// changed (guaranteed to be a real match), so neither strategy short-circuits on an
// early "definitely no match" bail-out.
[MemoryDiagnoser]
public class WordsWithinTwoEditsOfDictionaryBenchmarks
{
    private const int WordLength = 8;

    private const int RandomSeed = 2452; // LeetCode problem number

    private const int AlphabetSize = 26;

    private const int MaxEdits = 2;

    [Params(2_000, 6_000)]
    public int DictionarySize;

    private string[] _dictionary = null!;
    private string[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _dictionary = Enumerable.Range(0, DictionarySize).Select(_ => RandomWord(random)).Distinct().ToArray();
        _queries = _dictionary.Select(word => WithinEditBudget(word, random)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var matches = 0;

        foreach (var query in _queries)
        {
            if (_dictionary.Any(word => IsWithinEditBudget(word, query)))
            {
                matches++;
            }
        }

        return matches;
    }

    private static bool IsWithinEditBudget(string word, string query)
    {
        var differences = 0;

        for (var i = 0; i < word.Length && differences <= MaxEdits; i++)
        {
            if (word[i] != query[i])
            {
                differences++;
            }
        }

        return differences <= MaxEdits;
    }

    [Benchmark]
    public int TrieSearch()
    {
        var trie = new LowercaseTrie<bool>();

        foreach (var word in _dictionary)
        {
            trie.Set(word, true);
        }

        var matches = 0;

        foreach (var query in _queries)
        {
            if (Search(trie.Root, new SearchState(query, 0, MaxEdits)))
            {
                matches++;
            }
        }

        return matches;
    }

    private readonly record struct SearchState(string Query, int Index, int RemainingEdits);

    private static bool Search(LowercaseTrieNode<bool> node, SearchState state)
    {
        if (state.Index == state.Query.Length)
        {
            return node.HasValue;
        }

        return SearchChildren(node, state);
    }

    private static bool SearchChildren(LowercaseTrieNode<bool> node, SearchState state)
    {
        var target = state.Query[state.Index] - 'a';

        for (var candidate = 0; candidate < LowercaseTrieNode<bool>.AlphabetSize; candidate++)
        {
            var next = node.Children[candidate];

            if (next is not null && TryMatchCandidate(next, state, isTargetCandidate: candidate == target))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryMatchCandidate(LowercaseTrieNode<bool> next, SearchState state, bool isTargetCandidate)
    {
        var nextBudget = isTargetCandidate ? state.RemainingEdits : state.RemainingEdits - 1;

        if (nextBudget < 0)
        {
            return false;
        }

        return Search(next, state with { Index = state.Index + 1, RemainingEdits = nextBudget });
    }

    private static string RandomWord(Random random)
        => new(Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());

    private static string WithinEditBudget(string word, Random random)
    {
        var characters = word.ToCharArray();
        var editCount = random.Next(MaxEdits + 1); // 0, 1, or 2 edits

        for (var i = 0; i < editCount; i++)
        {
            var position = random.Next(word.Length);
            characters[position] = (char)('a' + ((characters[position] - 'a' + 1) % AlphabetSize));
        }

        return new string(characters);
    }
}
