using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Implement Magic Dictionary (LC 676): scanning the whole dictionary and computing a
// Hamming distance for every search word (same ReplaceWordsBenchmarks precedent -
// build+scan cost has to be paid every call either way, so both benchmark methods
// process a full batch of search words per call rather than just one, the same way
// ReplaceWordsBenchmarks batches sentence words) vs. this repo's own
// LowercaseTrie<bool>, walked via a one-substitution DFS over its already-public
// Root/Children. O(words * dictionarySize * wordLength) vs. O(dictionarySize *
// wordLength) to build the trie once plus O(words * wordLength * alphabetSize) to
// search it. Each search word is exactly one dictionary word with a single character
// substituted (guaranteed to be a real match), so neither strategy gets to
// short-circuit on an early "definitely no match" bail-out.
[MemoryDiagnoser]
public class ImplementMagicDictionaryBenchmarks
{
    private const int WordLength = 8;

    private const int RandomSeed = 676; // LeetCode problem number

    private const int AlphabetSize = 26;

    [Params(10_000, 30_000)]
    public int DictionarySize;

    private string[] _dictionary = null!;
    private string[] _searchWords = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _dictionary = Enumerable.Range(0, DictionarySize).Select(_ => RandomWord(random)).Distinct().ToArray();
        _searchWords = _dictionary.Select(word => OneCharacterAway(word, random)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var matches = 0;

        foreach (var searchWord in _searchWords)
        {
            if (_dictionary.Any(word => IsOneCharacterAway(word, searchWord)))
            {
                matches++;
            }
        }

        return matches;
    }

    private static bool IsOneCharacterAway(string word, string searchWord)
    {
        if (word.Length != searchWord.Length)
        {
            return false;
        }

        var differences = 0;

        for (var i = 0; i < word.Length && differences <= 1; i++)
        {
            if (word[i] != searchWord[i])
            {
                differences++;
            }
        }

        return differences == 1;
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

        foreach (var searchWord in _searchWords)
        {
            if (Search(trie.Root, new SearchState(searchWord, 0, UsedSubstitution: false)))
            {
                matches++;
            }
        }

        return matches;
    }

    private readonly record struct SearchState(string SearchWord, int Index, bool UsedSubstitution);

    private static bool Search(LowercaseTrieNode<bool> node, SearchState state)
    {
        if (state.Index == state.SearchWord.Length)
        {
            return state.UsedSubstitution && node.HasValue;
        }

        return SearchChildren(node, state);
    }

    private static bool SearchChildren(LowercaseTrieNode<bool> node, SearchState state)
    {
        var target = state.SearchWord[state.Index] - 'a';

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
        var nextState = state with { Index = state.Index + 1 };

        if (isTargetCandidate)
        {
            return Search(next, nextState);
        }

        return !state.UsedSubstitution && Search(next, nextState with { UsedSubstitution = true });
    }

    private static string RandomWord(Random random)
        => new(Enumerable.Range(0, WordLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());

    private static string OneCharacterAway(string word, Random random)
    {
        var characters = word.ToCharArray();
        var position = random.Next(word.Length);
        characters[position] = (char)('a' + ((characters[position] - 'a' + 1) % AlphabetSize));
        return new string(characters);
    }
}
