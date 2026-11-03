using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.LeetCode.Solutions.Conventions;
using RootTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetTrie<
    DSAExperimentation.Domain.Language.English.EnglishLowercase,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.PackedTrieLayout,
    bool>;

namespace DSAExperimentation.LeetCode.Solutions.ReplaceWords;

// LeetCode 648. Replace Words: replace every sentence word with the shortest
// dictionary root it starts with, leaving words with no matching root unchanged.
//
// The two strategies differ in how they find that shortest root - scanning the whole
// dictionary and checking StartsWith for each candidate (every root has to be
// checked anyway, since the first match found is not guaranteed to be the shortest),
// vs. this repo's own AlphabetTrie, where the shortest root falls out for free
// by walking the word once and stopping at the first HasValue node reached.
[LeetCodeQuestion("replace-words", LeetCodeAnswerOrder.AsWritten)]
internal static class ReplaceWordsSolution
{
    // The textbook answer: scan every root against every word with BCL StartsWith.
    // O(words * dictionarySize * rootLength) vs. the trie walk's O(words * wordLength).
    public static string ReplaceByDictionaryScan(string[] dictionary, string sentence)
    {
        var words = sentence.Split(' ');

        for (var i = 0; i < words.Length; i++)
        {
            words[i] = ShortestMatchingRoot(dictionary, words[i]);
        }

        return string.Join(' ', words);
    }

    private static string ShortestMatchingRoot(string[] dictionary, string word)
    {
        string? best = null;

        foreach (var root in dictionary)
        {
            if (IsBetterRootThanBest(new SentenceWord(word), new DictionaryRoot(root), best))
            {
                best = root;
            }
        }

        return best ?? word;
    }

    // A root wins over the best one found so far when the word starts with it and
    // it is either the first match or shorter than everything matched yet.
    private static bool IsBetterRootThanBest(SentenceWord word, DictionaryRoot root, string? best)
        => word.Text.StartsWith(root.Text, StringComparison.Ordinal)
            && (best is null || root.Text.Length < best.Length);

    // This repo's own AlphabetTrie over the lowercase letters: every root is set true, and
    // walking a sentence word stops at the first HasValue node - the shortest matching root,
    // because the walk visits nodes in root-to-leaf order. The packed layout: the trie is
    // built once per call and walked once per word, which packed nodes measured faster for
    // (IMPROVEMENT-WORKLIST, 2026-11-02).
    public static string ReplaceByTrieWalk(string[] dictionary, string sentence)
    {
        var trie = new RootTrie();

        foreach (var root in dictionary)
        {
            trie.Set(root, true);
        }

        var words = sentence.Split(' ');

        for (var i = 0; i < words.Length; i++)
        {
            words[i] = ShortestRootPrefix(trie, words[i]);
        }

        return string.Join(' ', words);
    }

    private static string ShortestRootPrefix(RootTrie trie, string word)
    {
        var current = trie.Root;

        for (var i = 0; i < word.Length; i++)
        {
            var next = current.ChildAt(EnglishLowercase.IndexOf(word[i]));

            if (next is null)
            {
                return word;
            }

            current = next;

            if (current.HasValue)
            {
                return word[..(i + 1)];
            }
        }

        return word;
    }

    // The two ends of the containment test, named for the roles they play here rather
    // than left as two adjacent `string` positions a caller could hand over the wrong
    // way round with the compiler none the wiser. A sentence word is what is being
    // replaced; a dictionary root is a candidate replacement it may start with - and
    // the test is one-directional, since `root` must be a prefix of `word` and never
    // the other way round.
    private readonly record struct SentenceWord(string Text);

    private readonly record struct DictionaryRoot(string Text);
}
