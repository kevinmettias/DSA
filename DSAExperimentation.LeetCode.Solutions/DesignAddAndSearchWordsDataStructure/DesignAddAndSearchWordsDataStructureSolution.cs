using WordTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.Trie<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        bool>,
    bool>;
using WordNode = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieNode<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
        bool>,
    bool>;

namespace DSAExperimentation.LeetCode.Solutions.DesignAddAndSearchWordsDataStructure;

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

        public bool Search(string pattern) =>
            _words.Exists(word => IsPatternMatch(new StoredWord(word), new SearchPattern(pattern)));
    }

    // This repo's own trie over the lowercase letters: a letter of the pattern is one step down, and
    // a '.' branches over the node's children, which the slot store walks in letter order. Only the
    // children present are visited, so a wildcard costs what the dictionary holds below that point,
    // not all 26 letters, and a pattern with no '.' is a single descent.
    internal sealed class WordDictionaryByTrie : IWordDictionaryStrategy
    {
        private readonly WordTrie _words = new();

        public void AddWord(string word) => _words.Set(word, true);

        public bool Search(string pattern) => HasMatchBelow(_words.Root, new PatternCursor(pattern, 0));
    }

    // The linear scan's wildcard matching: same length, and every position either matches
    // the stored word exactly or the pattern has '.' there.
    private static bool IsPatternMatch(StoredWord word, SearchPattern pattern)
    {
        if (word.Text.Length != pattern.Text.Length)
        {
            return false;
        }

        for (var i = 0; i < word.Text.Length; i++)
        {
            if (pattern.Text[i] != '.' && pattern.Text[i] != word.Text[i])
            {
                return false;
            }
        }

        return true;
    }

    // Whether some added word matches the rest of the pattern below `node`: the pattern's end must land
    // on a node a word ends at, a letter steps to its child, and '.' tries every child in turn.
    private static bool HasMatchBelow(WordNode node, PatternCursor rest)
    {
        if (rest.IsAtEnd)
        {
            return node.HasValue;
        }

        if (rest.Letter != '.')
        {
            var child = node.Children.ChildFor(rest.Letter);

            return child is not null && HasMatchBelow(child, rest.Next);
        }

        foreach (var (_, child) in node.Children)
        {
            if (HasMatchBelow(child, rest.Next))
            {
                return true;
            }
        }

        return false;
    }

    // The two sides of a wildcard match: the word that was added, and the pattern it is being
    // tested against. Only the pattern may carry '.', so the relation is one-directional - and
    // the two `string` positions it used to be did not say so.
    private readonly record struct StoredWord(string Text);

    private readonly record struct SearchPattern(string Text);

    // A pattern and how far into it a trie walk has matched: the position only means anything against
    // its own pattern, so the two travel together.
    private readonly record struct PatternCursor(string Pattern, int Position)
    {
        public bool IsAtEnd => Position == Pattern.Length;

        public char Letter => Pattern[Position];

        public PatternCursor Next => this with { Position = Position + 1 };
    }
}
