using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ImplementMagicDictionary;

// LeetCode 676. Implement Magic Dictionary: buildDict is this repo's own
// LowercaseTrie<bool>.Set for every dictionary word (all-lowercase alphabet is
// exactly LowercaseTrie's committed domain). Search isn't a prefix/lookup query
// Trie<TValue>'s own Set/HasKey/TryGetValue/HasPrefix surface already provides, so
// it's a manual DFS walking the trie's already-public Root/Children directly (the
// same navigation LowercaseTrieTests.WalkTo/ReplaceWordsBenchmarks.ShortestRootPrefix
// already rehearse), trying every one-character substitution at exactly one position
// and matching exactly everywhere else - composed here rather than built as a new
// production primitive.
public sealed class ImplementMagicDictionaryTests
{
    [Fact]
    public void Search_LeetCodeExample_MatchesExpectedSequence()
    {
        var dictionary = new MagicDictionary();
        dictionary.BuildDict(["hello", "leetcode"]);

        Assert.False(dictionary.Search("hello"));
        Assert.True(dictionary.Search("hhllo"));
        Assert.False(dictionary.Search("hell"));
        Assert.False(dictionary.Search("leetcoded"));
    }

    [Fact]
    public void Search_NoDictionaryWordSameLength_ReturnsFalse()
    {
        var dictionary = new MagicDictionary();
        dictionary.BuildDict(["hello"]);

        Assert.False(dictionary.Search("hi"));
    }

    [Fact]
    public void Search_MultipleCharacterDifference_ReturnsFalse()
    {
        var dictionary = new MagicDictionary();
        dictionary.BuildDict(["hello"]);

        Assert.False(dictionary.Search("hxllx"));
    }

    private sealed class MagicDictionary
    {
        private readonly LowercaseTrie<bool> _trie = new();

        public void BuildDict(string[] dictionary)
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

        private static bool TryDescend(LowercaseTrieNode<bool> next, string searchWord, SearchState state, bool isExactMatch)
        {
            if (isExactMatch)
            {
                return Search(next, searchWord, state.Index + 1, state.UsedSubstitution);
            }

            return !state.UsedSubstitution && Search(next, searchWord, state.Index + 1, usedSubstitution: true);
        }
    }
}
