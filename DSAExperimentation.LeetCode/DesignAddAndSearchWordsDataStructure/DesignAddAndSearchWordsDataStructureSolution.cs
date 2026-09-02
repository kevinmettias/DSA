using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.LeetCode.DesignAddAndSearchWordsDataStructure;

// LeetCode 211. Design Add and Search Words Data Structure: an instance API
// (addWord/search) where a search pattern may contain '.' as a single-character
// wildcard matching any letter. A Design problem's whole point is a sequence of
// mutating calls against one instance, so "every strategy for the problem"
// (ARCHITECTURE.md 17.3) takes the form of two classes implementing the shared
// IWordDictionaryStrategy surface below.
internal static class DesignAddAndSearchWordsDataStructureSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IWordDictionaryStrategy
    {
        void AddWord(string word);

        bool Search(string pattern);
    }

    // The textbook answer: every added word kept in a flat BCL List<string>,
    // matched by a per-character scan against every stored word - deliberately
    // without this repo's Trie, the arm WordDictionaryByTrie has to justify
    // itself against.
    internal sealed class WordDictionaryByLinearScan : IWordDictionaryStrategy
    {
        private readonly List<string> _words = [];

        public void AddWord(string word) => _words.Add(word);

        public bool Search(string pattern) => _words.Exists(word => MatchesPattern(word, pattern));
    }

    // This repo's own Trie<bool> gives the no-wildcard case an O(m) descent
    // instead of scanning every word. A '.' still falls back to scanning the
    // words actually added: Trie<TValue> deliberately exposes no node-level
    // traversal for a caller to branch a wildcard search over (see Trie.cs's
    // own doc comment on why it stays witness-less), so there is no repo
    // primitive to walk instead.
    internal sealed class WordDictionaryByTrie : IWordDictionaryStrategy
    {
        private readonly Trie<bool> _exactWords = new();
        private readonly List<string> _allWords = [];

        public void AddWord(string word)
        {
            _exactWords.Set(word, true);
            _allWords.Add(word);
        }

        public bool Search(string pattern) =>
            pattern.Contains('.')
                ? _allWords.Exists(word => MatchesPattern(word, pattern))
                : _exactWords.HasKey(pattern);
    }

    // Shared wildcard matching: same length, and every position either matches
    // the stored word exactly or the pattern has '.' there.
    private static bool MatchesPattern(string word, string pattern)
    {
        if (word.Length != pattern.Length)
        {
            return false;
        }

        for (var i = 0; i < word.Length; i++)
        {
            if (pattern[i] != '.' && pattern[i] != word[i])
            {
                return false;
            }
        }

        return true;
    }
}
