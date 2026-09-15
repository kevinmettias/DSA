using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.ImplementMagicDictionary;

// LeetCode 676. Implement Magic Dictionary.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// buildDict constructor step and a search operation, not a single return value
// - so the strategy choice is which implementation backs it, the same
// CreateBy<Strategy> factory shape AllOneDataStructureSolution/LRUCacheSolution
// use for their own design problems.
//
// CreateByTrieSearch buildDict's every dictionary word into this repo's own
// LowercaseTrie<bool> (all-lowercase alphabet is exactly LowercaseTrie's
// committed domain). Search isn't a prefix/lookup query Trie<TValue>'s own
// Set/HasKey/TryGetValue/HasPrefix surface already provides, so it's a manual
// DFS walking the trie's already-public Root/Children directly (the same
// navigation LowercaseTrieTests.WalkTo/ReplaceWordsBenchmarks.ShortestRootPrefix
// already rehearse), trying every one-character substitution at exactly one
// position and matching exactly everywhere else - composed here rather than
// built as a new production primitive.
//
// CreateByBruteForce is the textbook baseline this composition has to justify
// itself against: buildDict just remembers the word list, and search scans it
// computing a Hamming distance against every candidate.
internal static class ImplementMagicDictionarySolution
{
    public static IMagicDictionary CreateByBruteForce() => new BruteForceMagicDictionary();

    public static IMagicDictionary CreateByTrieSearch() => new TrieSearchMagicDictionary();

    internal interface IMagicDictionary
    {
        void BuildDict(IEnumerable<string> dictionary);

        bool Search(string searchWord);
    }

    private sealed class BruteForceMagicDictionary : IMagicDictionary
    {
        private string[] _dictionary = [];

        public void BuildDict(IEnumerable<string> dictionary) => _dictionary = dictionary.ToArray();

        public bool Search(string searchWord) =>
            _dictionary.Any(word => IsOneCharacterAway(new DictionaryWord(word), new SearchWord(searchWord)));

        private static bool IsOneCharacterAway(DictionaryWord word, SearchWord searchWord)
        {
            if (word.Text.Length != searchWord.Text.Length)
            {
                return false;
            }

            var differences = 0;

            for (var i = 0; i < word.Text.Length && differences <= 1; i++)
            {
                if (word.Text[i] != searchWord.Text[i])
                {
                    differences++;
                }
            }

            return differences == 1;
        }

        // The two ends of a Hamming comparison, named for the roles they play here rather
        // than left as two adjacent `string` positions a caller could hand over the wrong
        // way round with the compiler none the wiser. `word` is the dictionary entry the
        // scan is walking; `searchWord` is the one being looked for.
        private readonly record struct DictionaryWord(string Text);

        private readonly record struct SearchWord(string Text);
    }

    private sealed class TrieSearchMagicDictionary : IMagicDictionary
    {
        private readonly LowercaseTrie<bool> _trie = new();

        public void BuildDict(IEnumerable<string> dictionary)
        {
            foreach (var word in dictionary)
            {
                _trie.Set(word, true);
            }
        }

        public bool Search(string searchWord) => Search(_trie.Root, searchWord, 0, SubstitutionBudget.Available);

        private static bool Search(
            LowercaseTrieNode<bool> node, string searchWord, int index, SubstitutionBudget budget)
        {
            if (index == searchWord.Length)
            {
                return budget == SubstitutionBudget.Spent && node.HasValue;
            }

            var target = searchWord[index] - 'a';

            for (var candidate = 0; candidate < LowercaseAlphabet.Size; candidate++)
            {
                var next = node.Children[candidate];

                if (next is not null
                    && TryDescend(next, searchWord, new SearchState(index, budget), candidate == target
                        ? LetterMatch.Exact
                        : LetterMatch.Substituted))
                {
                    return true;
                }
            }

            return false;
        }

        private readonly record struct SearchState(int Index, SubstitutionBudget Budget);

        private static bool TryDescend(
            LowercaseTrieNode<bool> next, string searchWord, SearchState state, LetterMatch match)
        {
            if (match == LetterMatch.Exact)
            {
                return Search(next, searchWord, state.Index + 1, state.Budget);
            }

            return state.Budget == SubstitutionBudget.Available
                && Search(next, searchWord, state.Index + 1, SubstitutionBudget.Spent);
        }

        // Whether the one substitution a search is allowed to spend has been spent yet:
        // a state the call site names, where a bare `true` said it only by position.
        private enum SubstitutionBudget
        {
            Available,
            Spent,
        }

        // Whether this candidate letter is the search word's own letter at this position,
        // or the one substitution being spent on it - named where a rewritten true/false
        // at the call site said it only by position.
        private enum LetterMatch
        {
            Exact,
            Substituted,
        }
    }
}
