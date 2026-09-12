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

        public bool Search(string searchWord) => _dictionary.Any(word => IsOneCharacterAway(word, searchWord));

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

        public bool Search(string searchWord) => Search(_trie.Root, searchWord, 0, usedSubstitution: false);

        private static bool Search(LowercaseTrieNode<bool> node, string searchWord, int index, bool usedSubstitution)
        {
            if (index == searchWord.Length)
            {
                return usedSubstitution && node.HasValue;
            }

            var target = searchWord[index] - 'a';

            for (var candidate = 0; candidate < LowercaseTrieNode<bool>.AlphabetSize; candidate++)
            {
                var next = node.Children[candidate];

                if (next is not null
                    && TryDescend(next, searchWord, new SearchState(index, usedSubstitution), candidate == target))
                {
                    return true;
                }
            }

            return false;
        }

        private readonly record struct SearchState(int Index, bool UsedSubstitution);

        private static bool TryDescend(
            LowercaseTrieNode<bool> next, string searchWord, SearchState state, bool isExactMatch)
        {
            if (isExactMatch)
            {
                return Search(next, searchWord, state.Index + 1, state.UsedSubstitution);
            }

            return !state.UsedSubstitution && Search(next, searchWord, state.Index + 1, usedSubstitution: true);
        }
    }
}
